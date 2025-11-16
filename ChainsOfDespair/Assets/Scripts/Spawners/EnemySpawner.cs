using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private BoxCollider[] _spawnZones;
    [SerializeField] private GameObject[] _enemies;

    public static EnemySpawner Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;
    }

    public void Spawn()
    {
        if (!IsServer)
            return;
       
        foreach (BoxCollider spawnZone in _spawnZones)
        {
            GameObject enemy = _enemies[Random.Range(0, _enemies.Length)];

            Vector3 spawnZoneCenter = spawnZone.center + spawnZone.transform.position;

            Vector3 spawnPos = new Vector3(
                Random.Range(spawnZoneCenter.x - spawnZone.size.x / 2f, spawnZoneCenter.x + spawnZone.size.x / 2f),
                enemy.transform.position.y,
                Random.Range(spawnZoneCenter.z - spawnZone.size.z / 2f, spawnZoneCenter.z + spawnZone.size.z / 2f)
                );

            GameObject spawnedEnemy = Instantiate(enemy, spawnPos, Quaternion.identity);
            spawnedEnemy.GetComponent<NetworkObject>().Spawn();
        }
    }
}
