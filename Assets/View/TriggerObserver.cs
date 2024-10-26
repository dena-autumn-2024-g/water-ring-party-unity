using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerObserver : MonoBehaviour
{
    private bool isEnter = false;
    public event Action<bool> OnTriggerStateChanged;

    public void SetTriggerState(bool value)
    {
        isEnter = value;
        OnTriggerStateChanged?.Invoke(isEnter);
    }

    private void OnTriggerEnter(Collider other)
    {
        SetTriggerState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        SetTriggerState(false);
    }

    public bool GetIsEnter()
    {
        return isEnter;
    }
}
