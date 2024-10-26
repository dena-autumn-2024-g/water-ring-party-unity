using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Timer timer;
    public Score score;
    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer();
        timer.StartTimer();
        score = new Score();
        Debug.Log(score.ToString());
        score.AddPoints(10);
        Debug.Log(score.ToString());
        score.ResetScore();
        Debug.Log(score.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(timer.GetCurrentTime());
    }
}
