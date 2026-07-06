using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScratchTicketSim.Core;

namespace ScratchTicketSim.Customer
{
    /// <summary>
    /// Spawnt Kunden in regelmäßigen Abständen und verwaltet die Warteschlange.
    /// </summary>
    public class CustomerSpawner : MonoBehaviour
    {
        public static CustomerSpawner Instance { get; private set; }

        [Header("Spawn-Einstellungen")]
        [SerializeField] private GameObject customerPrefab;
        [SerializeField] private Transform  spawnPoint;
        [SerializeField] private float      spawnIntervalMin = 5f;
        [SerializeField] private float      spawnIntervalMax = 15f;

        [Header("Warteschlange")]
        [SerializeField] private Transform   queueStartPoint;
        [SerializeField] private float       queueSpacing = 1.2f;
        [SerializeField] private int         maxQueueSize = 5;

        private List<CustomerAI> _queue = new List<CustomerAI>();
        private bool _spawning = false;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable()
        {
            Core.GameManager.Instance?.OnDayStart.AddListener(StartSpawning);
            Core.GameManager.Instance?.OnDayEnd.AddListener(StopSpawning);
        }

        private void OnDisable()
        {
            Core.GameManager.Instance?.OnDayStart.RemoveListener(StartSpawning);
            Core.GameManager.Instance?.OnDayEnd.RemoveListener(StopSpawning);
        }

        public void StartSpawning()
        {
            _spawning = true;
            StartCoroutine(SpawnLoop());
        }

        public void StopSpawning()
        {
            _spawning = false;
            StopAllCoroutines();
        }

        private IEnumerator SpawnLoop()
        {
            while (_spawning)
            {
                float interval = Random.Range(spawnIntervalMin, spawnIntervalMax);
                yield return new WaitForSeconds(interval);

                if (_queue.Count < maxQueueSize)
                    SpawnCustomer();
            }
        }

        private void SpawnCustomer()
        {
            if (customerPrefab == null || spawnPoint == null) return;

            GameObject go = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
            CustomerAI ai = go.GetComponent<CustomerAI>();
            if (ai == null) return;

            CustomerType type = GetRandomType();
            ai.Initialize(type);
            AddToQueue(ai);
        }

        public void AddToQueue(CustomerAI customer)
        {
            _queue.Add(customer);
            RefreshQueuePositions();
            if (_queue.Count == 1)
                CashRegister.Instance?.ServeNextCustomer();
        }

        public void RemoveFromQueue(CustomerAI customer)
        {
            _queue.Remove(customer);
            RefreshQueuePositions();
            if (_queue.Count > 0)
                CashRegister.Instance?.ServeNextCustomer();
        }

        public CustomerAI GetNextCustomer()
            => _queue.Count > 0 ? _queue[0] : null;

        private void RefreshQueuePositions()
        {
            for (int i = 0; i < _queue.Count; i++)
            {
                if (_queue[i] == null) continue;
                Vector3 pos = queueStartPoint.position + Vector3.right * (i * queueSpacing);
                _queue[i].JoinQueue(pos);
            }
        }

        private CustomerType GetRandomType()
        {
            float roll = Random.value;
            if (roll < 0.40f) return CustomerType.Regular;
            if (roll < 0.65f) return CustomerType.Casual;
            if (roll < 0.85f) return CustomerType.Gambler;
            if (roll < 0.95f) return CustomerType.Impatient;
            return CustomerType.VIP;
        }
    }
}
