using UnityEngine;

namespace ScratchTicketSim.Tickets
{
    /// <summary>
    /// Datenmodell für einen Rubbellos-Typ.
    /// </summary>
    [CreateAssetMenu(fileName = "NewTicket", menuName = "ScratchSim/Scratch Ticket")]
    public class ScratchTicket : ScriptableObject
    {
        [Header("Allgemein")]
        public string TicketName     = "Glückslos";
        public Sprite TicketSprite;
        public float  Price          = 1f;   // Verkaufspreis
        public float  Commission     = 0.05f; // Provision pro Verkauf

        [Header("Gewinn-Konfiguration")]
        [Range(0f, 1f)]
        public float WinChance       = 0.30f; // 30% Gewinnchance

        public PrizeEntry[] Prizes;          // Mögliche Gewinne

        [Header("Saison")]
        public bool IsSeasonal       = false;
        public string SeasonTag      = ""; // z.B. "Christmas"

        // ── Gewinn-Logik ────────────────────────────────────────────────

        /// <summary>
        /// Berechnet einen Gewinn für dieses Los.
        /// Gibt 0 zurück wenn kein Gewinn.
        /// </summary>
        public float Roll()
        {
            if (Random.value > WinChance) return 0f;  // Kein Gewinn

            // Gewichtete Zufallsauswahl aus den Prize-Einträgen
            float total = 0f;
            foreach (var p in Prizes) total += p.Weight;

            float roll = Random.Range(0f, total);
            float cumulative = 0f;
            foreach (var p in Prizes)
            {
                cumulative += p.Weight;
                if (roll <= cumulative) return p.Amount;
            }
            return Prizes[Prizes.Length - 1].Amount;
        }
    }

    /// <summary>Einzelner Gewinn-Eintrag mit Betrag und Gewichtung.</summary>
    [System.Serializable]
    public class PrizeEntry
    {
        public string Label  = "Kleiner Gewinn";
        public float  Amount = 2f;
        [Tooltip("Höhere Gewichtung = häufiger")]
        public float  Weight = 10f;
    }
}
