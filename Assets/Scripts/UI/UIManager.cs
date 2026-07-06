using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ScratchTicketSim.UI
{
    /// <summary>
    /// Zentrales UI-System: Geld, Tageszeit, Nachrichten und Upgrade-Panel.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD")]
        [SerializeField] private TextMeshProUGUI balanceText;
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI ticketsSoldText;

        [Header("Nachrichten-Banner")]
        [SerializeField] private GameObject      messageBanner;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private float           messageDuration = 3f;

        [Header("Upgrade-Panel")]
        [SerializeField] private GameObject      upgradePanel;
        [SerializeField] private Transform       upgradeListParent;
        [SerializeField] private GameObject      upgradeButtonPrefab;

        [Header("Tagesabschluss-Panel")]
        [SerializeField] private GameObject      daySummaryPanel;
        [SerializeField] private TextMeshProUGUI summaryRevenueText;
        [SerializeField] private TextMeshProUGUI summaryTicketsText;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            if (messageBanner != null) messageBanner.SetActive(false);
            if (upgradePanel  != null) upgradePanel.SetActive(false);
            if (daySummaryPanel != null) daySummaryPanel.SetActive(false);
        }

        // ── HUD-Updates ──────────────────────────────────────────────────

        public void UpdateBalance(float balance)
        {
            if (balanceText != null)
                balanceText.text = $"💰 {balance:F2} €";
        }

        public void UpdateDay(int day)
        {
            if (dayText != null)
                dayText.text = $"📅 Tag {day}";
        }

        public void UpdateTime(float normalizedTime)
        {
            if (timeText == null) return;
            int totalMinutes = Mathf.FloorToInt(normalizedTime * 480); // 8h Spieltag
            int hours   = 8 + totalMinutes / 60;
            int minutes = totalMinutes % 60;
            timeText.text = $"🕐 {hours:D2}:{minutes:D2}";
        }

        public void UpdateTicketsSold(int count)
        {
            if (ticketsSoldText != null)
                ticketsSoldText.text = $"🎟️ {count} verkauft";
        }

        // ── Nachrichten-Banner ───────────────────────────────────────────

        public void ShowMessage(string message)
        {
            if (messageBanner == null || messageText == null) return;
            messageText.text = message;
            messageBanner.SetActive(true);
            CancelInvoke(nameof(HideMessage));
            Invoke(nameof(HideMessage), messageDuration);
        }

        private void HideMessage()
        {
            if (messageBanner != null)
                messageBanner.SetActive(false);
        }

        // ── Upgrade-Panel ────────────────────────────────────────────────

        public void ToggleUpgradePanel()
        {
            if (upgradePanel == null) return;
            bool isActive = upgradePanel.activeSelf;
            upgradePanel.SetActive(!isActive);
            if (!isActive) RefreshUpgradeList();
        }

        private void RefreshUpgradeList()
        {
            if (upgradeListParent == null || upgradeButtonPrefab == null) return;

            foreach (Transform child in upgradeListParent)
                Destroy(child.gameObject);

            var available = Shop.ShopUpgrade.Instance?.GetAvailableUpgrades();
            if (available == null) return;

            foreach (var upgrade in available)
            {
                var go  = Instantiate(upgradeButtonPrefab, upgradeListParent);
                var btn = go.GetComponent<Button>();
                var lbl = go.GetComponentInChildren<TextMeshProUGUI>();

                if (lbl != null)
                    lbl.text = $"{upgrade.DisplayName}\n{upgrade.Cost:F0} €";

                if (btn != null)
                {
                    var capturedUpgrade = upgrade;
                    btn.onClick.AddListener(() =>
                    {
                        if (Shop.ShopUpgrade.Instance.Purchase(capturedUpgrade.Type))
                        {
                            ShowMessage($"✅ {capturedUpgrade.DisplayName} gekauft!");
                            RefreshUpgradeList();
                        }
                        else
                        {
                            ShowMessage("❌ Nicht genug Geld!");
                        }
                    });
                }
            }
        }

        // ── Tagesabschluss ───────────────────────────────────────────────

        public void ShowDaySummary(float revenue, int ticketsSold)
        {
            if (daySummaryPanel == null) return;
            daySummaryPanel.SetActive(true);

            if (summaryRevenueText != null)
                summaryRevenueText.text = $"Einnahmen: {revenue:F2} €";
            if (summaryTicketsText != null)
                summaryTicketsText.text = $"Verkaufte Lose: {ticketsSold}";
        }

        public void HideDaySummary()
        {
            if (daySummaryPanel != null)
                daySummaryPanel.SetActive(false);
        }
    }
}
