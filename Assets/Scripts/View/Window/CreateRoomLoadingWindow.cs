using System.Collections;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using DG.Tweening;

public class CreateRoomLoadingWindow : Window
{
    [SerializeField] private TextMeshProUGUI _loadingText;
    private Sequence _sequence;

    public void StartLoadingTextAnimation()
    {
        _sequence = DOTween.Sequence();

        _sequence.AppendCallback(() => _loadingText.text = "通信中")
                .AppendInterval(0.1f)
                .AppendCallback(() => _loadingText.text = "通信中.")
                .AppendInterval(0.1f)
                .AppendCallback(() => _loadingText.text = "通信中..")
                .AppendInterval(0.1f)
                .AppendCallback(() => _loadingText.text = "通信中...")
                .AppendInterval(0.1f)
                .SetLoops(-1, LoopType.Restart);
    }

    public void StopLoadingTextAnimation()
    {
        _sequence?.Kill();
        _sequence = null;
    }
}