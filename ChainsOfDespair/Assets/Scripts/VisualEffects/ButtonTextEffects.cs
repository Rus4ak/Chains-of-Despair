using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTextEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _addSize;
    [SerializeField] private float _addColor;

    public void OnPointerDown(PointerEventData eventData)
    {
        _text.fontSize += _addSize;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Color textColor = _text.color;
        textColor += new Color(_addColor, _addColor, _addColor);
        _text.color = textColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Color textColor = _text.color;
        textColor -= new Color(_addColor, _addColor, _addColor);
        _text.color = textColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _text.fontSize -= _addSize;
    }
}
