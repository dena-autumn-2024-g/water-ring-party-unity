using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterRing;
using UniRx;
using UnityEngine.SceneManagement;
using TMPro;

public class ParticipantRegistrationWindow : Window
{
    [SerializeField]
    private StartGameButton _startGameButton;

    [SerializeField]
    private BaseButton _userResetButton;

    [SerializeField]
    private QRcodePanel _qRcodePanel;
    [SerializeField]
    private TextMeshProUGUI _qRErrorText;

    [SerializeField]
    private UserCounter _userCounter;

    public void Awake()
    {
        CommonInfoManager.CLIENT = new();
        _startGameButton.Observable.Subscribe((_) =>
        {
            CommonInfoManager.CLIENT?.StopStream();
            // �V�[���ړ� + client.StartGame
            SceneManager.LoadScene("SampleScene");
        });

        _userResetButton.Observable.Subscribe(async (_) =>
        {
            CommonInfoManager.CLIENT?.StopStream();
            await CommonInfoManager.CLIENT.CloseRoomAsync(CommonInfoManager.ROOM_ID);

            CommonInfoManager.CLIENT = new();
            CreateRoomResponse response = await CommonInfoManager.CLIENT.CreateRoomAsync();
            CommonInfoManager.ROOM_ID = response.RoomId;
            CommonInfoManager.ROOM_URL = response.RoomUrl;
            CommonInfoManager.NUM_PLAYER = 0;
            OpenRoom();
        });

        if (CommonInfoManager.END_GAME)
        {
            OpenRoom();
        }
    }

    public void OpenRoom()
    {
        Debug.Log($"�������J���܂��B�v���C���[��: {CommonInfoManager.NUM_PLAYER}");
        _qRcodePanel.GenerateQRCode(CommonInfoManager.ROOM_URL);
        if (CommonInfoManager.ROOM_URL == null)
        {
            _qRErrorText.gameObject.SetActive(true);
        }
        else
        {
            _qRErrorText.gameObject.SetActive(false);
        }

        _userCounter.SetUser(CommonInfoManager.NUM_PLAYER);
        if (CommonInfoManager.NUM_PLAYER > 0)
        {
            _startGameButton.SetInteractable(true);
        }

        CommonInfoManager.CLIENT.OnUserJoined += OnUserJoinedHandler;
        CommonInfoManager.CLIENT.WaitForUserJoin(CommonInfoManager.ROOM_ID);
    }

    private void OnUserJoinedHandler(WaitForUserJoinResponse response)
    {
        Debug.Log($"���[�U�[���Q�����܂����B���[�U�[ID: {response.UserId}");
        CommonInfoManager.NUM_PLAYER++;

        _startGameButton.SetInteractable(true);
        Debug.Log($"�v���C���[��: {CommonInfoManager.NUM_PLAYER}");
        _userCounter.SetUser(CommonInfoManager.NUM_PLAYER);
    }
}
