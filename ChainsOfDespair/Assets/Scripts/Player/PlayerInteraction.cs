using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _promptText;
    [SerializeField] private float _checkDistance = 8f;
    [SerializeField] private Transform _camera;

    private void Start()
    {
        if (!IsOwner)
            _promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _checkDistance))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                _promptText.text = "[E]\n" + interactable.GetInteractionPrompt();
                _promptText.gameObject.SetActive(true);
                
                if (Input.GetKeyDown(KeyCode.E))
                    interactable.Interact();
            }
            else
                if (_promptText.gameObject.activeInHierarchy)
                    _promptText.gameObject.SetActive(false);
        }
        else
            if (_promptText.gameObject.activeInHierarchy)
                _promptText.gameObject.SetActive(false);
    }
}
