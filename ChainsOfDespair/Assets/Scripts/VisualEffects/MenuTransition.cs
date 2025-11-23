using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuTransition : MonoBehaviour
{
    public static MenuTransition Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;
    }

    public IEnumerator AnimationRoutine(Transform appearingMenu, Transform disappearingMenu, float animationTime, float progressStartAppear)
    {
        Vector3 appearingMenuStartPos = appearingMenu.localPosition;
        Vector3 disappearingMenuStartPos = disappearingMenu.localPosition;

        Coroutine appearRoutine = null;

        float halfTime = animationTime / 2;
        float t = 0f;

        while (t < halfTime)
        {
            t += Time.deltaTime;
            float progress = t / halfTime;

            float eased = EaseOutBack(progress);

            disappearingMenu.localPosition = Vector3.LerpUnclamped(disappearingMenuStartPos, appearingMenuStartPos, eased);

            if (appearRoutine == null && progress >= progressStartAppear)
            {
                appearRoutine = StartCoroutine(
                    MoveWithBackEasing(
                        appearingMenu,
                        appearingMenuStartPos,
                        disappearingMenuStartPos,
                        halfTime * 0.5f
                    )
                );
            }

            yield return null;
        }

        disappearingMenu.localPosition = appearingMenuStartPos;

        if (appearRoutine != null)
            yield return appearRoutine;
    }

    private IEnumerator MoveWithBackEasing(Transform target, Vector3 start, Vector3 end, float time)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float progress = t / time;

            float eased = EaseOutBack(progress);

            target.localPosition = Vector3.LerpUnclamped(start, end, eased);

            yield return null;
        }

        target.localPosition = end;
    }

    private float EaseOutBack(float x)
    {
        float c1 = 1f;
        float c3 = c1 + 1f;

        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }
}
