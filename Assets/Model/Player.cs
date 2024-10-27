using UnityEngine;

class Player
{
    // 砲台の状態を管理する変数
    private bool isPushButton = false;
    
    // 現在選択されている砲台番号（0-4）
    private int currentTurretNumber = 0;

    // プレイヤーのID(0 - 9)
    private int playerId;

    public Player(int id) {
        isPushButton = false;
        currentTurretNumber = 0;
        this.playerId = id;
    }
    // 砲台番号を切り替える関数
    public void switchTurretNumber(int newNumber)
    {
        currentTurretNumber = (currentTurretNumber + newNumber) % 5;
        Debug.Log("switchTurretNumber: " + currentTurretNumber);
    }

    // 砲台のオン・オフを切り替える関数
    public void setTurretState(bool state)
    {
        Debug.Log("setTurretState: " + state);
        isPushButton = state;
    }

    // 砲台の状態を取得する関数
    public bool GetTurretState()
    {
        return isPushButton;
    }

    // 現在選択されている砲台番号を取得する関数
    public int GetCurrentTurretNumber()
    {
        return currentTurretNumber;
    }

    public int GetPlayerId() {
        return playerId;
    }
}
