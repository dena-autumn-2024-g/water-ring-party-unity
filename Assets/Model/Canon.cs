using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

class Canon {
    public Vector3 Power = Vector3.zero;
    public int id;
    private float MaxPower = 0.075f;

    private List<Tweener> _tweenersForPlayer;
    private List<Vector3> _addedPowersForPlayer;
    
    public Canon(int canonId) {
        this.id = canonId;
        _tweenersForPlayer = new List<Tweener>();
        _addedPowersForPlayer = new List<Vector3>();
        for(int i = 0; i < CommonInfoManager.NUM_PLAYER; i++)
        {
            _tweenersForPlayer.Add(null);
            _addedPowersForPlayer.Add(Vector3.zero);
        }
    }

    public void SetPower(Vector3 power) {
        this.Power = power;
    }

    private Vector3 AddDiffPower(Vector3 diffPower)
    {
        float diffX = MaxPower < Power.x + diffPower.x ? MaxPower - Power.x : diffPower.x;
        float diffY = MaxPower < Power.y + diffPower.y ? MaxPower - Power.y : diffPower.y;
        float diffZ = MaxPower < Power.z + diffPower.z ? MaxPower - Power.z : diffPower.z;
        Vector3 realDiff = new Vector3(diffX, diffY, diffZ);
        Power += realDiff;
        return realDiff;
    }

    public void AddPower(Vector3 power, int playerId)
    {
        if (_tweenersForPlayer[playerId] != null)
        {
            _tweenersForPlayer[playerId].Kill();
            AddDiffPower(-_addedPowersForPlayer[playerId]);
            _addedPowersForPlayer[playerId] = Vector3.zero;
        }

        Vector3 added = Vector3.zero;
        _tweenersForPlayer[playerId] = DOTween.To(() => 0f, value =>
        {
            var realPower = AddDiffPower(power * value);
            _addedPowersForPlayer[playerId] += realPower; 
        }, 1f, 0.5f)
        .SetEase(Ease.InQuad)
        .OnComplete(() =>
        {
            AddDiffPower(-_addedPowersForPlayer[playerId]);
            _addedPowersForPlayer[playerId] = Vector3.zero;
        });
    }

    public void CancelPower(int playerId)
    {
        if (_tweenersForPlayer[playerId] != null)
        {
            _tweenersForPlayer[playerId].Kill();
            AddDiffPower(-_addedPowersForPlayer[playerId]);
            _addedPowersForPlayer[playerId] = Vector3.zero;
        }
    }
}