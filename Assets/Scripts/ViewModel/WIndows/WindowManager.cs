using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public HomeWindow HomeWindow;

    public ParticipantRegistrationWindow ParticipantRegistrationWindow;

    public Window CurWindow { get; private set; }

    public void Awake()
    {
        GameStart();
    }

    private void GameStart()
    {
        HomeWindow.gameObject.SetActive(false);
        ParticipantRegistrationWindow.gameObject.SetActive(false);

        if (CommonInfoManager.END_GAME)
        {
            Activate(ParticipantRegistrationWindow, null);
        }
        else
        {
            Activate(HomeWindow, null);
        }
    }

    public void Activate(Window openWindow, Window closeWindow)
    {
        openWindow.OnActivateWindow();
        closeWindow?.OnDeactivateWindow();
        CurWindow = openWindow;
    }
}