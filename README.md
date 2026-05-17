# YubiKey-Identity-Guard 🛡️

### Enterprise Identity Governance & Hardware Validation

**YubiKey-Identity-Guard** is a professional security application in **C#** that ensures physical hardware tokens (YubiKeys) comply with a company's internal IT compliance policies. The tool validates cryptographic identities directly on the hardware, thus closing the gap between Identity Management (IAM) and IT Governance.

This project demonstrates the practical application of **Public Key Infrastructure (PKI)** and **hardware security**, inspired by my work in IT infrastructure at **Fraport AG**.

---

## 🎯 Focus & Use Case

In enterprise environments, security tokens must adhere to strict rules. This tool automates the checking of:
* **Certificate Compliance:** Do the stored certificates (PIV slots) meet the requirements for key length and signature algorithms?
* **Lifecycle Management:** Timely warning about expiring certificates on the token.
* **Hardware Integrity:** Reading firmware versions and serial numbers for inventory and security checks.

---

## ✨ Features (In Development)

- [x] **Hardware Detection:** Automatic detection of YubiKey devices via USB.
- [ ] **PIV Slot Audit:** Deep inspection of certificates in slots 9a, 9c, 9d, and 9e.
- [ ] **Policy Engine:** Definition of governance rules (e.g., RSA 2048-bit minimum).
- [ ] **Reporting:** Generation of compliance reports for IT audits.

---

## 🛠️ Technology Stack

* **Language:** C# / .NET
* **Frameworks:** `Yubico.YubiKey` SDK, `System.Security.Cryptography`
* **Concepts:** IAM, GRC (Governance, Risk & Compliance), PKI, X.509 Certificates

---

## 📖 Background

This project is a private endeavor. During my time at **Fraport AG**, I contributed to an internal YubiKey management tool. The **YubiKey Identity Guard** abstracts these experiences into an independent tool to show how governance policies can be programmatically enforced at the hardware level.

---

## 🚀 Installation & Test

*Note: Since the project is actively being developed, a physical YubiKey is required for full functionality.*

1. Clone the repository:
   ```bash
   git clone [https://github.com/YOUR_USERNAME/YubiKey-Identity-Guard.git](https://github.com/YOUR_USERNAME/YubiKey-Identity-Guard.git)
