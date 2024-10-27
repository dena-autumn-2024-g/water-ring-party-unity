using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class QRcodePanel: MonoBehaviour
{
    [SerializeField]
    private Image _qRImage;
    private Texture2D _encodedQRTextire;

    private int _qrTextureW = 512;
    private int _qrTextureH = 512;

    private void Awake()
    {
        string url = "https://github.com/dena-autumn-2024-g/protobuf/tree/main";
        GenerateQRCode(url);
    }

    public void GenerateQRCode(string url)
    {
        //新規の空のテクスチャを作成
        _encodedQRTextire = new Texture2D(_qrTextureW, _qrTextureH);

        //エンコード処理
        var color32 = Encode(url, _encodedQRTextire.width, _encodedQRTextire.height);
        _encodedQRTextire.SetPixels32(color32);
        _encodedQRTextire.Apply();

        _qRImage.sprite = Sprite.Create(_encodedQRTextire, new Rect(0, 0, _qrTextureW, _qrTextureH), Vector2.zero);

    }


    private Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,

            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }
}