using System;
using System.Collections.Generic;

public class Score
{
    private List<int> _teamScores;
    public IReadOnlyList<int> TeamScores { get { return _teamScores; } }

    public Score()
    {
        _teamScores = new List<int>();
        for(int i = 0; i < CommonInfoManager.NUM_PLAYER; i++)
        {
            _teamScores.Add(0);
        }
    }

    public void AddPoints(int points, int teamId)
    {
        _teamScores[teamId] += points;
    }

    public int GetCurrentScore(int teamId)
    {
        return TeamScores[teamId];
    }
}

