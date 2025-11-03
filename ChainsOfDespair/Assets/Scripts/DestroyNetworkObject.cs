using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class DestroyNetworkObject : MonoBehaviour
{
    void Start()
    {
        GetComponent<NetworkObject>().DestroyWithScene = true;
    }
}
