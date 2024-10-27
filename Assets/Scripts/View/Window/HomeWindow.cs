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
            CreateRoomResponse response = await client.CreateRoomAsync();
            _windowManager.Activate(_windowManager.ParticipantRegistrationWindow, _windowManager.HomeWindow);
            Debug.Log(response);
            _windowManager.ParticipantRegistrationWindow.OpenRoom(response.RoomId, response.RoomUrl, client);
        });
    }
}
