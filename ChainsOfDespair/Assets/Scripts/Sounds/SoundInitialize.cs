using UnityEngine;
using UnityEngine.Audio;

enum SoundType
{
    Player,
    Enemies,
    Environment,
    UI
}

public class SoundInitialize : MonoBehaviour
{
    [SerializeField] private SoundType _soundType;

    private AudioSource _audioSource;
    private float _basicVolume;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _basicVolume = _audioSource.volume;

        ApplyVolume();
        SoundsVolume.Instance.OnVolumeChanged += ApplyVolume;
    }

    private void OnDestroy()
    {
        SoundsVolume.Instance.OnVolumeChanged -= ApplyVolume;
    }

    private void ApplyVolume()
    {
        switch (_soundType)
        {
            case SoundType.Player:
                _audioSource.volume = _basicVolume * SoundsVolume.Instance.generalVolume * SoundsVolume.Instance.playerVolume;
                break;

            case SoundType.Enemies:
                _audioSource.volume = _basicVolume * SoundsVolume.Instance.generalVolume * SoundsVolume.Instance.enemiesVolume;
                break;

            case SoundType.Environment:
                _audioSource.volume = _basicVolume * SoundsVolume.Instance.generalVolume * SoundsVolume.Instance.environmentVolume;
                break;

            case SoundType.UI:
                _audioSource.volume = _basicVolume * SoundsVolume.Instance.generalVolume * SoundsVolume.Instance.UIVolume;
                break;
        }
    }
}
