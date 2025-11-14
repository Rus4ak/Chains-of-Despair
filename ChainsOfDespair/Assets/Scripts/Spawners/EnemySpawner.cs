using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private float _minSpawnCount;
    [SerializeField] private float _maxSpawnCount;
    [SerializeField] private Transform _minPos;
    [SerializeField] private Transform _maxPos;
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

        //float count = Random.Range(_minSpawnCount, _maxSpawnCount);

        GameObject enemy = Instantiate(_enemies[1], new Vector3(-18.8f, _enemies[1].transform.position.y, 0.45f), Quaternion.identity);
        enemy.GetComponent<NetworkObject>().Spawn();

        //for (int i = 0; i < count; i++)
        //{
        //    GameObject enemy = _enemies[Random.Range(0, _enemies.Length)];

        //    Vector3 spawnPos = new Vector3(
        //        Random.Range(_minPos.position.x, _maxPos.position.x),
        //        enemy.transform.position.y,
        //        Random.Range(_minPos.position.z, _maxPos.position.z)
        //        );

        //    GameObject spawnedEnemy = Instantiate(enemy, spawnPos, Quaternion.identity);
        //    spawnedEnemy.GetComponent<NetworkObject>().Spawn();
        //}
    }
}
