using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class StartedDoor : NetworkBehaviour, IInteractable
{
    [Header("Spawn area")]
    [SerializeField] private Vector2 _minPos;
    [SerializeField] private Vector2 _maxPos;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public string GetInteractionPrompt()
    {
        return "Start the game";
    }

    public void Interact()
    {
        StartFadeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        KeySpawner.Instance.SpawnKeys();
        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        PlayersManager.Instance.ownerCalculateDistance.enabled = false;

        PlayerInitialize ownerPlayer = PlayersManager.Instance.ownerPlayer;

        Vector3 position = ownerPlayer.transform.position;
        position.y = -50;
        position.x = Mathf.Clamp(position.x, _minPos.x, _maxPos.x);
        position.z = Mathf.Clamp(position.z, _minPos.y, _maxPos.y);
        
        ownerPlayer.Spawn(position);

        ScreenFade.Instance.FadeIn(1.5f);
    }

    private IEnumerator StartFade()
    {
        ScreenFade.Instance.FadeOut(1.5f);

        while (!ScreenFade.Instance.isFade)
        {
            yield return null;
        }

        StartGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartFadeServerRpc()
    {
        StartFadeClientRpc();
    }

    [ClientRpc]
    private void StartFadeClientRpc()
    {
        _audioSource.Play();
        StartCoroutine(StartFade());
    }
}
