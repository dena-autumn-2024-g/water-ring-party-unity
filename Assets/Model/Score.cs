using System;

public class Score
{
    private int currentScore;

    public Score()
    {
        currentScore = 0;
    }

    public void AddPoints(int points)
    {
        currentScore += points;
    }
    public void ResetScore()
    {
        currentScore = 0;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public override string ToString()
    {
        return $"現在のスコア: {currentScore}";
    }
}

