using System.Collections.Generic;
using UnityEngine;

namespace ScratchTicketSim.Tickets
{
    /// <summary>
    /// Verwaltet alle verfügbaren Lostypen und den Lagerbestand.
    /// </summary>
    public class TicketManager : MonoBehaviour
    {
        public static TicketManager Instance { get; private set; }

        [Header("Verfügbare Lostypen")]
        [SerializeField] private List<TicketStock> stock = new List<TicketStock>();

        [Header("Lager-Einstellungen")]
        [SerializeField] private int lowStockThreshold = 10;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        // ── Öffentliche API ──────────────────────────────────────────────

        /// <summary>Gibt alle verfügbaren (auf Lager) Lostypen zurück.</summary>
        public List<ScratchTicket> GetAvailableTickets()
        {
            var available = new List<ScratchTicket>();
            foreach (var s in stock)
                if (s.Quantity > 0) available.Add(s.Ticket);
            return available;
        }

        /// <summary>Gibt den Lagerbestand eines bestimmten Lostyps zurück.</summary>
        public int GetStock(ScratchTicket ticket)
        {
            var entry = stock.Find(s => s.Ticket == ticket);
            return entry?.Quantity ?? 0;
        }

        /// <summary>
        /// Nimmt ein Los aus dem Lager (beim Verkauf).
        /// Gibt false zurück wenn nicht auf Lager.
        /// </summary>
        public bool TakeTicket(ScratchTicket ticket)
        {
            var entry = stock.Find(s => s.Ticket == ticket);
            if (entry == null || entry.Quantity <= 0)
            {
                Debug.LogWarning($"[TicketManager] {ticket.TicketName} nicht auf Lager!");
                return false;
            }
            entry.Quantity--;
            CheckLowStock(entry);
            return true;
        }

        /// <summary>Bestellt neue Lose nach (kostet Geld).</summary>
        public bool RestockTicket(ScratchTicket ticket, int amount)
        {
            float cost = ticket.Price * 0.95f * amount; // 95% des Verkaufspreises
            if (!EconomyManager.Instance.Spend(cost))
                return false;

            var entry = stock.Find(s => s.Ticket == ticket);
            if (entry != null)
                entry.Quantity += amount;
            else
                stock.Add(new TicketStock { Ticket = ticket, Quantity = amount });

            Debug.Log($"[TicketManager] {amount}x {ticket.TicketName} nachbestellt. Kosten: {cost:F2}€");
            return true;
        }

        // ── Interne Methoden ─────────────────────────────────────────────

        private void CheckLowStock(TicketStock entry)
        {
            if (entry.Quantity <= lowStockThreshold)
                Debug.LogWarning($"[TicketManager] Niedriger Bestand: {entry.Ticket.TicketName} ({entry.Quantity} verbleibend)");
        }
    }

    /// <summary>Lagerbestand-Eintrag für einen Lostyp.</summary>
    [System.Serializable]
    public class TicketStock
    {
        public ScratchTicket Ticket;
        public int Quantity = 50;
    }
}
