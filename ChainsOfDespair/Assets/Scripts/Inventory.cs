using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Image[] _slots;
    [SerializeField] private Transform _camera;
    [SerializeField] private float _throwForce;
    [SerializeField] private GameObject[] _availableObjects;

    private int _selectedSlot = 0;
    private string[] _slotObjectsName = new string[3];

    [HideInInspector] public bool[] freeSlots = new bool[] { true, true, true };
    [HideInInspector] public string selectedItem;

    private void Update()
    {
        HandleNumberInput();
        HandleScrollInput();

        if (Input.GetKeyDown(KeyCode.Q))
            ThrowItem();
    }

    public void FillSlot(Sprite itemSprite, string itemName, string objectName)
    {
        int firstFreeSlot = -1;

        for (int i = 0; i < freeSlots.Length; i++)
        {
            if (freeSlots[i])
            {
                firstFreeSlot = i;
                break;
            }
        }

        if (firstFreeSlot == -1)
            return;

        Image slot = _slots[firstFreeSlot];
        slot.enabled = true;
        slot.name = itemName;
        slot.sprite = itemSprite;

        freeSlots[firstFreeSlot] = false;
        _slotObjectsName[firstFreeSlot] = objectName;

        if (_selectedSlot == firstFreeSlot)
            selectedItem = slot.name;
    }

    public void ThrowItem()
    {
        Image slot = _slots[_selectedSlot];

        if (slot.sprite == null)
            return;

        foreach (var availableObject in _availableObjects)
        {
            if (availableObject.name == _slotObjectsName[_selectedSlot])
            {
                GameObject obj = Instantiate(availableObject, _camera.position, Quaternion.identity);
                obj.GetComponent<NetworkObject>().Spawn();
                obj.GetComponent<Rigidbody>().AddForce(_camera.forward * _throwForce);
                obj.name = _slots[_selectedSlot].name;

                _slotObjectsName[_selectedSlot] = "";
            }
        }

        slot.sprite = null;
        slot.name = "Image";
        slot.enabled = false;

        freeSlots[_selectedSlot] = true;
    }

    private void SelectSlot(int selectedSlot)
    {
        _slots[_selectedSlot].transform.parent.localScale = Vector3.one;
        _slots[selectedSlot].transform.parent.localScale = Vector3.one * 1.1f;

        selectedItem = _slots[selectedSlot].name;

        _selectedSlot = selectedSlot;
    }

    private void HandleNumberInput()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSlot(i);
                break;
            }
        }
    }

    private void HandleScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            if (_selectedSlot <= 0)
                SelectSlot(_slots.Length - 1);
            else
                SelectSlot(_selectedSlot - 1);
        }

        else if (scroll < 0f)
        {
            if (_selectedSlot >= _slots.Length - 1)
                SelectSlot(0);
            else
                SelectSlot(_selectedSlot + 1);
        }
    }
}
