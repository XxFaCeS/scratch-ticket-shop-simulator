# 🎰 Scratch Ticket Shop Simulator

> Ein Shop-Simulator bei dem du einen Rubbellos-Kiosk besitzt, ausbaust und zur Lotto-Filialkette erweiterst!

---

## 🎮 Über das Spiel

Starte mit einem kleinen Kiosk und verkaufe Rubbellose an Kunden. Verwalte dein Lager, stelle Mitarbeiter ein, schalte neue Lostypen frei und erweitere deinen Laden Schritt für Schritt. Reagiere auf ungeduldige Kunden, seltene Jackpot-Ereignisse und Lieferanten-Deals – werde zum größten Rubbellos-Händler der Stadt!

---

## ✨ Features

- 🏪 Laden-Ausbau (Kiosk → Shop → Filiale)
- 🎫 Verschiedene Rubbellos-Typen (1€ bis 10€, Sonderlotterien)
- 👤 Kunden-KI mit verschiedenen Persönlichkeiten
- 💰 Realistische Wirtschaftsmechanik (Provision, Einkauf, Lager)
- 👥 Mitarbeiter einstellen & verwalten
- 📦 Lager- & Nachbestell-System
- 🎉 Saisonale Events (Weihnachten, Jackpot-Wochen)
- 📊 Tages- & Wochenabrechnungen

---

## 🏗️ Projektstruktur

```
scratch-ticket-shop-simulator/
├── Assets/
│   ├── Scripts/          # Alle C# Spielskripte
│   │   ├── Core/         # Spiellogik (GameManager, etc.)
│   │   ├── Customer/     # Kunden-KI
│   │   ├── Shop/         # Laden & Kassensystem
│   │   ├── Tickets/      # Rubbellos-Mechanik
│   │   ├── UI/           # HUD, Menüs, Dialoge
│   │   ├── Economy/      # Wirtschaft, Lager, Finanzen
│   │   └── Employees/    # Mitarbeiter-System
│   ├── Prefabs/          # Unity Prefabs
│   ├── Scenes/           # Unity Szenen
│   ├── Art/              # Sprites, Texturen, Animationen
│   │   ├── Sprites/
│   │   ├── UI/
│   │   └── Animations/
│   ├── Audio/            # Musik & Soundeffekte
│   └── Resources/        # Dynamisch geladene Assets
├── Docs/
│   └── GDD.md            # Game Design Document
├── ProjectSettings/      # Unity Projekteinstellungen
├── .gitignore
└── README.md
```

---

## 🚀 Getting Started

### Voraussetzungen
- [Unity 2022.3 LTS](https://unity.com/releases/lts) oder neuer
- Git

### Installation

```bash
# Repository klonen
git clone https://github.com/XxFaCeS/scratch-ticket-shop-simulator.git

# Projekt in Unity Hub öffnen
# → Unity Hub → Add → Ordner auswählen
```

### Erster Start
1. Szene `Assets/Scenes/MainMenu.unity` öffnen
2. Play drücken ▶️

---

## 🗺️ Roadmap

| Phase | Status | Inhalt |
|-------|--------|--------|
| Phase 1 – MVP | 🔧 In Entwicklung | Grundgameplay, Kasse, Kunden, Lose |
| Phase 2 – Ausbau | 📋 Geplant | Laden-Upgrades, Mitarbeiter, Lager |
| Phase 3 – Tiefe | 📋 Geplant | Events, Lizenzen, Filialen |

---

## 🛠️ Tech Stack

- **Engine:** Unity 2022.3 LTS
- **Sprache:** C#
- **Versionskontrolle:** Git + GitHub

---

## 📄 Dokumentation

- [Game Design Document](Docs/GDD.md)

---

## 👤 Autor

**XxFaCeS** – [@XxFaCeS](https://github.com/XxFaCeS)
