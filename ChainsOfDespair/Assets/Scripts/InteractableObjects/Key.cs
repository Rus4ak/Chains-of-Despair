using Unity.Netcode;
using UnityEngine;

public class Key : NetworkBehaviour, IInteractable
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject _keyModel;

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
        Destroy(_keyModel);

        _audioSource.Play();
        Destroy(gameObject, _audioSource.clip.length);
    }
}
