using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerResultPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _playerScoreText;

    [SerializeField]
    private TextMeshProUGUI _playerIdText;

    [SerializeField]
    private Image _playerIdPanel;

    [SerializeField]
    private GameObject _pollRingResultModelPrefab;

    [SerializeField]
    private RawImage _rowImage;

    private PollRingResultModelController _modelController;

    private int _playerId;
    private int _ringCount;

    public void Init(int playerId)
    {
        _ringCount = 0;
        _playerScoreText.text = $"{_ringCount}";
        _playerId = playerId;
        _playerIdText.text = $"{playerId+1}P";
        _playerIdPanel.color = IconColor.Colors[playerId];
        _modelController = Instantiate(_pollRingResultModelPrefab).GetComponent<PollRingResultModelController>();
        _rowImage.texture = _modelController.RenderTexture;
    }

    public void AddRing()
    {
        SetScoreText(++_ringCount);
        _modelController.AddRing(_playerId);
    }

    public void SetScoreText(int count)
    {
        _playerScoreText.transform.DOComplete();
        _playerScoreText.transform.DOKill();

        _playerScoreText.text = $"{_ringCount}";

        _playerScoreText.transform.localScale = Vector3.one;
        _playerScoreText.transform.DOScale(1.5f, 0.2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _playerScoreText.transform.DOScale(1f, 0.2f)
                    .SetEase(Ease.InQuad);
            });
    }
}
