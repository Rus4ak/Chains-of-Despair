using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    [SerializeField] private GameObject _staminaUI;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private float _drainRate = 0.2f;
    [SerializeField] private float _recoveryRate = 0.1f;

    private bool _isStaminaUsing = false;
    private float _sliderSpeed;

    [HideInInspector] public bool isCanRun = true;

    private void Update()
    {
        isCanRun = _staminaSlider.value > 0;

        if (!_isStaminaUsing)
            return;

        _staminaSlider.value += _sliderSpeed * Time.deltaTime;

        if (_staminaSlider.value == 1)
            _staminaUI.SetActive(false);

        if (_staminaSlider.value == 1 || _staminaSlider.value == 0)
            _isStaminaUsing = false;
    }

    public void StartUse()
    {
        _staminaUI.SetActive(true);
        _sliderSpeed = -_drainRate;
        _isStaminaUsing = true;
        
    }   

    public void StopUse()
    {
        _sliderSpeed = _recoveryRate;
        _isStaminaUsing = true;
    }
}
