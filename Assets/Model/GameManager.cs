using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Timer timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer();
        timer.StartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(timer.GetCurrentTime());
    }
}
