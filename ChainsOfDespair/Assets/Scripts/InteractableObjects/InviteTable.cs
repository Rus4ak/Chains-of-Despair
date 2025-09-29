using UnityEngine;

public class InviteTable : MonoBehaviour, IInteractable
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public string GetInteractionPrompt()
    {
        return "Invite friends in your lobby";
    }

    public void Interact()
    {
        _audioSource.Play();

        Debug.Log("Inviting friends");
    }
}
