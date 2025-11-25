using UnityEngine;
using UnityEngine.UI;

public class ChangeMenu : MonoBehaviour
{
    [SerializeField] private Transform _appearingMenu;
    [SerializeField] private Transform _disappearingMenu;
    [SerializeField] private float _animationTime;
    [SerializeField] private float _progressStartAppear;
    [SerializeField] private float _overshoot = 1f;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(StartAnimation);
    }

    private void StartAnimation()
    {
        StartCoroutine(MenuTransition.Instance.AnimationRoutine(_appearingMenu, _disappearingMenu, _animationTime, _progressStartAppear, _overshoot));
    }
}
