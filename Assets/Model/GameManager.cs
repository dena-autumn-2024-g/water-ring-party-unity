using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    private Timer timer;
    private Score score;
    public GameObject ringPrefab;
    public GameObject[] poleObjects;
    public FluidSimulation fluidSimulation;

    public float limitTime = 300;
    public int RingNum = 20;
    
    private Canon[] canons;
    private Player pcPlayer;
    private Player[] players;
    private float fixedFluidPower;

    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Transform[] iconParents;
    [SerializeField] private Text timerText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text ringSumText;

    [SerializeField] private float fluidPower = 0.1f;
    [SerializeField] private float singleFluidPower = 2f;
    // Start is called before the first frame update
    void Start()
    {
        _prevRingCount = 0;
        DecreaseScoreCount(_prevRingCount);
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

        pcPlayer = new Player(9, iconPrefab, iconParents);
        fixedFluidPower = fluidPower;

        var numPlayer = CommonInfoManager.NUM_PLAYER;
        if (numPlayer == 0) {
            numPlayer = 10;
        }
        if (CommonInfoManager.NUM_PLAYER == 1)
        {
            fixedFluidPower = fluidPower * singleFluidPower;
        }
        players = new Player[numPlayer-1];
        for (int i = 0; i < numPlayer-1; i++) {
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
        _isRunning = true;
        StartCoroutine(SpawnRingsCoroutine());
    }

    private bool _isRunning = false;

    public void OnGameFinished()
    {   // 時間切れで終了
        CommonInfoManager.SCORE_LEFT_TIME = 0;
        CommonInfoManager.SCORE_RING_COUNT = score.GetCurrentScore();
        CommonInfoManager.SCORE_POINT = score.GetCurrentScore() + CommonInfoManager.SCORE_LEFT_TIME;

        SceneManager.LoadScene("Score");
    }

    public void MoveLeft(int playerId)
    {
        if (!players[playerId].GetTurretState()) {  
            players[playerId].switchTurretNumber(-1);
            Debug.Log($"←キーが押されました。プレイヤーID: {playerId}");
        }
    }

    public void MoveRight(int playerId)
    {
        if (!players[playerId].GetTurretState()) {  
            players[playerId].switchTurretNumber(1);
            Debug.Log($"→キーが押されました。プレイヤーID: {playerId}");
        }
    }

    public void PushButtonPressed(int playerId)
    {
            handleButtonPressed(playerId, true);
            Debug.Log($"スペースキーが押されました。プレイヤーID: {playerId}");
    }

    public void PushButtonReleased(int playerId)
    {
        handleButtonPressed(playerId, false);
        Debug.Log($"スペースキーが離されました。プレイヤーID: {playerId}");
    }

    // Update is called once per frame
    void Update()
    {
        if (_isRunning)
        {
            float time = getRemainingTime();
            if (time <= 0)
            {
                OnGameFinished();
            }

            if (score.GetCurrentScore() == 200)
            {
                OnGameFinished();
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (!pcPlayer.GetTurretState()) {
                pcPlayer.switchTurretNumber(1);
                Debug.Log("→キーが押されました。");
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (!pcPlayer.GetTurretState()) {
                pcPlayer.switchTurretNumber(-1);
                Debug.Log("←キーが押されました。");
            }
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            handleButtonPressed(true);
            Debug.Log("スペースキーが押されました。");
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            handleButtonPressed(false);
            Debug.Log("スペースキーが離されました。");
        }
        float remainingTime = getRemainingTime();
        remainingTime = remainingTime < 0 ? 0 : remainingTime;
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        for (int i = 0; i < canons.Length; i++) {
            fluidSimulation.powers[i] = canons[i].power;
        }

        // スコアを表示
        int nowRingCount = score.GetCurrentScore() / 10;
        if (_prevRingCount < nowRingCount)
        {
            IncreaseScoreText(nowRingCount);
        }
        else if (nowRingCount < _prevRingCount)
        {
            DecreaseScoreCount(nowRingCount);
        }
        _prevRingCount = nowRingCount;
    }

    private int _prevRingCount = 0;

    private void DecreaseScoreCount(int count)
    {
        scoreText.text = $"{count} ";
        ringSumText.text = "/ " + RingNum.ToString();
    }

    private void IncreaseScoreText(int count)
    {
        scoreText.transform.DOComplete();
        scoreText.transform.DOKill();

        scoreText.text = $"{count} ";
        ringSumText.text = "/ " + RingNum.ToString();

        scoreText.transform.localScale = Vector3.one;
        scoreText.transform.DOScale(1.5f, 0.2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                scoreText.transform.DOScale(1f, 0.2f)
                    .SetEase(Ease.InQuad);
            });
    }

    public void handleButtonPressed(bool isPressed)
    {
        var previousState = pcPlayer.GetTurretState();
        if (previousState == isPressed) { return; }
        Debug.Log("handleButtonPressed: " + 9 + " " + isPressed);
        pcPlayer.setTurretState(isPressed);
        var canonId = pcPlayer.GetCurrentTurretNumber();
        if (isPressed)
        {
            canons[canonId].addPower(Vector3.up * fixedFluidPower);
        }
        else
        {
            canons[canonId].addPower(-Vector3.up * fixedFluidPower);
        }
    }

    public void handleButtonPressed(int playerId, bool isPressed) {
        var previousState = players[playerId].GetTurretState();
        if (previousState == isPressed) { return; }
        Debug.Log("handleButtonPressed: " + playerId + " " + isPressed);
        players[playerId].setTurretState(isPressed);
        var canonId = players[playerId].GetCurrentTurretNumber();
        if (isPressed) {
            canons[canonId].addPower(Vector3.up * fixedFluidPower);
        } else {
            canons[canonId].addPower(-Vector3.up * fixedFluidPower);
        }
    }

    public float getRemainingTime() {
        return limitTime - timer.GetCurrentTime();
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
            
            // x座標を-22から22までランダムに配置
            float randomX = Random.Range(-22f, 22f);
            prefab.transform.position = new Vector3(randomX, prefab.transform.position.y, prefab.transform.position.z);
            
            // プレハブのマテリアルの色をランダムに設定
            Renderer renderer = prefab.GetComponent<Renderer>();
            Debug.Log(renderer);
            if (renderer != null)
            {
                Material material = renderer.material;
                Color[] colors = new Color[]
                {
                    new Color(0.043f, 0.478f, 0.816f),
                    new Color(1f, 0.094f, 0.094f),
                    new Color(0.071f, 0.694f, 0f),
                    new Color(1f, 0.922f, 0.231f),
                    new Color(0.612f, 0.157f, 0.690f)
                };
                int colorIndex = i % colors.Length;
                Color selectedColor = colors[colorIndex];
                material.color = selectedColor;
                
                // エミッションの設定
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", selectedColor * 0.8f);
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
}
