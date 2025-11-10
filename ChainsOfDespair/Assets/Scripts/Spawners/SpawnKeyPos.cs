using Unity.Netcode;
using UnityEngine;

public class SpawnKeyPos : MonoBehaviour
{
    [SerializeField] private string[] _keysCantSpawn;

    public string[] KeysCantSpawn => _keysCantSpawn;

    private void Start()
    {
        if (!KeySpawner.Instance.isSpawned)
            KeySpawner.Instance.spawnPositions.Add(this);
    }
}
