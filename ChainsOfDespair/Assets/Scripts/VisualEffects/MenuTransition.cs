using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuTransition : MonoBehaviour
{
    private AudioSource _menuSwitchSound;

    [HideInInspector] public bool isAnimate;

    public static MenuTransition Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _menuSwitchSound = GetComponent<AudioSource>();
    }

    public IEnumerator AnimationRoutine(Transform appearingMenu, Transform disappearingMenu, float animationTime, float progressStartAppear, float overshoot)
    {
        while (isAnimate)
            yield return new WaitForEndOfFrame();

        isAnimate = true;
        
        Vector3 appearingMenuStartPos = appearingMenu.localPosition;
        Vector3 disappearingMenuStartPos = disappearingMenu.localPosition;

        Coroutine appearRoutine = null;

        float halfTime = animationTime / 2;
        float t = 0f;

        if (disappearingMenu.name != "Target")
            _menuSwitchSound.Play();

        while (t < halfTime)
        {
            t += Time.deltaTime;
            float progress = t / halfTime;

            float eased = EaseOutBack(progress, overshoot);

            disappearingMenu.localPosition = Vector3.LerpUnclamped(disappearingMenuStartPos, appearingMenuStartPos, eased);

            if (appearRoutine == null && progress >= progressStartAppear)
            {
                if (appearingMenu.name != "Target")
                    _menuSwitchSound.Play();

                appearRoutine = StartCoroutine(
                    MoveWithBackEasing(
                        appearingMenu,
                        appearingMenuStartPos,
                        disappearingMenuStartPos,
                        halfTime * 0.5f,
                        overshoot
                    )
                );
            }

            yield return null;
        }

        disappearingMenu.localPosition = appearingMenuStartPos;

        if (appearRoutine != null)
            yield return appearRoutine;
        
        isAnimate = false;
    }

    private IEnumerator MoveWithBackEasing(Transform target, Vector3 start, Vector3 end, float time, float overshoot)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float progress = t / time;

            float eased = EaseOutBack(progress, overshoot);

            target.localPosition = Vector3.LerpUnclamped(start, end, eased);

            yield return null;
        }

        target.localPosition = end;
    }

    private float EaseOutBack(float x, float overshoot)
    {
        float c1 = overshoot;
        float c3 = c1 + 1f;

        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }
}
