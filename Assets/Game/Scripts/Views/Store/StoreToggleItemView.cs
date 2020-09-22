using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StoreToggleItemView : MonoBehaviour
{
    public Text Name;
    public Toggle toggle;
    public Animator animator;

    public void Populate(string name, ToggleGroup toggleGroup, UnityAction<bool> action)
    {
        Name.text = Utils.LocalizeTerm(name);
        toggle.group = toggleGroup;
        toggleGroup.RegisterToggle(toggle);
        toggle.isOn = false;
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(b => { action(b); OnValueChange(b); });
    }

    private void OnValueChange(bool isOn)
    {
        animator.SetBool("IsOn", isOn);
    }
}
