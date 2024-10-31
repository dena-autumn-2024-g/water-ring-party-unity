using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using WaterRing;
using Cysharp.Net.Http;
using Grpc.Net.Client;

public class HomeWindow : Window
{
    [SerializeField]
    private BaseButton _createRoomButton;

    [SerializeField]
    private WindowManager _windowManager;

    public void Awake()
    {
        _createRoomButton.Observable.Subscribe(async (_) =>
        {
            CreateRoomResponse response = await CommonInfoManager.CLIENT.CreateRoomAsync();
            CommonInfoManager.ROOM_ID = response.RoomId;
            CommonInfoManager.ROOM_URL = response.RoomUrl;

            _windowManager.ParticipantRegistrationWindow.OpenRoom();

            _windowManager.Activate(_windowManager.ParticipantRegistrationWindow, _windowManager.HomeWindow);
        });
    }
}
