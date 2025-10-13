using Unity.Netcode;
using UnityEngine;

public class SpawnKeyPos : MonoBehaviour
{
    [SerializeField] private string[] _keysCantSpawn;

    public string[] KeysCantSpawn => _keysCantSpawn;

    private void Awake()
    {
        if (!KeySpawner.isSpawned)
            KeySpawner.spawnPositions.Add(this);
    }
}
