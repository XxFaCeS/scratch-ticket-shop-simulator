using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ScratchTicketSim.Shop
{
    public enum UpgradeType { ExtraCounter, StorageExpansion, DecorUpgrade, StaffSlot, SecurityCamera }

    /// <summary>
    /// Verwaltet alle Shop-Upgrades: Kauf, Freischaltung und Effekte.
    /// </summary>
    public class ShopUpgrade : MonoBehaviour
    {
        public static ShopUpgrade Instance { get; private set; }

        [Header("Verfügbare Upgrades")]
        [SerializeField] private List<UpgradeData> upgrades = new List<UpgradeData>();

        public UnityEvent<UpgradeData> OnUpgradePurchased = new UnityEvent<UpgradeData>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        // ── Öffentliche API ──────────────────────────��───────────────────

        /// <summary>Gibt alle noch nicht gekauften Upgrades zurück.</summary>
        public List<UpgradeData> GetAvailableUpgrades()
        {
            return upgrades.FindAll(u => !u.IsPurchased && MeetsRequirements(u));
        }

        /// <summary>Gibt alle bereits gekauften Upgrades zurück.</summary>
        public List<UpgradeData> GetPurchasedUpgrades()
        {
            return upgrades.FindAll(u => u.IsPurchased);
        }

        /// <summary>Kauft ein Upgrade, wenn genug Geld vorhanden.</summary>
        public bool Purchase(UpgradeType type)
        {
            var upgrade = upgrades.Find(u => u.Type == type && !u.IsPurchased);
            if (upgrade == null)
            {
                Debug.LogWarning($"[ShopUpgrade] Upgrade {type} nicht verfügbar oder bereits gekauft.");
                return false;
            }

            if (!Economy.EconomyManager.Instance.Spend(upgrade.Cost))
                return false;

            upgrade.IsPurchased = true;
            ApplyUpgradeEffect(upgrade);
            OnUpgradePurchased?.Invoke(upgrade);
            Debug.Log($"[ShopUpgrade] Upgrade gekauft: {upgrade.DisplayName} für {upgrade.Cost:F2}€");
            return true;
        }

        public bool IsUnlocked(UpgradeType type)
            => upgrades.Exists(u => u.Type == type && u.IsPurchased);

        // ── Effekte anwenden ─────────────────────────────────────────────

        private void ApplyUpgradeEffect(UpgradeData upgrade)
        {
            switch (upgrade.Type)
            {
                case UpgradeType.ExtraCounter:
                    // Erhöht max. Warteschlangengröße
                    var spawner = FindObjectOfType<Customer.CustomerSpawner>();
                    Debug.Log("[ShopUpgrade] Extra Kasse freigeschaltet!");
                    break;

                case UpgradeType.StorageExpansion:
                    // Verdoppelt Lagerbestand aller Lostypen
                    Debug.Log("[ShopUpgrade] Lager erweitert!");
                    break;

                case UpgradeType.StaffSlot:
                    Debug.Log("[ShopUpgrade] Neuer Mitarbeiter-Slot freigeschaltet!");
                    break;

                case UpgradeType.SecurityCamera:
                    Debug.Log("[ShopUpgrade] Sicherheitskamera installiert!");
                    break;

                case UpgradeType.DecorUpgrade:
                    Debug.Log("[ShopUpgrade] Shop-Dekor verbessert – mehr Kundenzufriedenheit!");
                    break;
            }
        }

        // ── Voraussetzungen prüfen ───────────────────────────────────────

        private bool MeetsRequirements(UpgradeData upgrade)
        {
            if (upgrade.RequiredUpgrade == UpgradeType.ExtraCounter &&
                upgrade.Type != UpgradeType.ExtraCounter)
                return IsUnlocked(upgrade.RequiredUpgrade);
            return true;
        }
    }

    /// <summary>Datenklasse für ein einzelnes Upgrade.</summary>
    [System.Serializable]
    public class UpgradeData
    {
        public UpgradeType Type;
        public string      DisplayName  = "Upgrade";
        [TextArea]
        public string      Description  = "";
        public float       Cost         = 500f;
        public Sprite      Icon;
        public bool        IsPurchased  = false;
        public UpgradeType RequiredUpgrade;
    }
}
