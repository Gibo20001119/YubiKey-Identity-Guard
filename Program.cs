using System;
using System.Linq;
using Yubico.YubiKey;
using System.Threading;

const int Allowed_Serial = 19646323; // Hardcoded for Implementation phase

Console.WriteLine("--- YubiKey Identity Guard: Hardware Test ---");
Console.WriteLine("Press 'STRG + C' to exit\n");

bool isAuthorized = false;
bool keepRunning = true;

while(keepRunning)
{
    // check if the user pressed a key
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(true);
        if(key.Key == ConsoleKey.E) // We use 'E' for Exit
        {
            Console.WriteLine("\n[System] Exiting program...");
            keepRunning = false;
            break;
        }
    }
    
    try 
    {
        // Search for connected YubiKeys
        var yubikeys = YubiKeyDevice.FindAll();
        
        // Check if our specific Master Key is in the list
        bool masterKeyDetected = yubikeys.Any(d => d.SerialNumber == Allowed_Serial);

        if (masterKeyDetected && !isAuthorized)
        {
            // State: Key was inserted and is the master key
            isAuthorized = true;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Access Granted: Master Key Detected (Serial: {Allowed_Serial})");
            Console.ResetColor();
        }
        else if (!masterKeyDetected && isAuthorized)
        {
            // State: Master Key was removed
            isAuthorized = false;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ALARM: Master Key Removed!");
            Console.ResetColor();
        }
        
        // Feedback if no key at all is present (and we aren't authorized anyway)
        if (!yubikeys.Any() && !isAuthorized)
        {
            // Keep console clean, just a small hint or nothing
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Error: {ex.Message}");
    }

    // Interval: 1000ms is a good balance for security and performance
    Thread.Sleep(1000); 
}