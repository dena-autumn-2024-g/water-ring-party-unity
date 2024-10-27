using UnityEngine;
using UnityEngine.UI;

public class TimeScoreWindow : MonoBehaviour
{
    // Text コンポーネントの参照
    public Text scoreText;

    void Start()
    {
        int score = CommonInfoManager.SCORE_LEFT_TIME;
        scoreText.text = score.ToString() + " seconds";
    }

}