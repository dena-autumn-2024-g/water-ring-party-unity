using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartGameButton : MonoBehaviour
{
    [SerializeField]
    private Button _button;

    public IObservable<Unit> Observable
    {
        get { return _button.OnClickAsObservable(); }
    }

    public void SetInteractable(bool isInteractable)
    {
        _button.interactable = isInteractable;
    }
}
