using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BaseButton : MonoBehaviour
{
    [SerializeField]
    private Button _button;

    public IObservable<Unit> Observable
    {
        get { return _button.OnClickAsObservable(); }
    }
}