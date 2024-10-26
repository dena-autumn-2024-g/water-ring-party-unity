using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Timer timer;
    private Score score;
    public GameObject ringPrefab;
    public GameObject[] poleObjects;
    public FluidSimulation fluidSimulation;
    
    private Canon[] canons;
    private Player[] players;

    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Transform[] iconParents;
    // Start is called before the first frame update
    void Start()
    {
        score = new Score();
        timer = new Timer();
        timer.StartTimer();
        canons = new Canon[5];
        for (int i = 0; i < 5; i++) {
            canons[i] = new Canon(i);
        }
        foreach (var canon in canons) {
            Debug.Log("canon: " + canon.id);
        }

        var numPlayer = CommonInfoManager.NUM_PLAYER;
        if (numPlayer == 0) {
            numPlayer = 10;
        }
        players = new Player[numPlayer];
        for (int i = 0; i < numPlayer; i++) {
            players[i] = new Player(i, iconPrefab, iconParents);
            Debug.Log("player: " + players[i].GetPlayerId());
        }
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
        StartCoroutine(SpawnRingsCoroutine());
    }

    public void MoveLeft(int playerId)
    {
        players[playerId].switchTurretNumber(-1);
    }

    public void MoveRight(int playerId)
    {
        players[playerId].switchTurretNumber(1);
    }

    public void PushButtonPressed(int playerId)
    {
        players[playerId].setTurretState(true);
    }

    public void PushButtonReleased(int playerId)
    {
        players[playerId].setTurretState(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            players[0].switchTurretNumber(1);
            Debug.Log("→キーが押されました。全プレイヤーの砲台番号を1つ増やしました。");
        }
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
        for (int i = 0; i < 20; i++)
        {
            var prefab = Instantiate(ringPrefab);
            prefab.GetComponent<ringController>().fluidSimulation = fluidSimulation;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
