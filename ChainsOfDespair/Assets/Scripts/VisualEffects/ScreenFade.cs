using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] private Image _blackScreen;
    [SerializeField] private float _fadeDuration = 1.5f;

    [HideInInspector] public bool isFade;

    public static ScreenFade Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void FadeOut()
    {
        isFade = false;
        StartCoroutine(Fade(0f, 1f));
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(1f, 0f));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float time = 0f;
        Color color = _blackScreen.color;
        while (time < _fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / _fadeDuration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            _blackScreen.color = color;
            yield return null;
        }

        isFade = true;
    }
}
