using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QRCoder;

[RequireComponent(typeof(Image))]
public class QRcodePanel : MonoBehaviour
{
    [SerializeField]
    private Image _qRImage;

    private int _qrTextureW = 512;
    private int _qrTextureH = 512;

    public void GenerateQRCode(string text)
    {
        // QRCoder���C�u������QRCodeGenerator���g����QR�R�[�h�𐶐�
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);

        // �o�C�g�z���QR�R�[�h�摜���擾
        byte[] qrCodeBytes = qrCode.GetGraphic(20);

        // �o�C�g�z�񂩂�Texture2D�𐶐�
        Texture2D qrTexture = new Texture2D(_qrTextureW, _qrTextureH);
        qrTexture.LoadImage(qrCodeBytes);

        // Unity��RawImage�ɕ\��
        Sprite qrSprite = Sprite.Create(qrTexture, new Rect(0, 0, qrTexture.width, qrTexture.height), new Vector2(0.5f, 0.5f));
        _qRImage.sprite = qrSprite;
    }
}