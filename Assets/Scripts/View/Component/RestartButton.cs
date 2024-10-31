using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public string RestartSceneName;
    public string HomeSceneName;

    public void ChangeToRestartScene()
    {
        CommonInfoManager.END_GAME = true;
        SceneManager.LoadScene(HomeSceneName);
    }

    public void ChangeToHomeScene()
    {
        CommonInfoManager.END_GAME = false;
        SceneManager.LoadScene(HomeSceneName);

        var client = new WaterRingStreamClient();
        var roomId = CommonInfoManager.ROOM_ID;

        client?.CloseRoomAsync(roomId);
    }
}