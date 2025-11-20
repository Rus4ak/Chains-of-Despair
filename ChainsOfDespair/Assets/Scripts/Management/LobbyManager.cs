using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    private void Start()
    {
        if (!NetworkManager.Singleton.IsListening)
        {
            StartSession();
        }
    }

    private void StartSession()
    {
        NetworkManager.Singleton.StartHost();
    }
}
