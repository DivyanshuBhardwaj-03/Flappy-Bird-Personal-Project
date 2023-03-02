using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class GameOverWindow : MonoBehaviour
{
    private Text scoreText;
    private Text highScoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highScoreText = transform.Find("highScoreText").GetComponent<Text>();
        transform.Find("retryButton").GetComponent<Button_UI>().ClickFunc = () =>
        {
            Loader.Load(Loader.Scene.GameScene);
        };
        transform.Find("retryButton").GetComponent<Button_UI>().AddButtonSounds();
        transform.Find("mainMenuButton").GetComponent<Button_UI>().ClickFunc = () =>
        {
            Loader.Load(Loader.Scene.MainMenu);
        };
        transform.Find("mainMenuButton").GetComponent<Button_UI>().AddButtonSounds();
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
        Hide();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Retry
            Loader.Load(Loader.Scene.GameScene);
        }
    }

    private void Bird_OnDied(object sender, System.EventArgs e)
    {
        // throw new System.NotImplementedException();
        scoreText.text = Level.GetInstance().GetsPipesPassedCount().ToString();

        if (Level.GetInstance().GetsPipesPassedCount() >= Score.GetHighScore())
        {
            //New HighScore
            highScoreText.text = "NEW HIGHSCORE";
        }
        else
        {
            highScoreText.text = "HIGHSCORE: " + Score.GetHighScore();
        }
        Show();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}

