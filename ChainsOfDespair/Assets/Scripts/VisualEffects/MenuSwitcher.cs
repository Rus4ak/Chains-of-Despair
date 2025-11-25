using UnityEngine;

public class MenuSwitcher : MonoBehaviour
{
    [SerializeField] private Transform _menuTarget;
    [SerializeField] private float _animationTime;
    [SerializeField] private float _progressStartAppear;
    [SerializeField] private float _overshoot = 1f;

    private Transform _activeMenu;

    public void ChangeMenu(Transform appearingMenu)
    {
        if (_activeMenu == null || _activeMenu == _menuTarget)
        {
            if (appearingMenu != _menuTarget)
                StartCoroutine(MenuTransition.Instance.AnimationRoutine(appearingMenu, _menuTarget, _animationTime, 0, _overshoot));
        }
        else if (_activeMenu != appearingMenu)
        {
            StartCoroutine(MenuTransition.Instance.AnimationRoutine(appearingMenu, _activeMenu, _animationTime, _progressStartAppear, _overshoot));
        }
        
        _activeMenu = appearingMenu;
    }
}
