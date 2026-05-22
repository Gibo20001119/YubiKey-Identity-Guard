using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration; // Neu für JSON
using Yubico.YubiKey;

class Program
{
    static void Main()
    {
        Console.WriteLine("--- YubiKey Identity Guard ---");
        
        // 1. JSON Konfiguration laden
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("policy.json", optional: false, reloadOnChange: true)
            .Build();

        // 2. In unser Options-Modell mappen
        var options = config.Get<SecurityOptions>() ?? new SecurityOptions();

        Console.WriteLine($"[Config] Required Invalid Ticks: {options.RequiredInvalidTicks}");
        Console.WriteLine($"[Config] Monitored Serials: {string.Join(", ", options.AllowedSerials)}");
        Console.WriteLine("Press 'E' to exit\n");

        // 3. Regel mit den dynamischen Serials füttern
        var rules = new List<IValidationRule<IYubiKeyDevice>>
        {
            new YubiKeySerialRule(options.AllowedSerials)
        };

        var engine = new ValidationEngine<IYubiKeyDevice>(rules);

        bool isRunning = true;
        bool isAuthenticated = false;
        int invalidTicks = 0;
        
        // Wert aus der JSON statt der Hardcoded Const
        int requiredInvalidTicks = options.RequiredInvalidTicks; 

        while (isRunning)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.E)
                {
                    Console.WriteLine("\n[System] Exiting...");
                    break;
                }
            }

            try
            {
                var devices = YubiKeyDevice.FindAll().ToList();
                bool isValidDevicePresent = false;

                foreach (var device in devices)
                {
                    var results = engine.Validate(device);
                    if (results.Any(r => r.IsValid))
                    {
                        isValidDevicePresent = true;
                    }
                }

                // STATE MACHINE
                if (isValidDevicePresent)
                {
                    invalidTicks = 0;

                    if (!isAuthenticated)
                    {
                        isAuthenticated = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] AUTHORIZED STATE ENTERED");
                        Console.ResetColor();
                    }
                }
                else
                {
                    invalidTicks++;

                    if (invalidTicks >= requiredInvalidTicks && isAuthenticated)
                    {
                        isAuthenticated = false;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] UNAUTHORIZED STATE ENTERED");
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[ERROR] {ex.Message}");
                Console.ResetColor();
            }

            Thread.Sleep(1000);
        }
    }
}

// ----------------------------------------------------
// INTERFACE
// ----------------------------------------------------
public interface IValidationRule<T>
{
    ValidationResult Validate(T input);
}

// ----------------------------------------------------
// RESULT MODEL
// ----------------------------------------------------
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = "Info";
}

// ----------------------------------------------------
// CONFIG OPTIONS MODEL
// ----------------------------------------------------
public class SecurityOptions
{
    public List<int> AllowedSerials { get; set; } = new();
    public int RequiredInvalidTicks { get; set; } = 3;
    public bool RequireAtLeastOneDevice { get; set; } = true;
}

// ----------------------------------------------------
// ENGINE
// ----------------------------------------------------
public class ValidationEngine<T>
{
    private readonly List<IValidationRule<T>> _rules;

    public ValidationEngine(List<IValidationRule<T>> rules)
    {
        _rules = rules;
    }

    public List<ValidationResult> Validate(T input)
    {
        List<ValidationResult> results = new();

        foreach (var rule in _rules)
        {
            results.Add(rule.Validate(input));
        }

        return results;
    }
}

// ----------------------------------------------------
// RULE
// ----------------------------------------------------
public class YubiKeySerialRule : IValidationRule<IYubiKeyDevice>
{
    private readonly List<int> _allowedSerials;

    public YubiKeySerialRule(List<int> allowedSerials)
    {
        _allowedSerials = allowedSerials;
    }

    public ValidationResult Validate(IYubiKeyDevice device)
    {
        bool valid = device.SerialNumber.HasValue && _allowedSerials.Contains(device.SerialNumber.Value);

        return new ValidationResult
        {
            IsValid = valid,
            Message = valid 
                ? $"Authorized YubiKey detected (Serial: {device.SerialNumber})."
                : $"Unauthorized YubiKey detected (Serial: {device.SerialNumber}).",
            Severity = valid ? "Info" : "Critical"
        };
    }
}