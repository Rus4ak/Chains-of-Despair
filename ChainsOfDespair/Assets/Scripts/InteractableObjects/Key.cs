using Unity.Netcode;
using UnityEngine;

public class Key : NetworkBehaviour, IInteractable
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject _keyModel;
    [SerializeField] private Sprite _keySprite;

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
        PlayersManager.Instance.ownerInventory.FillSlot(_keySprite, gameObject.name, "Key");

        Destroy(_keyModel);

        _audioSource.Play();
        Destroy(gameObject, _audioSource.clip.length);
    }
}
