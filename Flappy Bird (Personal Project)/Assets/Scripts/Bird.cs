using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEditorInternal;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private const float JUMP_FLOAT = 10f;

    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;

    private Rigidbody2D birdRigidBody2D;
    //private static Bird birdInstance;
    private static Bird Instance;
    private State state;
    public static Bird GetInstance()
    {
        return Instance;
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        Dead,
    }
    private void Awake()
    {   /*if (birdInstance != null)
        {
            Destroy(gameObject);
            
        }
        birdInstance = this;*/
        Instance = this;
        // DontDestroyOnLoad(gameObject);
        birdRigidBody2D = GetComponent<Rigidbody2D>();
        birdRigidBody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;
    }
    private void Update()
    {
        switch (state)
        {
            default:
            case State.WaitingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    state = State.Playing;
                    birdRigidBody2D.bodyType = RigidbodyType2D.Dynamic;
                    Jump();
                    if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
                }
                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    Jump();
                }
                break;
            case State.Dead:
                break;

        }
        /*if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Jump();
        }*/
    }

    private void Jump()
    {
        birdRigidBody2D.velocity = Vector2.up * JUMP_FLOAT;
        SoundManager.PlaySound(SoundManager.Sound.BirdJump);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        birdRigidBody2D.bodyType = RigidbodyType2D.Static;
        SoundManager.PlaySound(SoundManager.Sound.Lose);
        if (OnDied != null) OnDied(this, EventArgs.Empty);
    }


}
