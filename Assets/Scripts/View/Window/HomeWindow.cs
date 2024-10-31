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
        var client = new WaterRingStreamClient();
        _createRoomButton.Observable.Subscribe(async (_) =>
        {
            var homeWindow = _windowManager.HomeWindow;
            var createRoomWindow = _windowManager.CreateRoomLoadingWindow;
            //_windowManager.Activate(createRoomWindow, _windowManager.HomeWindow);
            createRoomWindow.gameObject.SetActive(true);
            createRoomWindow.StartLoadingTextAnimation();
            CreateRoomResponse response = await client.CreateRoomAsync();
            _windowManager.Activate(_windowManager.ParticipantRegistrationWindow, createRoomWindow);
            homeWindow.gameObject.SetActive(false);
            createRoomWindow.StopLoadingTextAnimation();
            CommonInfoManager.ROOM_ID = response.RoomId;
            _windowManager.ParticipantRegistrationWindow.OpenRoom(response.RoomId, response.RoomUrl, client);
        });
    }
}
