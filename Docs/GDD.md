# 📄 Game Design Document (GDD)
## Scratch Ticket Shop Simulator

**Version:** 0.1  
**Autor:** XxFaCeS  
**Engine:** Unity 2022.3 LTS (C#)  
**Genre:** Shop-Simulator / Wirtschaftssimulation  
**Plattform:** PC (Windows/Mac)

---

## 1. 🎯 Vision & Konzept

Der Spieler startet als Besitzer eines kleinen Kiosks und verkauft Rubbellose an Kunden. Durch Einnahmen aus Provisionen baut er seinen Laden aus, kauft bessere Lose ein, stellt Mitarbeiter ein und expandiert zu einer Lotto-Filialkette. Das Spiel kombiniert das entspannte Gameplay von Shop-Simulatoren mit dem Spannungsmoment des Rubbellos-Mechanismus.

---

## 2. 🎮 Kern-Gameplay-Loop

```
[Tag beginnt]
      ↓
Kunden betreten den Laden
      ↓
Spieler bedient Kasse (Losauswahl → Bezahlung)
      ↓
Kunde rubbelt Los auf → Gewinn/Verlust-Reaktion
      ↓
Spieler sammelt Provision
      ↓
Lager prüfen → ggf. nachbestellen
      ↓
[Tagesende] Abrechnung → Geld investieren → Laden ausbauen
```

---

## 3. 🏪 Laden-Ausbau-System

| Stufe | Name | Kosten | Kunden/Tag | Kassen | Besonderheit |
|-------|------|--------|-----------|--------|--------------|
| 1 | Kiosk | Start | 5–10 | 1 | Basis-Lose |
| 2 | Kleiner Laden | 500€ | 15–25 | 2 | Mehr Lostypen |
| 3 | Lotto-Shop | 2.000€ | 30–50 | 3 | Sonderlotterien |
| 4 | Lotto-Center | 10.000€ | 60–100 | 5 | Mitarbeiter-Slots |

---

## 4. 🎫 Rubbellos-System

### Lostypen

| Name | Kaufpreis | Provision | Gewinnchance | Max. Gewinn |
|------|-----------|-----------|-------------|-------------|
| Glückslos | 1€ | 0,05€ | 30% | 10€ |
| Silberlos | 2€ | 0,10€ | 25% | 50€ |
| Goldlos | 5€ | 0,25€ | 20% | 200€ |
| Platinlos | 10€ | 0,50€ | 15% | 1.000€ |
| Weihnachtslos *(saisonal)* | 5€ | 0,30€ | 35% | 500€ |

> **Wichtig:** Der Spieler zahlt Gewinne **nicht selbst** aus. Die Lotterie-Gesellschaft übernimmt Gewinnauszahlungen. Der Spieler verdient ausschließlich die Provision pro verkauftem Los.

### Rubbel-Mechanik
- Jedes Los hat ein zufällig generiertes Ergebnis beim Kauf
- Animation: Kunde "rubbelt" das Los auf (Partikel-Effekt)
- Reaktion je nach Ergebnis: neutral / freudig / enttäuscht / Jackpot-Jubel

---

## 5. 👥 Kunden-KI

### Kundentypen

| Typ | Verhalten | Geduld | Kaufmenge |
|-----|-----------|--------|-----------|
| 🧓 Stammkunde | Kauft täglich, loyal | Hoch | 1–2 Lose |
| 🎲 Glücksspieler | Kauft viele Lose, euphorisch | Mittel | 5–10 Lose |
| 💸 Gelegenheitskäufer | Nur bei Sonderaktionen | Niedrig | 1 Los |
| 👑 VIP-Kunde | Kauft teure Lose, gibt Trinkgeld | Hoch | 1–3 Lose |
| 😠 Ungeduldiger Kunde | Verlässt Laden bei langer Wartezeit | Sehr niedrig | 1 Los |

### KI-Verhalten
- Kunden wählen Lostyp basierend auf ihrem Typ und Kontostand
- Warteschlangen-System: max. 5 Kunden gleichzeitig
- Kundenzufriedenheit beeinflusst Ruf und Stammkunden-Rate

---

## 6. 💰 Wirtschaftssystem

### Einnahmen
- Provision pro verkauftem Los (5% des Verkaufspreises)
- Trinkgelder von zufriedenen VIP-Kunden
- Boni für Tages-Verkaufsziele

### Ausgaben
- Lose einkaufen (Großhandel: 95% des Verkaufspreises)
- Laden-Upgrades
- Mitarbeitergehälter
- Lotterie-Lizenz (monatliche Gebühr)

### Finanzen-UI
- Tagesabrechnung (Einnahmen / Ausgaben / Gewinn)
- Wochenübersicht als Diagramm
- Lager-Warnanzeige bei niedrigem Bestand

---

## 7. 👷 Mitarbeiter-System

| Position | Kosten/Tag | Funktion |
|----------|-----------|----------|
| Kassierer | 50€ | Bedient 2. Kasse automatisch |
| Lagerhelfer | 30€ | Bestellt Lose automatisch nach |
| Manager | 100€ | Verbessert Kundenzufriedenheit |

---

## 8. 📦 Lager-System

- Jeder Lostyp hat einen eigenen Lagerbestand
- Automatische Warn-Meldung bei < 10 Losen
- Manuelle oder automatische Nachbestellung (mit Lagerhelfer)
- Lieferzeit: 1 Spieltag

---

## 9. 🎉 Events

| Event | Dauer | Effekt |
|-------|-------|--------|
| Jackpot-Woche | 7 Tage | Doppelte Kundenzahl, höhere Provision |
| Weihnachten | 14 Tage | Weihnachtslose verfügbar |
| Lieferanten-Angebot | 1 Tag | 20% Rabatt beim Einkauf |
| Behörden-Kontrolle | 1 Tag | Lizenz muss aktuell sein |

---

## 10. 🗺️ Szenen-Übersicht

| Szene | Beschreibung |
|-------|-------------|
| `MainMenu` | Hauptmenü, Spielstand laden/starten |
| `ShopScene` | Hauptspielszene (Laden + Kasse) |
| `UpgradeMenu` | Laden ausbauen, Upgrades kaufen |
| `StockMenu` | Lager verwalten, Lose bestellen |
| `EmployeeMenu` | Mitarbeiter einstellen/entlassen |
| `DailyReport` | Tagesabrechnung & Statistiken |

---

## 11. 🎵 Audio

- Hintergrundmusik: entspannte Kiosk-Musik
- Soundeffekte: Kasse, Rubbel-Animation, Jackpot-Sound, Türklingel
- Kundengeräusche: Jubeln, Seufzen, Gespräche

---

## 12. 🏆 Progression & Ziele

### Kurzzeit-Ziele (täglich)
- Verkaufe X Lose heute
- Erreiche X€ Tageseinnahmen
- Bediene X Kunden ohne Wartezeit

### Langzeit-Ziele (Achievements)
- "Erster Schritt": Ersten Laden-Upgrade kaufen
- "Jackpot-Dealer": 1.000 Lose verkauft
- "Tycoon": Lotto-Center-Stufe erreicht
- "Volksheld": 100 Stammkunden

---

*Letzte Aktualisierung: Phase 1 – MVP*
