using UnityEngine;
using UnityEngine.UI;

public class TotalScoreWindow : MonoBehaviour
{
    // Text コンポーネントの参照
    public Text scoreText;

    void Start()
    {
        int score = CommonInfoManager.SCORE_POINT;
        scoreText.text = score.ToString() + " points!";
    }

}
