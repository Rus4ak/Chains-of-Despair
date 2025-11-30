using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] private Slider _audioGeneralSlider;
    [SerializeField] private Slider _audioPlayerSlider;
    [SerializeField] private Slider _audioEnemiesSlider;
    [SerializeField] private Slider _audioEnvironmentSlider;
    [SerializeField] private Slider _audioUISlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("GeneralVolume"))
            _audioGeneralSlider.value = PlayerPrefs.GetFloat("GeneralVolume");

        if (PlayerPrefs.HasKey("PlayerVolume"))
            _audioPlayerSlider.value = PlayerPrefs.GetFloat("PlayerVolume");

        if (PlayerPrefs.HasKey("EnemiesVolume"))
            _audioEnemiesSlider.value = PlayerPrefs.GetFloat("EnemiesVolume");

        if (PlayerPrefs.HasKey("EnvironmentVolume"))
            _audioEnvironmentSlider.value = PlayerPrefs.GetFloat("EnvironmentVolume");

        if (PlayerPrefs.HasKey("UIVolume"))
            _audioUISlider.value = PlayerPrefs.GetFloat("UIVolume");
    }

    public void Save()
    {
        SoundsVolume.Instance.ChangeVolume("General", _audioGeneralSlider.value);
        SoundsVolume.Instance.ChangeVolume("Player", _audioPlayerSlider.value);
        SoundsVolume.Instance.ChangeVolume("Enemies", _audioEnemiesSlider.value);
        SoundsVolume.Instance.ChangeVolume("Environment", _audioEnvironmentSlider.value);
        SoundsVolume.Instance.ChangeVolume("UI", _audioUISlider.value);
    }
}
