using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BasePanel : MonoBehaviour
{
    [SerializeField]
    private Image _image;

    public Image Image { get { return _image; } }
}