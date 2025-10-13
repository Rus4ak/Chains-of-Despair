using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class StartedDoor : NetworkBehaviour, IInteractable
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public string GetInteractionPrompt()
    {
        return "Start the game";
    }

    public void Interact()
    {
        StartFadeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        PlayerInitialize ownerPlayer = PlayersManager.Instance.ownerPlayer;

        Vector3 position = ownerPlayer.transform.position;
        position.y = -50;

        ownerPlayer.Spawn(position);

        ScreenFade.Instance.FadeIn();
    }

    private IEnumerator StartFade()
    {
        ScreenFade.Instance.FadeOut();

        while (!ScreenFade.Instance.isFade)
        {
            yield return null;
        }

        StartGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartFadeServerRpc()
    {
        StartFadeClientRpc();
    }

    [ClientRpc]
    private void StartFadeClientRpc()
    {
        _audioSource.Play();
        StartCoroutine(StartFade());
    }
}
