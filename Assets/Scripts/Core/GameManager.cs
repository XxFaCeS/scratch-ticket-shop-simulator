using UnityEngine;
using UnityEngine.Events;

namespace ScratchTicketSim.Core
{
    /// <summary>
    /// Zentraler Spielcontroller – verwaltet Tagesablauf, Spielzustand und Events.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Tag-Einstellungen")]
        [SerializeField] private float dayDurationSeconds = 180f; // 3 Minuten = 1 Spieltag
        private float _dayTimer;
        private int _currentDay = 1;
        private bool _dayRunning = false;

        [Header("Shop-Stufe")]
        public int ShopLevel = 1; // 1=Kiosk, 2=Kleiner Laden, 3=Lotto-Shop, 4=Lotto-Center

        // Events
        public UnityEvent OnDayStart = new UnityEvent();
        public UnityEvent OnDayEnd   = new UnityEvent();
        public UnityEvent<int> OnDayChanged = new UnityEvent<int>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start() => StartDay();

        private void Update()
        {
            if (!_dayRunning) return;
            _dayTimer -= Time.deltaTime;
            if (_dayTimer <= 0f) EndDay();
        }

        public void StartDay()
        {
            _dayTimer   = dayDurationSeconds;
            _dayRunning = true;
            Debug.Log($"[GameManager] Tag {_currentDay} beginnt.");
            OnDayStart?.Invoke();
        }

        private void EndDay()
        {
            _dayRunning = false;
            Debug.Log($"[GameManager] Tag {_currentDay} endet.");
            OnDayEnd?.Invoke();
            _currentDay++;
            OnDayChanged?.Invoke(_currentDay);
        }

        /// <summary>Gibt die verbleibende Zeit des aktuellen Tages in Sekunden zurück.</summary>
        public float GetRemainingDayTime() => Mathf.Max(_dayTimer, 0f);

        /// <summary>Gibt den aktuellen Spieltag zurück.</summary>
        public int GetCurrentDay() => _currentDay;

        /// <summary>Startet den nächsten Tag manuell (z.B. nach Tagesabrechnung).</summary>
        public void ProceedToNextDay() => StartDay();
    }
}
