using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QRCoder;

[RequireComponent(typeof(Image))]
public class QRcodePanel: MonoBehaviour
{
    [SerializeField]
    private Image _qRImage;

    private int _qrTextureW = 512;
    private int _qrTextureH = 512;

    private void Awake()
    {
        string url = "https://github.com/dena-autumn-2024-g/protobuf/tree/main";
        GenerateQRCode(url);
    }

    public void GenerateQRCode(string text)
    {
        // QRCoderライブラリのQRCodeGeneratorを使ってQRコードを生成
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);

        // バイト配列でQRコード画像を取得
        byte[] qrCodeBytes = qrCode.GetGraphic(20);

        // バイト配列からTexture2Dを生成
        Texture2D qrTexture = new Texture2D(_qrTextureW, _qrTextureH);
        qrTexture.LoadImage(qrCodeBytes);

        // UnityのRawImageに表示
        Sprite qrSprite = Sprite.Create(qrTexture, new Rect(0, 0, qrTexture.width, qrTexture.height), new Vector2(0.5f, 0.5f));
        _qRImage.sprite = qrSprite;
    }
}