using UnityEngine;

[RequireComponent(typeof(Collider)), RequireComponent(typeof(AudioSource))]
public class Ragdoll : MonoBehaviour
{
    private AudioSource _hitSound;

    private void Start()
    {
        _hitSound = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_hitSound.isPlaying)
            _hitSound.Play();
    }
}
