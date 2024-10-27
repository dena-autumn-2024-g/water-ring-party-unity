using System.Collections;
using UnityEngine;
using TMPro;

public class BaseText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    public TextMeshProUGUI Text { get { return _text; } }

    public void SetText(string text)
    {
        Text.text = text;
    }
}