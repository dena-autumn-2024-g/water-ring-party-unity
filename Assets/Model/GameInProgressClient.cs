using System.Collections;
using UnityEngine;
using WaterRing;

public class GameInProgressClient : MonoBehaviour
{
    [SerializeField]
    private GameManager _gameManager;
 

    public void Start()
    {
        var client = new WaterRingStreamClient(); // CommonInfoManager.CLIENT;
        var roomId = CommonInfoManager.ROOM_ID;

        client?.StartGame(roomId);
        client.OnGameEvent += OnGameEventReceived;
    }

    public void OnGameEventReceived(StartGameStreamResponse response)
    {
        int playerId;
        switch (response.EventCase)
        {
            case StartGameStreamResponse.EventOneofCase.MoveButton:
                playerId = response.MoveButton.UserId;
                if (response.MoveButton.Direction == Direction.Left)
                {
                    _gameManager.MoveLeft(playerId);
                }
                else
                {
                    _gameManager.MoveRight(playerId);
                }
                break;
            case StartGameStreamResponse.EventOneofCase.PushButtonPressed:
                playerId = response.PushButtonPressed.UserId;
                _gameManager.PushButtonPressed(playerId);
                break;
            case StartGameStreamResponse.EventOneofCase.PushButtonReleased:
                playerId = response.PushButtonReleased.UserId;
                _gameManager.PushButtonReleased(playerId);
                break;
        }
    }
}