using UnityEngine;
using UnityEngine.UI;

public class TimeScoreWindow : MonoBehaviour
{
    // 表示したい変数
    public int score;
    // Text コンポーネントの参照
    public Text scoreText;

    void Start()
    {
        scoreText.text = score.ToString() + " seconds";
    }

}
