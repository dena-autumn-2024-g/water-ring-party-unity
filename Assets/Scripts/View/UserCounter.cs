using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UserCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    private uint _userCount = 0;

    private void Awake()
    {
        _text.text = $"{_userCount} / 10";
    }

    public void AddUser()
    {
        _userCount++;
        _text.text = $"{_userCount} / 10";
    }
}
