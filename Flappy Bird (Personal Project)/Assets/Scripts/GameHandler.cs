using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("GameHandler Start");

        // GameObject gameObject = new GameObject("Pipe", typeof(SpriteRenderer));
        // gameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.GetAssetInstance().pipeHeadSprite;
        //Score.TrySetNewHighScore(20);
        Score.Start();
    }


}
