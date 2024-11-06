using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Linq;
using DG.Tweening;

public class ScoreWindow : MonoBehaviour
{
    [SerializeField]
    private PlayerResultPanel _playerResultPanelPrefab;

    [SerializeField]
    private GameObject _playerResultPanelParent;

    [SerializeField]
    private GameObject _buttons;

    [SerializeField]
    private TextMeshProUGUI _winnerText;

    private List<PlayerResultPanel> _playerResultPanels;

    public async void Awake()
    {
        Init();

        await Task.Delay(1000);

        int maxCount = 0;
        var scores = CommonInfoManager.SCORE_RING_COUNT;
        foreach (var score in scores)
        {
            maxCount = Mathf.Max(maxCount, score);
        }

        for (int i = 0; i < maxCount; i++)
        {
            for (int j = 0; j < CommonInfoManager.NUM_PLAYER; j++)
            {
                if (i < scores[j])
                {
                    _playerResultPanels[j].AddRing();
                }
            }
            await Task.Delay(1000);
        }

        SetWinnerText();
        _winnerText.transform.DOScale(1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);

        await Task.Delay(500);
        _buttons.gameObject.SetActive(true);
    }

    private void Init()
    {
        _buttons.SetActive(false);

        _playerResultPanels = new List<PlayerResultPanel>();
        for (int i = 0; i < CommonInfoManager.NUM_PLAYER; i++)
        {
            PlayerResultPanel panel = Instantiate(_playerResultPanelPrefab, _playerResultPanelParent.transform).GetComponent<PlayerResultPanel>();
            panel.Init(i);
            _playerResultPanels.Add(panel);
        }
    }

    private void SetWinnerText()
    {
        int[] scores = CommonInfoManager.SCORE_RING_COUNT;
        int maxScore = 0;
        // óDèüé“ÇÃindexÇÃÉäÉXÉg
        List<int> winnerList = new List<int>();

        for (int i = 0; i < CommonInfoManager.NUM_PLAYER; i++)
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

        string displayText;
        bool isDraw = winnerList.Count == CommonInfoManager.NUM_PLAYER || winnerList.Count == 0;

        if (isDraw)
        {
            displayText = "à¯Ç´ï™ÇØ";
        }
        else
        {
            displayText = "óDèüÅFPlayer" + string.Join(",", winnerList.Select(p => (p + 1).ToString()));
        }

        _winnerText.text = displayText;
    }
}
