using System.Collections;
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

        [Header("Counter Position")]
        [SerializeField] private Transform counterPoint;

        private bool _isBusy = false;
        private CustomerAI _currentCustomer = null;

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

            if (!Tickets.TicketManager.Instance.TakeTicket(ticket))
            {
                customer.Leave();
                return;
            }

            Economy.EconomyManager.Instance.AddRevenue(ticket.Commission);
            Economy.EconomyManager.Instance.RegisterTicketSold();

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
            _currentCustomer = next;

            if (counterPoint != null)
                next.WalkToCounter(counterPoint.position);
            else
                next.BeServed();

            Debug.Log($"[CashRegister] Bediene Kunden: {next.Type}");
        }

        /// <summary>Wird aufgerufen wenn Kunde am Counter angekommen ist.</summary>
        public void OnCustomerArrived(CustomerAI customer)
        {
            if (customer != _currentCustomer) return;
            customer.BeServed();
            StartCoroutine(FinishServiceAfterDelay());
        }

        // ── Interne Methoden ─────────────────────────────────────────────

        private IEnumerator FinishServiceAfterDelay()
        {
            yield return new WaitForSeconds(serviceTimeSeconds);
            _isBusy = false;
            _currentCustomer = null;

            CustomerAI next = CustomerSpawner.Instance?.GetNextCustomer();
            if (next != null)
                ServeNextCustomer();
        }
    }
}
