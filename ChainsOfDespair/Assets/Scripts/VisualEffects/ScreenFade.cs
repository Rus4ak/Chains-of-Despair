using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] private Image _blackScreen;

    [HideInInspector] public bool isFade;

    public static ScreenFade Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);
        
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void FadeOut(float fadeDuration)
    {
        _blackScreen.gameObject.SetActive(true);
        isFade = false;
        StartCoroutine(Fade(0f, 1f, fadeDuration));
    }

    public void FadeIn(float fadeDuration)
    {
        StartCoroutine(Fade(1f, 0f, fadeDuration));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float fadeDuration)
    {
        float time = 0f;
        Color color = _blackScreen.color;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            _blackScreen.color = color;
            yield return null;
        }

        isFade = true;

        if (endAlpha == 0)
            _blackScreen.gameObject.SetActive(false);

        if (PlayersManager.Instance.ownerCalculateDistance != null)
            PlayersManager.Instance.ownerCalculateDistance.enabled = true;
    }
}
