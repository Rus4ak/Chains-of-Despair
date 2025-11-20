using Unity.Netcode;
using UnityEngine;

public class MainMenuManager : NetworkBehaviour
{
    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
