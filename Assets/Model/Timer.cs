using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float startTime;
    private float elapsedTime;
    private bool isRunning;

    public void StartTimer()
    {
        if (!isRunning)
        {
            startTime = Time.time;
            isRunning = true;
        }
    }

    public void StopTimer()
    {
        if (isRunning)
        {
            elapsedTime += Time.time - startTime;
            isRunning = false;
        }
    }

    public float GetCurrentTime()
    {
        if (isRunning)
        {
            return elapsedTime + (Time.time - startTime);
        }
        else
        {
            return elapsedTime;
        }
    }
}
