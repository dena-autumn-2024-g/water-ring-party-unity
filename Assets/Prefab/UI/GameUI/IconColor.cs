using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconColor : MonoBehaviour
{
    public int colorId;
    private static readonly Color[] colors = new Color[]
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.cyan,
        Color.magenta,
        Color.white,
        Color.black,
        new Color(1f, 0.5f, 0f), // オレンジ
        new Color(0.5f, 0f, 0.5f) // パープル
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
