using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeScoreWindow : MonoBehaviour
{
    // Text コンポーネントの参照
    public TextMeshProUGUI scoreText;

    void Start()
    {
        int score = CommonInfoManager.SCORE_LEFT_TIME;
        scoreText.text = score.ToString() + " 秒";
    }

}