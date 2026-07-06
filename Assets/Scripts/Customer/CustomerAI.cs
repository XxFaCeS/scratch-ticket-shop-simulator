using System.Collections;
using UnityEngine;

namespace ScratchTicketSim.Customer
{
    public enum CustomerType { Regular, Gambler, Casual, VIP, Impatient }
    public enum CustomerState { Entering, WaitingInQueue, BeingServed, Leaving }

    /// <summary>
    /// Kunden-KI: Bewegung, Losauswahl, Reaktion auf Gewinn/Verlust.
    /// </summary>
    public class CustomerAI : MonoBehaviour
    {
        [Header("Kunden-Typ")]
        public CustomerType Type = CustomerType.Regular;

        [Header("Geduld")]
        [SerializeField] private float maxWaitTime = 30f;
        private float _waitTimer;

        [Header("Bewegung")]
        [SerializeField] private float moveSpeed = 2f;
        private Vector3 _targetPosition;

        public CustomerState State { get; private set; } = CustomerState.Entering;

        private ScratchTicketSim.Tickets.ScratchTicket _chosenTicket;
        private int _ticketsToBuy;

        // ── Initialisierung ──────────────────────────────────────────────

        public void Initialize(CustomerType type)
        {
            Type = type;
            _ticketsToBuy = GetTicketCount();
            maxWaitTime   = GetPatience();
        }

        private void Update()
        {
            MoveToTarget();

            if (State == CustomerState.WaitingInQueue)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0f) LeaveImpatient();
            }
        }

        // ── Warteschlange ────────────────────────────────────────────────

        public void JoinQueue(Vector3 queuePosition)
        {
            _targetPosition = queuePosition;
            State           = CustomerState.WaitingInQueue;
            _waitTimer      = maxWaitTime;
        }

        public void BeServed()
        {
            State = CustomerState.BeingServed;
            ChooseTicket();
        }

        // ── Losauswahl ───────────────────────────────────────────────────

        private void ChooseTicket()
        {
            var available = Tickets.TicketManager.Instance.GetAvailableTickets();
            if (available.Count == 0) { Leave(); return; }

            // VIP bevorzugt teure Lose, Casual nimmt das günstigste
            if (Type == CustomerType.VIP)
                available.Sort((a, b) => b.Price.CompareTo(a.Price));
            else
                available.Sort((a, b) => a.Price.CompareTo(b.Price));

            _chosenTicket = available[0];
            StartCoroutine(BuyTickets());
        }

        private IEnumerator BuyTickets()
        {
            for (int i = 0; i < _ticketsToBuy; i++)
            {
                yield return new WaitForSeconds(0.5f);
                CashRegister.Instance?.SellTicket(_chosenTicket, this);
            }
            yield return new WaitForSeconds(1f);
            Leave();
        }

        // ── Reaktionen ───────────────────────────────────────────────────

        public void ReactToResult(float prize)
        {
            if (prize <= 0f)
                Debug.Log($"[CustomerAI] {Type} ist enttäuscht – kein Gewinn.");
            else if (prize >= 100f)
                Debug.Log($"[CustomerAI] {Type} jubelt! Gewinn: {prize:F2}€ 🎉");
            else
                Debug.Log($"[CustomerAI] {Type} freut sich – Gewinn: {prize:F2}€");
        }

        // ── Bewegung & Verlassen ─────────────────────────────────────────

        private void MoveToTarget()
        {
            transform.position = Vector3.MoveTowards(
                transform.position, _targetPosition, moveSpeed * Time.deltaTime);
        }

        private void LeaveImpatient()
        {
            Debug.Log($"[CustomerAI] {Type} verlässt den Laden – zu lange gewartet!");
            Leave();
        }

        public void Leave()
        {
            State = CustomerState.Leaving;
            _targetPosition = transform.position + Vector3.right * 10f;
            Destroy(gameObject, 3f);
        }

        // ── Hilfsmethoden ────────────────────────────────────────────────

        private int GetTicketCount() => Type switch
        {
            CustomerType.Gambler  => Random.Range(5, 11),
            CustomerType.VIP      => Random.Range(1, 4),
            CustomerType.Casual   => 1,
            _                     => Random.Range(1, 3)
        };

        private float GetPatience() => Type switch
        {
            CustomerType.Impatient => Random.Range(5f,  15f),
            CustomerType.Regular   => Random.Range(20f, 40f),
            CustomerType.VIP       => Random.Range(30f, 60f),
            _                      => Random.Range(15f, 30f)
        };
    }
}
