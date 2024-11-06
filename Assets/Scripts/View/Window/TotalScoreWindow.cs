using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TotalScoreWindow : MonoBehaviour
{
    // Text コンポーネントの参照
    public TextMeshProUGUI scoreText;

    void Start()
    {
        /*int[] scores = CommonInfoManager.SCORE_RING_COUNT;
        int maxScore = 0;
        // 優勝者のindexのリスト
        List<int> winnerList = new List<int>();

        for (int i = 1; i < 10; i++)
        {
            if (scores[i] > maxScore)
            {
                maxScore = scores[i];
                winnerList.Clear();
                winnerList.Add(i);
            }
            else if (scores[i] == maxScore)
            {
                winnerList.Add(i);
            }
        }

        string displayText = "";
        bool isDraw = winnerList.Count == CommonInfoManager.NUM_PLAYER || winnerList.Count == 0;

        if (isDraw)
        {
            displayText += "引き分け";
        }
        else
        {
            displayText += "優勝：Player";
            for (int i = 0; i < winnerList.Count; i++)
            {
                displayText += (winnerList[i] + 1).ToString();
                if (i < winnerList.Count - 1)
                {
                    displayText += ",";
                }
            }
        }

        scoreText.text = displayText;*/
    }

}
