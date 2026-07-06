using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScratchTicketSim.Core;

namespace ScratchTicketSim.Customer
{
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

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnDayStart.AddListener(StartSpawning);
                GameManager.Instance.OnDayEnd.AddListener(StopSpawning);
                StartSpawning();
            }
        }

        private void OnDisable()
        {
            GameManager.Instance?.OnDayStart.RemoveListener(StartSpawning);
            GameManager.Instance?.OnDayEnd.RemoveListener(StopSpawning);
        }

        public void StartSpawning()
        {
            if (_spawning) return;
            _spawning = true;
            StartCoroutine(SpawnLoop());
            Debug.Log("[CustomerSpawner] Spawning gestartet!");
        }

        public void StopSpawning()
        {
            _spawning = false;
            StopAllCoroutines();
        }

        private IEnumerator SpawnLoop()
        {
            yield return new WaitForSeconds(2f);

            while (_spawning)
            {
                if (_queue.Count < maxQueueSize)
                    SpawnCustomer();

                float interval = Random.Range(spawnIntervalMin, spawnIntervalMax);
                yield return new WaitForSeconds(interval);
            }
        }

        private void SpawnCustomer()
        {
            if (customerPrefab == null || spawnPoint == null)
            {
                Debug.LogWarning("[CustomerSpawner] customerPrefab oder spawnPoint nicht zugewiesen!");
                return;
            }

            GameObject go = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
            CustomerAI ai = go.GetComponent<CustomerAI>();
            if (ai == null) return;

            CustomerType type = GetRandomType();
            ai.Initialize(type);
            AddToQueue(ai);
            Debug.Log($"[CustomerSpawner] Kunde gespawnt: {type}");
        }

        public void AddToQueue(CustomerAI customer)
        {
            _queue.Add(customer);
            // Erster Kunde → JoinQueue (setzt State), Rest → nur Position
            int index = _queue.Count - 1;
            Vector3 pos = queueStartPoint.position + Vector3.back * (index * queueSpacing);
            customer.JoinQueue(pos);

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
                Vector3 pos = queueStartPoint.position + Vector3.back * (i * queueSpacing);
                // UpdateQueuePosition ändert den State NICHT
                _queue[i].UpdateQueuePosition(pos);
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
