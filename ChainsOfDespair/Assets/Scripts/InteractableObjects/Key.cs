using Unity.Netcode;
using UnityEngine;

public class Key : NetworkBehaviour, IInteractable
{
    [SerializeField] private GameObject _keyModel;
    [SerializeField] private Sprite _keySprite;
    [SerializeField] private AudioSource _takeKeyAudioSource;
    [SerializeField] private AudioSource _fallKeyAudioSource;

    public string GetInteractionPrompt()
    {
        bool isFreeSlot = false;

        foreach (bool freeSlot in PlayersManager.Instance.ownerInventory.freeSlots) 
            if (freeSlot)
            {
                isFreeSlot = true;
                break;
            }

        if (isFreeSlot)
            return $"Take key '{gameObject.name}'";
        else
            return $"Take key '{gameObject.name}'\n(Inventory is full)";
    }

    public void Interact()
    {
        bool isFreeSlot = false;

        foreach (bool freeSlot in PlayersManager.Instance.ownerInventory.freeSlots)
            if (freeSlot)
            {
                isFreeSlot = true;
                break;
            }

        if (isFreeSlot)
        {
            PlayersManager.Instance.ownerInventory.FillSlot(_keySprite, gameObject.name, "Key");
            TakeKeyServerRpc();
        }
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

        _takeKeyAudioSource.Play();
        DestroyKeyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKeyServerRpc()
    {
        Destroy(gameObject, _takeKeyAudioSource.clip.length);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            _fallKeyAudioSource.Play();
    }
}
