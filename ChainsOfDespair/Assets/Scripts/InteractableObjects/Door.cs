using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using WebSocketSharp;

[Serializable]
public class DoorData
{
    public Transform door;
    public float zMoveTo;
}

public class Door : NetworkBehaviour, IInteractable
{
    [SerializeField] private float _openTime = 1.5f;
    [SerializeField] private DoorData[] _doors;
    [SerializeField] private AudioSource _lockedDoorAudioSource;
    [SerializeField] private AudioSource _unlockDoorAudioSource;
    [SerializeField] private string _keyName = null;

    private bool _isMove = false;
    private AudioSource _audioSource;

    public string KeyName => _keyName;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
        if (!KeySpawner.Instance.isSpawned)
            if (!_keyName.IsNullOrEmpty())
                KeySpawner.Instance.lockedDoors.Add(this);
    }

    public string GetInteractionPrompt()
    {
        if (_keyName.IsNullOrEmpty())
            return "Open the door";
        else
            return $"Locked, key '{_keyName}' required";
    }

    public void Interact()
    {
        if (_keyName.IsNullOrEmpty())
            OpenDoorServerRpc();
        else
        {
            if (PlayersManager.Instance.ownerInventory.selectedItem == _keyName)
            {
                _keyName = null;
                _unlockDoorAudioSource.Play();
            }
            
            else
                _lockedDoorAudioSource.Play();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OpenDoorServerRpc()
    {
        OpenDoorClientRpc();
    }

    [ClientRpc]
    private void OpenDoorClientRpc()
    {
        if (!_isMove)
        {
            _audioSource.Play();
            StartCoroutine(Open());
        }
    }

    private IEnumerator Open()
    {
        _isMove = true;

        float time = 0f;

        Vector3[] startRotation = new Vector3[_doors.Length];
        Vector3[] endRotation = new Vector3[_doors.Length];

        for (int i = 0; i < _doors.Length; i++)
        {
            startRotation[i] = _doors[i].door.rotation.eulerAngles;

            endRotation[i] = new Vector3(startRotation[i].x, startRotation[i].y, startRotation[i].z + _doors[i].zMoveTo);

            _doors[i].zMoveTo = -_doors[i].zMoveTo;
        }

        while (time < _openTime)
        {
            time += Time.deltaTime;
            float t = time / _openTime;

            for (int i = 0; i < _doors.Length; i++)
            {
                Vector3 rotation = Vector3.Lerp(startRotation[i], endRotation[i], t);
                _doors[i].door.rotation = Quaternion.Euler(rotation);
            }    

            yield return null;
        }

        _isMove = false;
    }
}
