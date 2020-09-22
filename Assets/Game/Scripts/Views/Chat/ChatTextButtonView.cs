using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using GT.Store;

public class ChatTextButtonView : MonoBehaviour
{
    public Text text;
    StoreItem data;

    public void Init(StoreItem data, UnityAction<string> action)
    {
        this.data = data;
        this.text.text = data.Name;
        GetComponent<Button>().onClick.AddListener(() =>action(this.data.Id));
    }
}
