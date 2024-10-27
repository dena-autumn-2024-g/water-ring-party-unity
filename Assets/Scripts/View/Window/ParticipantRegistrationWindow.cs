using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterRing;
using UniRx;
using UnityEngine.SceneManagement;

public class ParticipantRegistrationWindow : Window
{
    [SerializeField]
    private BaseButton _startGameButton;

    [SerializeField]
    private QRcodePanel _qRcodePanel;

    private string _roomId;
    private WaterRingStreamClient _client;

    private int _numPeople = 0;

    public void Awake()
    {
        _startGameButton.Observable.Subscribe((_) =>
        {
            _client?.StopStream();
            // �V�[���ړ� + client.StartGame
            SceneManager.LoadScene("SampleScene");
            _client?.StartGame(_roomId);
        });
    }

    public void OpenRoom(string roomId, string roomURL, WaterRingStreamClient client)
    {
        _roomId = roomId;
        _qRcodePanel.GenerateQRCode(roomURL);
        _numPeople = 0;

        client.OnUserJoined += OnUserJoinedHandler;
        client.WaitForUserJoin(roomId);
    }

    private void OnUserJoinedHandler(WaitForUserJoinResponse response)
    {
        Debug.Log($"���[�U�[���Q�����܂����B���[�U�[ID: {response.UserId}");
        _numPeople += 1;
        Debug.Log($"�l��{_numPeople}");
    }
}
