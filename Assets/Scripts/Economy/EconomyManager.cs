using UnityEngine;
using UnityEngine.Events;

namespace ScratchTicketSim.Core
{
    /// <summary>
    /// Verwaltet alle Finanzen: Kontostand, Provisionen, Ausgaben und Tagesabrechnung.
    /// </summary>
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }

        [Header("Startkapital")]
        [SerializeField] private float startingMoney = 100f;

        private float _balance;
        private float _dailyRevenue;
        private float _dailyExpenses;
        private int   _dailyTicketsSold;

        public UnityEvent<float> OnBalanceChanged = new UnityEvent<float>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            _balance = startingMoney;
        }

        private void OnEnable()
        {
            GameManager.Instance?.OnDayStart.AddListener(ResetDailyStats);
            GameManager.Instance?.OnDayEnd.AddListener(PrintDailyReport);
        }

        private void OnDisable()
        {
            GameManager.Instance?.OnDayStart.RemoveListener(ResetDailyStats);
            GameManager.Instance?.OnDayEnd.RemoveListener(PrintDailyReport);
        }

        // ── Öffentliche API ──────────────────────────────────────────────

        public float Balance => _balance;

        /// <summary>Fügt eine Einnahme hinzu (z.B. Provision).</summary>
        public void AddRevenue(float amount)
        {
            _balance       += amount;
            _dailyRevenue  += amount;
            OnBalanceChanged?.Invoke(_balance);
        }

        /// <summary>Zieht eine Ausgabe ab (z.B. Loseinkauf, Gehalt).</summary>
        public bool Spend(float amount)
        {
            if (_balance < amount)
            {
                Debug.LogWarning("[EconomyManager] Nicht genug Geld!");
                return false;
            }
            _balance        -= amount;
            _dailyExpenses  += amount;
            OnBalanceChanged?.Invoke(_balance);
            return true;
        }

        public void RegisterTicketSold() => _dailyTicketsSold++;

        // ── Interne Methoden ─────────────────────────────────────────────

        private void ResetDailyStats()
        {
            _dailyRevenue      = 0f;
            _dailyExpenses     = 0f;
            _dailyTicketsSold  = 0;
        }

        private void PrintDailyReport()
        {
            float profit = _dailyRevenue - _dailyExpenses;
            Debug.Log($"[Tagesabrechnung] Einnahmen: {_dailyRevenue:F2}€ | " +
                      $"Ausgaben: {_dailyExpenses:F2}€ | Gewinn: {profit:F2}€ | " +
                      $"Lose verkauft: {_dailyTicketsSold} | Kontostand: {_balance:F2}€");
        }

        public (float revenue, float expenses, int ticketsSold) GetDailyStats()
            => (_dailyRevenue, _dailyExpenses, _dailyTicketsSold);
    }
}
