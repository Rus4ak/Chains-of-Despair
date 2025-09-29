using Unity.Netcode;
using UnityEngine;

public class MainMenuManager : NetworkBehaviour
{
    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }
}
