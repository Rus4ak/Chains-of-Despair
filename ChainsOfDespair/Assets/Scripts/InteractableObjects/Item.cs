using Unity.Netcode;
using UnityEngine;

enum ItemType
{
    Key,
    Battery
}

public class Item : NetworkBehaviour, IInteractable
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private GameObject _itemModel;
    [SerializeField] private Sprite _itemSprite;
    [SerializeField] private AudioSource _takeAudioSource;
    [SerializeField] private AudioSource _fallAudioSource;

    public string GetInteractionPrompt()
    {
        bool isFreeSlot = false;

        foreach (bool freeSlot in PlayersManager.Instance.ownerInventory.freeSlots) 
            if (freeSlot)
            {
                isFreeSlot = true;
                break;
            }

        if (itemType == ItemType.Key)
        {
            if (isFreeSlot)
                return $"Take key '{gameObject.name}'";
            else
                return $"Take key '{gameObject.name}'\n(Inventory is full)";
        }
        else if (itemType == ItemType.Battery) 
        {
            if (isFreeSlot)
                return "Take battery";
            else
                return "Take battery\n(Inventory is full)";
        }
        else
            return null;
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
            PlayersManager.Instance.ownerInventory.FillSlot(_itemSprite, gameObject.name, itemType.ToString());
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
        Destroy(_itemModel);

        _takeAudioSource.Play();
        DestroyKeyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKeyServerRpc()
    {
        Destroy(gameObject, _takeAudioSource.clip.length);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            _fallAudioSource.Play();
    }
}
