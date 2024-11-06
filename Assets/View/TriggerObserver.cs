using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerObserver : MonoBehaviour
{
    private bool isEnter = false;
    public event Action<bool, Collider> OnTriggerStateChanged;

    public void SetTriggerState(bool value, Collider other)
    {
        isEnter = value;
        OnTriggerStateChanged?.Invoke(isEnter, other);
    }

    private void OnTriggerEnter(Collider other)
    {
        SetTriggerState(true, other);
    }

    private void OnTriggerExit(Collider other)
    {
        SetTriggerState(false, other);
    }

    public bool GetIsEnter()
    {
        return isEnter;
    }
}
