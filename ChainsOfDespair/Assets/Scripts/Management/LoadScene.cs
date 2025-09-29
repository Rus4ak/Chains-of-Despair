using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void SimpleLoad(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void NetworkLoad(string name)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(name, LoadSceneMode.Single);
    }
}
