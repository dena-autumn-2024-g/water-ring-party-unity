using System.Collections;
using UnityEngine;
using UniRx;
using System;

public abstract class Window : MonoBehaviour
{
    protected Window PrevWindow;

    private Subject<Unit> _onActivateWindow = new Subject<Unit>();
    public IObservable<Unit> OnActivateWindowObservable { get { return _onActivateWindow; } }

    public virtual void OnActivateWindow()
    {
        gameObject.SetActive(true);
        _onActivateWindow.OnNext(Unit.Default);
    }

    public virtual void OnDeactivateWindow()
    {
        gameObject.SetActive(false);
    }
}