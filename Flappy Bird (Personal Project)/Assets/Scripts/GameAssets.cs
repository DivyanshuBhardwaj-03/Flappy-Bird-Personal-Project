using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    // private static GameAssets AssetInstance;
    private static GameAssets Instance;

    public static GameAssets GetAssetInstance()
    {
        return Instance;
    }
    private void Awake()
    {
        /*  if (AssetInstance != null)
          {
              Destroy(gameObject);

          }
          AssetInstance = this;
          DontDestroyOnLoad(gameObject);*/

        Instance = this;
    }



    public Sprite pipeHeadSprite;
    public Transform pfPipeHeadSprite;
    public Transform pfPipeBodySprite;
    public Transform pfGroundSprite;
    public Transform pfCloud_1;
    public Transform pfCloud_2;
    public Transform pfCloud_3;

    public SoundAudioClip[] soundAudioClipArray;

    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
}


