using UnityEngine;
using DG.Tweening;

public class SlideInPanel : MonoBehaviour
{
    [SerializeField]
    public RectTransform panel;
    public float duration = 0.5f;
    public Vector2 startOffset = new Vector2(-3840, 0);
    public Vector2 firstOffset = new Vector2(-1920, 0);
    public Vector2 secondOffset = new Vector2(0, 0);
    public Vector2 finalOffset = new Vector2(1920, 0);

    void Start()
    {
        panel.anchoredPosition = startOffset;
    }

    public void SlideIn()
    {
        panel.DOAnchorPos(firstOffset, duration).SetEase(Ease.OutCubic);

        // 1秒後に2回目のアニメーションを実行
        DOVirtual.DelayedCall(1.0f, () =>
        {
            panel.DOAnchorPos(secondOffset, duration).SetEase(Ease.OutCubic);
        });

        // 2秒後に3回目のアニメーションを実行
        DOVirtual.DelayedCall(2.0f, () =>
        {
            panel.DOAnchorPos(finalOffset, duration).SetEase(Ease.OutCubic);
        });
    }
}
