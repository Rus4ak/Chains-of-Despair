using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _keyPrefab;
    [SerializeField] private GameObject _batteryPrefab;

    [HideInInspector] public List<Door> lockedDoors = new List<Door>();
    [HideInInspector] public List<SpawnKeyPos> spawnPositions = new List<SpawnKeyPos>();
    [HideInInspector] public bool isSpawned = false;

    public static ItemSpawner Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }

    public void SpawnItems()
    {
        if (IsServer)
        {
            foreach (Door door in lockedDoors)
            {
                SpawnKeyPos randomSpawnKeyPos;
                while (true)
                {
                    randomSpawnKeyPos = spawnPositions[Random.Range(0, spawnPositions.Count)];

                    bool isCanSpawn = true;

                    foreach (string keyCantSpawn in randomSpawnKeyPos.KeysCantSpawn)
                    {
                        if (keyCantSpawn == door.KeyName)
                            isCanSpawn = false;
                    }

                    if (isCanSpawn)
                        break;
                }
                
                Vector3 randomPos = randomSpawnKeyPos.transform.position;
                GameObject key = Instantiate(_keyPrefab, randomPos, _keyPrefab.transform.rotation);
                key.GetComponent<NetworkObject>().Spawn();

                key.name = door.KeyName;
            }

            for (int i = 0; i < 3; i++)
            {
                Vector3 randomPos = spawnPositions[Random.Range(0, spawnPositions.Count)].transform.position;
                GameObject battery = Instantiate(_batteryPrefab, randomPos, _batteryPrefab.transform.rotation);
                battery.GetComponent<NetworkObject>().Spawn();
            }

            isSpawned = true;
        }
    }
}
