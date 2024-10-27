using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconColor : MonoBehaviour
{
    public int colorId;
    private static readonly Color[] colors = new Color[]
    {
        // Hexで色を指定
        // 0B7AD0
        new Color(0.043f, 0.478f, 0.816f),
        // FF1818
        new Color(1f, 0.094f, 0.094f),
        // 12B100
        new Color(0.071f, 0.694f, 0f),
        // FFEB3B
        new Color(1f, 0.922f, 0.231f),
        // 9C27B0
        new Color(0.612f, 0.157f, 0.690f),
        // 353535
        new Color(0.208f, 0.208f, 0.208f),
        // 24DCCA
        new Color(0.141f, 0.863f, 0.792f),
        // FF4DA9
        new Color(1f, 0.302f, 0.663f),
        // FF8000
        new Color(1f, 0.502f, 0f),
        // FFFFFF
        new Color(1f, 1f, 1f),
        // 000000
        new Color(0f, 0f, 0f),

    };

    private Image iconImage;

    private void Awake()
    {
        iconImage = GetComponent<Image>();
        SetColorById();
    }

    private void SetColorById()
    {
        if (colorId < 0 || colorId >= colors.Length)
        {
            Debug.LogWarning("無効なカラーID: " + colorId + "。ランダムな色を割り当てます。");
            colorId = Random.Range(0, colors.Length);
        }
        if (iconImage != null)
        {
            iconImage.color = colors[colorId];
        }
        else
        {
            Debug.LogError("Imageコンポーネントが見つかりません。");
        }
    }

    public void setColorId(int id)
    {
        colorId = id;
        SetColorById();
    }
}
