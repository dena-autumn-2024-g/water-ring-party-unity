using UnityEngine;
using UnityEngine.UI;

public class RingScoreWindow : MonoBehaviour
{
    // Text コンポーネントの参照
    public Text scoreText;

    void Start()
    {
        int score = CommonInfoManager.SCORE_RING_COUNT;
        scoreText.text = (score/10).ToString() + " rings";
    }

}
