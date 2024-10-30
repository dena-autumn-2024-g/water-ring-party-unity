using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TotalScoreWindow : MonoBehaviour
{
    // Text コンポーネントの参照
    public TextMeshProUGUI scoreText;

    void Start()
    {
        int score = CommonInfoManager.SCORE_POINT;
        scoreText.text = score.ToString() + "点!";
    }

}
