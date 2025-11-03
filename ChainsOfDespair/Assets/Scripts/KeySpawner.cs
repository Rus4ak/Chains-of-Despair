using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KeySpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _keyPrefab;

    public List<Door> lockedDoors = new List<Door>();
    public List<SpawnKeyPos> spawnPositions = new List<SpawnKeyPos>();
    public bool isSpawned = false;

    public static KeySpawner Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }

    private void Start()
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
        }
    }
}
