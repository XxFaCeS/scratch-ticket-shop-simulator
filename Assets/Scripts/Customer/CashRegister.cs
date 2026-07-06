using UnityEngine;

namespace ScratchTicketSim.Customer
{
    /// <summary>
    /// Kassensystem: Verkauft Lose an Kunden, verarbeitet Zahlung und Provision.
    /// </summary>
    public class CashRegister : MonoBehaviour
    {
        public static CashRegister Instance { get; private set; }

        [Header("Kassen-Einstellungen")]
        [SerializeField] private float serviceTimeSeconds = 2f;
        private bool _isBusy = false;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        // ── Öffentliche API ──────────────────────────────────────────────

        /// <summary>Verkauft ein Los an einen Kunden.</summary>
        public void SellTicket(Tickets.ScratchTicket ticket, CustomerAI customer)
        {
            if (ticket == null || customer == null) return;

            // Los aus Lager nehmen
            if (!Tickets.TicketManager.Instance.TakeTicket(ticket))
            {
                customer.Leave();
                return;
            }

            // Provision gutschreiben
            Economy.EconomyManager.Instance.AddRevenue(ticket.Commission);
            Economy.EconomyManager.Instance.RegisterTicketSold();

            // Gewinn berechnen & Kundenreaktion
            float prize = ticket.Roll();
            customer.ReactToResult(prize);

            Debug.Log($"[CashRegister] Verkauft: {ticket.TicketName} | " +
                      $"Provision: {ticket.Commission:F2}€ | Gewinn: {prize:F2}€");
        }

        /// <summary>Bedient den nächsten Kunden in der Warteschlange.</summary>
        public void ServeNextCustomer()
        {
            if (_isBusy) return;

            CustomerAI next = CustomerSpawner.Instance?.GetNextCustomer();
            if (next == null) return;

            _isBusy = true;
            next.BeServed();
            Invoke(nameof(FinishService), serviceTimeSeconds);
        }

        // ── Interne Methoden ─────────────────────────────────────────────

        private void FinishService()
        {
            _isBusy = false;

            // Aktuellen Kunden aus der Schlange entfernen
            CustomerAI served = CustomerSpawner.Instance?.GetNextCustomer();
            if (served != null)
                CustomerSpawner.Instance.RemoveFromQueue(served);
        }
    }
}
