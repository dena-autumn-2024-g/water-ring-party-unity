using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RingScoreWindow : MonoBehaviour
{
    // Text コンポーネントの参照
    public TextMeshProUGUI scoreText;

    void Start()
    {
        //int score = CommonInfoManager.SCORE_RING_COUNT;
        //scoreText.text = (score/10).ToString() + " 個";
    }

}
