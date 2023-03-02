using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score
{
    public static void Start()
    {
        // ResetHighScore();
        Bird.GetInstance().OnDied += Bird_OnDied;
    }

    private static void Bird_OnDied(object sender, System.EventArgs e)
    {
        // throw new System.NotImplementedException();
        TrySetNewHighScore(Level.GetInstance().GetsPipesPassedCount());
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt("highScore");
    }

    public static bool TrySetNewHighScore(int score)
    {
        int currentHighScore = GetHighScore();
        if (score > currentHighScore)
        {
            //New HighScore
            PlayerPrefs.SetInt("highScore", score);
            PlayerPrefs.Save();
            return true;
        }
        else
        {
            return false;
        }

    }

    public static void ResetHighScore()
    {
        PlayerPrefs.SetInt("highScore", 0);
        PlayerPrefs.Save();
    }
}
