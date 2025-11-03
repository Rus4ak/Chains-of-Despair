using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayersManager : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab;

    public static PlayersManager Instance;

    [HideInInspector] public List<PlayerInitialize> players = new List<PlayerInitialize>();
    [HideInInspector] public PlayerInitialize ownerPlayer;
    [HideInInspector] public Inventory ownerInventory;
    [HideInInspector] public CalculateDistance ownerCalculateDistance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void RestartGame()
    {
        NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
        NetworkManager.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (!IsServer)
            return;
        
        foreach (var kvp in NetworkManager.Singleton.ConnectedClients)
        {
            ulong clientID = kvp.Key;

            if (kvp.Value.PlayerObject == null)
            {
                NetworkObject player = Instantiate(_playerPrefab, new Vector3(0, 0, -3f * clientID), Quaternion.identity).GetComponent<NetworkObject>();
                player.SpawnWithOwnership(clientID);
            }
        }
    }
}
