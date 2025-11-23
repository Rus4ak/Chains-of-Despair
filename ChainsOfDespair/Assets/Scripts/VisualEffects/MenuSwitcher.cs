using UnityEngine;

public class MenuSwitcher : MonoBehaviour
{
    [SerializeField] private Transform _menuTarget;
    [SerializeField] private float _animationTime;
    [SerializeField] private float _progressStartAppear;

    private Transform _activeMenu;

    public void ChangeMenu(Transform appearingMenu)
    {
        if (_activeMenu == null || _activeMenu == _menuTarget)
        {
            StartCoroutine(MenuTransition.Instance.AnimationRoutine(appearingMenu, _menuTarget, _animationTime, 0));
        }
        else if (_activeMenu != appearingMenu)
        {
            StartCoroutine(MenuTransition.Instance.AnimationRoutine(appearingMenu, _activeMenu, _animationTime, _progressStartAppear));
        }
        
        _activeMenu = appearingMenu;
    }
}
