using Unity.Netcode;
using UnityEngine;

public class LookAround : NetworkBehaviour
{
    [SerializeField] private Transform _camera;
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private GameObject[] _headObjects;
    [SerializeField] private Transform _neck;
    
    private float xRotation;
    private Transform _attackEnemy;

    public Transform Camera => _camera;

    private void Start()
    {
        if (!IsOwner && NetworkManager.Singleton.IsListening)
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
        if (!IsOwner)
            return;

        if (_attackEnemy != null)
        {
            Vector3 target = _attackEnemy.position;
            target.y = transform.position.y;

            transform.LookAt(target);
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);
    }
    
    private void LateUpdate()
    {
        if (!IsOwner)
            return;

        if (_attackEnemy != null)
        {
            _camera.LookAt(_attackEnemy);
            _neck.LookAt(_attackEnemy);
            return;
        }

        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, 10, 170);
        _camera.localRotation = Quaternion.Euler(xRotation, 0, 0);

        _neck.localRotation = Quaternion.Euler(xRotation - 90, 0, 0);
    }

    [ClientRpc]
    public void StartAttackClientRpc(NetworkObjectReference enemyRef)
    {
        if (IsOwner)
        {
            if (enemyRef.TryGet(out NetworkObject enemyObj))
            {
                if (enemyObj.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    _attackEnemy = enemy.midPoint;
                }
                else
                {
                    _attackEnemy = enemyObj.GetComponentInChildren<Enemy>().midPoint;
                }
            }
        }
    }
}
