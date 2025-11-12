using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDied : NetworkBehaviour
{
    [SerializeField] private GameObject _ragdollPlayer;
    [SerializeField] private GameObject _playerMesh;
    [SerializeField] private GameObject _playerInterface;
    [SerializeField] private GameObject _deadCamera;

    private NetworkVariable<NetworkObjectReference> _diedPlayerNO = new NetworkVariable<NetworkObjectReference>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public event Action OnDeath;

    public void Died()
    {
        DiedClientRpc();
    }

    [ClientRpc]
    private void DiedClientRpc()
    {
        if (IsOwner)
        {
            OnDeath?.Invoke();
        }
    }

    private void OnEnable()
    {
        OnDeath += Dead;
    }

    private void Dead()
    {
        StartCoroutine(StartFade());
        _diedPlayerNO.Value = NetworkObject;
        DeadServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeadServerRpc()
    {
        GameObject ragdoll = Instantiate(_ragdollPlayer, transform.position, transform.rotation);
        ragdoll.GetComponent<NetworkObject>().Spawn();

        DeadClientRpc(ragdoll.GetComponent<NetworkObject>());
    }

    [ClientRpc]
    private void DeadClientRpc(NetworkObjectReference ragdollRef)
    {
        GetComponent<PlayerInitialize>().DestroyChain(_diedPlayerNO.Value);
        GetComponent<Collider>().enabled = false;
        _playerMesh.SetActive(false);
        _playerInterface.SetActive(false);

        if (!ragdollRef.TryGet(out NetworkObject ragdoll))
            return;

        GetComponent<PlayerInitialize>().ragdoll = ragdoll.transform.Find("Hips").gameObject;
        
        if (IsOwner)
        {
            _deadCamera.SetActive(true);
        }
    }

    private IEnumerator StartFade()
    {
        ScreenFade.Instance.FadeOut(1);

        while (!ScreenFade.Instance.isFade)
        {
            yield return null;
        }

        RestartGameServerRpc();

        ScreenFade.Instance.FadeIn(1.5f);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RestartGameServerRpc()
    {
        bool isAnyoneAlive = false;

        foreach (PlayerInitialize player in PlayersManager.Instance.players)
        {
            if (player.isAlive) 
                isAnyoneAlive = true;
        }

        if (!isAnyoneAlive)
        {
            PlayersManager.Instance.RestartGame();
            //Destroy(ScreenFade.Instance.gameObject);
        }
    }
}
