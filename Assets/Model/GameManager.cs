using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Timer timer;
    private Score score;
    public GameObject ringPrefab;
    public GameObject[] poleObjects;
    // Start is called before the first frame update
    void Start()
    {
        score = new Score();
        timer = new Timer();
        timer.StartTimer();
        foreach (var pole in poleObjects)
        {
            var stick = pole.transform.Find("stick");
            if (stick != null)
            {
                var triggerObserver = stick.GetComponent<TriggerObserver>();
                if (triggerObserver != null)
                {
                    triggerObserver.OnTriggerStateChanged += OnTriggerStateChanged;
                }
            }
        }
        // StartCoroutine(SpawnRingsCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(timer.GetCurrentTime());
    }

    void OnTriggerStateChanged(bool isEnter)
    {
        if (isEnter)
        {
            score.AddPoints(10);
            Debug.Log(score.ToString());
        }
        else
        {
            score.AddPoints(-10);
            Debug.Log(score.ToString());
        }
    }

    private IEnumerator SpawnRingsCoroutine()
    {
        for (int i = 0; i < 100; i++)
        {
            Instantiate(ringPrefab);
            yield return new WaitForSeconds(1f);
        }
    }
}
