using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuTransition : MonoBehaviour
{
    [SerializeField] private Transform _appearingMenu;
    [SerializeField] private Transform _disappearingMenu;
    [SerializeField] private float _animationTime;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(StartAnimation);
    }

    private void StartAnimation()
    {
        StartCoroutine(AnimationRoutine());
    }

    private IEnumerator AnimationRoutine()
    {
        Vector3 appearingMenuStartPos = _appearingMenu.localPosition;
        Vector3 disappearingMenuStartPos = _disappearingMenu.localPosition;

        Coroutine appearRoutine = null;

        float halfTime = _animationTime / 2;
        float t = 0f;

        while (t < halfTime)
        {
            t += Time.deltaTime;
            float progress = t / halfTime;

            float eased = EaseOutBack(progress);

            _disappearingMenu.localPosition = Vector3.LerpUnclamped(disappearingMenuStartPos, appearingMenuStartPos, eased);

            if (appearRoutine == null && progress >= 0.15f)
            {
                appearRoutine = StartCoroutine(
                    MoveWithBackEasing(
                        _appearingMenu,
                        appearingMenuStartPos,
                        disappearingMenuStartPos,
                        halfTime * 0.5f
                    )
                );
            }

            yield return null;
        }

        _disappearingMenu.localPosition = appearingMenuStartPos;

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
