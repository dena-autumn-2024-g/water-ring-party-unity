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


    private void Awake()
    {
        _text.text = $"{CommonInfoManager.NUM_PLAYER} / 10";
    }

    public void SetUser(int userCount)
    {
        Debug.Log($"ÉvÉåÉCÉÑÅ[êî: {userCount}");
        _text.text = $"{userCount} / 10";
    }
}
