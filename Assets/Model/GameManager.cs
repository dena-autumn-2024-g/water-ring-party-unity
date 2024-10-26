using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Timer timer;
    public Score score;
    public GameObject ringPrefab;
    public GameObject[] poleObjects;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(ringPrefab);
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
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(timer.GetCurrentTime());
    }

    void OnTriggerStateChanged(bool isEnter)
    {
        Debug.Log("物体が入りました: " + isEnter);
    }
}
