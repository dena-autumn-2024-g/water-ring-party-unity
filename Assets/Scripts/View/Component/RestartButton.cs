using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{

    // シーン名を指定
    public string sceneToLoad;

    // ボタンが押されたときに呼び出されるメソッド
    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }


}