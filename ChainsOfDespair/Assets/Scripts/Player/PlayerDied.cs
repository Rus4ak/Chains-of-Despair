using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDied : NetworkBehaviour
{
    [SerializeField] private GameObject _ragdollPlayer;
    [SerializeField] private GameObject _playerMesh;

    private NetworkVariable<NetworkObjectReference> _diedPlayerNO = new NetworkVariable<NetworkObjectReference>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public event Action OnDeath;

    public void Died()
    {
        if (IsOwner)
            OnDeath?.Invoke();
    }

    private void OnEnable()
    {
        OnDeath += Dead;
    }

    private void Dead()
    {
        _diedPlayerNO.Value = NetworkObject;
        DeadServerRpc();
        StartCoroutine(StartFade());
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeadServerRpc()
    {
        GameObject ragdoll = Instantiate(_ragdollPlayer, transform.position, transform.rotation);
        ragdoll.GetComponent<NetworkObject>().Spawn();

        DeadClientRpc();
    }

    [ClientRpc]
    private void DeadClientRpc()
    {
        GetComponent<PlayerInitialize>().DestroyChain(_diedPlayerNO.Value);
        GetComponent<Collider>().enabled = false;
        _playerMesh.SetActive(false);
    }

    private IEnumerator StartFade()
    {
        ScreenFade.Instance.FadeOut();

        while (!ScreenFade.Instance.isFade)
        {
            yield return null;
        }

        bool isAnyoneAlive = false;

        foreach (PlayerInitialize player in PlayersManager.Instance.players)
            if (player.isAlive)
                isAnyoneAlive = true;

        if (!isAnyoneAlive)
        {
            RestartGameServerRpc();
            Destroy(ScreenFade.Instance.gameObject);
        }

        ScreenFade.Instance.FadeIn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RestartGameServerRpc()
    {
        PlayersManager.Instance.RestartGame();
    }
}
