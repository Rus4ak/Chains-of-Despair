using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    public static NetworkVariable<bool> isStartedSession = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Start()
    {
        if (!isStartedSession.Value && !NetworkManager.Singleton.IsListening)
        {
            StartSession();
        }

        else if (!isStartedSession.Value && NetworkManager.Singleton.IsListening)
        {
            isStartedSession.Value = true;
        }
    }

    private void StartSession()
    {
        NetworkManager.Singleton.StartHost();
        isStartedSession.Value = true;
    }
}
