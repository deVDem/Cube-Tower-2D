using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string key;

    private void Start()
    {
        Text component = GetComponent<Text>();
        if(component==null) return;
        if (string.IsNullOrEmpty(key)) key = component.text;
        component.text = LocaleManager.GetLocalizedText(key);
    }
}

