using System;
using UnityEngine;

public class SoundsVolume : MonoBehaviour
{
    public float generalVolume = .75f;
    public float playerVolume = 1.0f;
    public float enemiesVolume = 1.0f;
    public float environmentVolume = 1.0f;
    public float UIVolume = 1.0f;

    public event Action OnVolumeChanged;

    public static SoundsVolume Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("GeneralVolume"))
            generalVolume = PlayerPrefs.GetFloat("GeneralVolume");

        if (PlayerPrefs.HasKey("PlayerVolume"))
            playerVolume = PlayerPrefs.GetFloat("PlayerVolume");

        if (PlayerPrefs.HasKey("EnemiesVolume"))
            enemiesVolume = PlayerPrefs.GetFloat("EnemiesVolume");

        if (PlayerPrefs.HasKey("EnvironmentVolume"))
            environmentVolume = PlayerPrefs.GetFloat("EnvironmentVolume");

        if (PlayerPrefs.HasKey("UIVolume"))
            UIVolume = PlayerPrefs.GetFloat("UIVolume");
    }

    public void ChangeVolume(string type, float volume)
    {
        switch (type)
        {
            case "General":
                generalVolume = volume;
                PlayerPrefs.SetFloat("GeneralVolume", generalVolume);
                break;

            case "Player":
                playerVolume = volume;
                PlayerPrefs.SetFloat("PlayerVolume", playerVolume);
                break;

            case "Enemies":
                enemiesVolume = volume;
                PlayerPrefs.SetFloat("EnemiesVolume", enemiesVolume);
                break;

            case "Environment":
                environmentVolume = volume;
                PlayerPrefs.SetFloat("EnvironmentVolume", environmentVolume);
                break;

            case "UI":
                UIVolume = volume;
                PlayerPrefs.SetFloat("UIVolume", UIVolume);
                break;
        }

        OnVolumeChanged?.Invoke();
    }
}
