using Unity.Netcode;
using UnityEngine;

public class LookAround : NetworkBehaviour
{
    [SerializeField] private Transform _camera;
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private GameObject[] _headObjects;
    [SerializeField] private Transform _neck;
    
    private float xRotation;

    public Transform Camera => _camera;

    private void Start()
    {
        if (!IsOwner && LobbyManager.isStartedSession.Value)
        {
            _camera.gameObject.SetActive(false);
            
            foreach (var obj in _headObjects)
            {
                obj.layer = 0;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!IsOwner || !PlayersManager.Instance.ownerPlayer.isMove)
            return;

        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);
    }
    
    private void LateUpdate()
    {
        if (!IsOwner || !PlayersManager.Instance.ownerPlayer.isMove)
            return;

        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, 10, 170);
        _camera.localRotation = Quaternion.Euler(xRotation, 0, 0);

        _neck.localRotation = Quaternion.Euler(xRotation - 90, 0, 0);
    }
}
