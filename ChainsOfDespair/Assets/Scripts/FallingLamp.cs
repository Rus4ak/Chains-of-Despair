using Unity.Netcode;
using UnityEngine;

public class FallingLamp : NetworkBehaviour
{
    [SerializeField] private float _fallingForce = 25f;
    [SerializeField] private GameObject _light;

    private AudioSource _fallingSound;
    private bool _isFalling;
    private Rigidbody _rigibody;

    private void Awake()
    {
        _fallingSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_rigibody == null)
            return;

        if (_isFalling && _rigibody.IsSleeping())
            _isFalling = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartFallingServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartFallingServerRpc()
    {
        StartFallingClientRpc();
    }

    [ClientRpc]
    private void StartFallingClientRpc()
    {
        if (TryGetComponent<Rigidbody>(out _))
            return;

        _rigibody = gameObject.AddComponent<Rigidbody>();
        _rigibody.AddForce(Vector3.down * _fallingForce, ForceMode.Impulse);
        _isFalling = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isFalling)
        {
            for (int i = 0; i < Random.Range(1, 6); i++)
            {
                _fallingSound.Play();
            }

            Destroy(_light);
            Destroy(GetComponent<VisibilityCheck>());
        }
    }
}
