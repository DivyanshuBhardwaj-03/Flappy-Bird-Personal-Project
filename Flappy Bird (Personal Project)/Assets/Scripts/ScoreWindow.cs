using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScoreWindow : MonoBehaviour
{
    private Text scoreText;
    private Text highScoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highScoreText = transform.Find("highScoreText").GetComponent<Text>();
    }

    private void Start()
    {
        highScoreText.text = "Highscore: " + Score.GetHighScore().ToString();
        Bird.GetInstance().OnDied += ScoreWindow_OnDied;
        Bird.GetInstance().OnStartedPlaying += ScoreWindow_OnStartedPlaying;
    }

    private void ScoreWindow_OnStartedPlaying(object sender, EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void ScoreWindow_OnDied(object sender, EventArgs e)
    {
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        scoreText.text = Level.GetInstance().GetsPipesPassedCount().ToString();

    }
}
