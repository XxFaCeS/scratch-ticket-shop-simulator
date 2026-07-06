using System.Collections;
using UnityEngine;

namespace ScratchTicketSim.Customer
{
    public enum CustomerType  { Regular, Gambler, Casual, VIP, Impatient }
    public enum CustomerState { Entering, WalkingToCounter, WaitingInQueue, BeingServed, Leaving }

    /// <summary>
    /// Kunden-KI für 3D Perspective: Bewegung, Losauswahl, Farbe, Reaktion.
    /// </summary>
    public class CustomerAI : MonoBehaviour
    {
        [Header("Kunden-Typ")]
        public CustomerType Type = CustomerType.Regular;

        [Header("Geduld")]
        [SerializeField] private float maxWaitTime = 30f;
        private float _waitTimer;

        [Header("Bewegung")]
        [SerializeField] private float moveSpeed = 3f;
        private Vector3 _targetPosition;
        private bool _hasTarget = false;

        public CustomerState State { get; private set; } = CustomerState.Entering;

        private ScratchTicketSim.Tickets.ScratchTicket _chosenTicket;
        private int _ticketsToBuy;
        private Renderer _renderer;

        // ── Initialisierung ──────────────────────────────────────────────

        private void Awake()
        {
            _renderer = GetComponentInChildren<Renderer>();
            _targetPosition = transform.position;
        }

        public void Initialize(CustomerType type)
        {
            Type          = type;
            _ticketsToBuy = GetTicketCount();
            maxWaitTime   = GetPatience();
            SetColor();
        }

        private void SetColor()
        {
            if (_renderer == null) return;
            _renderer.material = new Material(_renderer.material);
            _renderer.material.color = Type switch
            {
                CustomerType.Regular   => new Color(0.2f, 0.5f, 1.0f),
                CustomerType.Casual    => new Color(0.2f, 0.8f, 0.2f),
                CustomerType.Gambler   => new Color(1.0f, 0.2f, 0.2f),
                CustomerType.Impatient => new Color(1.0f, 0.9f, 0.0f),
                CustomerType.VIP       => new Color(0.7f, 0.2f, 1.0f),
                _                      => Color.white
            };
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
            _targetPosition = new Vector3(queuePosition.x, transform.position.y, queuePosition.z);
            _hasTarget = true;
            State      = CustomerState.WaitingInQueue;
            _waitTimer = maxWaitTime;
        }

        /// <summary>Schickt den Kunden zum Counter.</summary>
        public void WalkToCounter(Vector3 counterPosition)
        {
            _targetPosition = new Vector3(counterPosition.x, transform.position.y, counterPosition.z);
            _hasTarget = true;
            State      = CustomerState.WalkingToCounter;
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

        // ── Bewegung (3D) ───────────────────────────────────────────────

        private void MoveToTarget()
        {
            if (!_hasTarget) return;

            Vector3 flatTarget = new Vector3(_targetPosition.x, transform.position.y, _targetPosition.z);
            float dist = Vector3.Distance(transform.position, flatTarget);

            if (dist > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, flatTarget, moveSpeed * Time.deltaTime);
                Vector3 dir = (flatTarget - transform.position).normalized;
                if (dir != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(dir);
            }
            else
            {
                // Ziel erreicht
                _hasTarget = false;

                if (State == CustomerState.WalkingToCounter)
                {
                    // Am Counter angekommen → CashRegister informieren
                    CashRegister.Instance?.OnCustomerArrived(this);
                }
            }
        }

        // ── Verlassen ────────────────────────────────────────────────────

        public void Leave()
        {
            State = CustomerState.Leaving;
            _targetPosition = transform.position + Vector3.back * 8f;
            _hasTarget = true;
            CustomerSpawner.Instance?.RemoveFromQueue(this);
            Destroy(gameObject, 2f);
        }

        private void LeaveImpatient()
        {
            Debug.Log($"[CustomerAI] {Type} ist ungeduldig und geht!");
            Leave();
        }

        // ── Hilfsmethoden ────────────────────────────────────────────────

        private int GetTicketCount() => Type switch
        {
            CustomerType.Gambler  => UnityEngine.Random.Range(3, 6),
            CustomerType.Casual   => UnityEngine.Random.Range(1, 3),
            CustomerType.VIP      => UnityEngine.Random.Range(2, 4),
            _                     => 1
        };

        private float GetPatience() => Type switch
        {
            CustomerType.Impatient => 10f,
            CustomerType.Gambler   => 60f,
            CustomerType.VIP       => 90f,
            CustomerType.Casual    => 45f,
            _                      => 30f
        };
    }
}
