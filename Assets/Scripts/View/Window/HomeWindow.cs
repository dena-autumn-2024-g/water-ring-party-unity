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
            _windowManager.ParticipantRegistrationWindow.OpenRoom(response.RoomId, response.RoomUrl, client);
        });


        // Ç±Ç±Ç≈ê⁄ë±ÉeÉXÉg
        Request();
    }

    private const string URL = "https://wrp.mazrean.com";

    private void Request()
    {
        using var handler = new YetAnotherHttpHandler();
        using var channel = GrpcChannel.ForAddress(URL, new GrpcChannelOptions
        {
            HttpHandler = handler,
            DisposeHttpClient = true,
        });
        var client = new RoomService.RoomServiceClient(channel);
        var reply = client.CreateRoom(new CreateRoomRequest());// .SayHello(new HelloRequest { Name = "UnityClient" });
    }
}
