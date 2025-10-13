using Unity.Netcode;
using UnityEngine;

public class Key : NetworkBehaviour, IInteractable
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public string GetInteractionPrompt()
    {
        return $"Take key '{gameObject.name}'";
    }

    public void Interact()
    {
        TakeKeyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakeKeyServerRpc()
    {
        TakeKeyClientRpc();
    }

    [ClientRpc]
    private void TakeKeyClientRpc()
    {
        _audioSource.Play();
        Destroy(gameObject);
    }
}
