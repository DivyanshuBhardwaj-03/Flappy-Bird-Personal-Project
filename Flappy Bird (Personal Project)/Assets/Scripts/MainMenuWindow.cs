using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class MainMenuWindow : MonoBehaviour
{
    private void Awake()
    {
        transform.Find("playButton").GetComponent<Button_UI>().ClickFunc = () =>
        {
            Loader.Load(Loader.Scene.GameScene); ;
        };
        transform.Find("playButton").GetComponent<Button_UI>().AddButtonSounds();
        transform.Find("quitButton").GetComponent<Button_UI>().ClickFunc = () =>
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
        };
        transform.Find("quitButton").GetComponent<Button_UI>().AddButtonSounds();
    }

}
