using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class InputFieldHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public InputField Input;
    public Image Highlight;

    public Color NormalColor = new Color(225, 255, 255, 0);
    public Color SelectedColor;

    private void Start()
    {
        Highlight.color = NormalColor;
    }

    public void OnSelect(BaseEventData eventData)
    {
        Highlight.color = SelectedColor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Highlight.color = NormalColor;
    }
}
