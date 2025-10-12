using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    public static NetworkVariable<bool> isStartedSession = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        if (!isStartedSession.Value)
        {
            StartSession();
        }
    }

    private void StartSession()
    {
        NetworkManager.Singleton.StartHost();
        isStartedSession.Value = true;
    }
}
