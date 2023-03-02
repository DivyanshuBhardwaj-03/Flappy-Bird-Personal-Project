using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using CodeMonkey;
using CodeMonkey.Utils;

public class Level : MonoBehaviour
{
    private const float PIPE_BODY_WIDTH = 7f;
    private const float PIPE_HEAD_HEIGHT = 6.43f;
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_MOVE_SPEED = 10f;
    private const float PIPE_DESTROY_X_POSTION = -100f;
    private const float PIPE_SPAWN_X_POSTION = +75f;
    private const float GROUND_DESTROY_X_POSTION = -200f;
    //private const float GROUND_SPAWN_X_POSTION = +75f;
    private const float CLOUD_DESTROY_X_POSTION = -120f;
    private const float CLOUD_SPAWN_X_POSTION = +120f;
    private const float CLOUD_SPAWN_Y_POSTION = +35f;
    private const float BIRD_X_POSITION = 0f;
    private int pipesPassedCount;
    // private static Level levelInstance;
    private static Level Instance;

    private List<Transform> groundList;
    private List<Transform> cloudList;
    private float cloudSpawnTimer;
    private float cloudSpawnTimerMax;
    private List<Pipe> pipeList;
    private int pipesSpawned;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;
    private State state;

    public static Level GetInstance()
    {
        return Instance;
    }
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        BirdDead,
    }
    private void Awake()
    {
        /*if (levelInstance != null)
        {
            Destroy(gameObject);
         
        }
        levelInstance = this;
        DontDestroyOnLoad(gameObject);*/
        Instance = this;

        SpawnInitialClouds();
        SpawnInitialGround();
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 2.0f;
        gapSize = 50f;
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
    }


    private void Start()
    {
        // CreatePipe(50f, 20f, true);
        //CreatePipe(50f, 20f, false);
        //  CreateGapPipes(50f, 20f, 20f);
        Bird.GetInstance().OnDied += Bird_OnDied;
        Bird.GetInstance().OnStartedPlaying += Bird_OnStartedPlaying;
    }

    private void Bird_OnStartedPlaying(object sender, System.EventArgs e)
    {
        // throw new System.NotImplementedException();
        state = State.Playing;
    }

    private void Bird_OnDied(object sender, System.EventArgs e)
    {
        // throw new System.NotImplementedException();
        state = State.BirdDead;

    }

    private void Update()
    {
        if (state == State.Playing)
        {
            HandlePipeMovement();
            HandlePipeSpawning();
            HandleGround();
            HandleClouds();
        }

    }


    private void SpawnInitialClouds()
    {
        cloudList = new List<Transform>();
        Transform cloudTransform;
        // float cloudY = +30f;
        cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(0, CLOUD_SPAWN_Y_POSTION, 0), Quaternion.identity);
        cloudList.Add(cloudTransform);
    }

    private Transform GetCloudPrefabTransform()
    {
        switch (Random.Range(0, 3))
        {
            default:
            case 0: return GameAssets.GetAssetInstance().pfCloud_1;
            case 1: return GameAssets.GetAssetInstance().pfCloud_2;
            case 2: return GameAssets.GetAssetInstance().pfCloud_3;
        }

    }

    private void HandleClouds()
    {
        //Handle Cloud Spawning
        cloudSpawnTimer -= Time.deltaTime;
        if (cloudSpawnTimer < 0)
        {
            //Time to Spawn another Cloud
            cloudSpawnTimerMax = 5f;
            cloudSpawnTimer = cloudSpawnTimerMax;
            //         float cloudY = +30f;
            Transform cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(CLOUD_SPAWN_X_POSTION, CLOUD_SPAWN_Y_POSTION, 0), Quaternion.identity);
            cloudList.Add(cloudTransform);
        }

        //Handle Cloud Moving
        for (int i = 0; i < cloudList.Count; i++)
        {
            Transform cloudTransform = cloudList[i];
            //Move Clouds by less speed than Pipes for Parallax
            cloudTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime * 0.7f;
            if (cloudTransform.position.x < CLOUD_DESTROY_X_POSTION)
            {   //Clouds past destroy point, destroy self
                Destroy(cloudTransform.gameObject);
                cloudList.RemoveAt(i);
                i--;
            }
        }
    }
    private void SpawnInitialGround()
    {
        groundList = new List<Transform>();
        Transform groundTransform;
        float groundY = -47f;
        float groundWidth = 160f;
        groundTransform = Instantiate(GameAssets.GetAssetInstance().pfGroundSprite, new Vector3(0, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetAssetInstance().pfGroundSprite, new Vector3(groundWidth, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetAssetInstance().pfGroundSprite, new Vector3(groundWidth * 2f, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);


    }

    private void HandleGround()
    {
        foreach (Transform groundTransform in groundList)
        {
            groundTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            if (groundTransform.position.x < GROUND_DESTROY_X_POSTION)
            {
                //Ground passed the left side, relocate on the right side
                //Find right most X Position
                float rightMostXPosition = -100f;
                for (int i = 0; i < groundList.Count; i++)
                {
                    if (groundList[i].position.x > rightMostXPosition)
                    {
                        rightMostXPosition = groundList[i].position.x;
                    }
                }
                //Place Ground on the right most position
                float groundWidth = 160f;
                groundTransform.position = new Vector3(rightMostXPosition + groundWidth, groundTransform.position.y, groundTransform.position.z);
            }

        }
    }
    private void HandlePipeSpawning()
    {
        pipeSpawnTimer -= Time.deltaTime;
        if (pipeSpawnTimer < 0)
        {
            pipeSpawnTimer += pipeSpawnTimerMax;
            float heightEdgeLimit = 10f;
            float minHeight = gapSize * .5f + heightEdgeLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float maxHeight = totalHeight - gapSize * .5f - heightEdgeLimit;
            float height = Random.Range(minHeight, maxHeight);

            CreateGapPipes(height, gapSize, PIPE_SPAWN_X_POSTION);

        }


    }
    private void HandlePipeMovement()
    {
        for (int i = 0; i < pipeList.Count; i++)
        {
            Pipe pipe = pipeList[i];
            bool isToTheRightOfBird = pipe.GetXPosition() > BIRD_X_POSITION;
            pipe.Move();
            if (isToTheRightOfBird && pipe.GetXPosition() <= BIRD_X_POSITION && pipe.IsBottom())
            {
                pipesPassedCount++;
                SoundManager.PlaySound(SoundManager.Sound.Score);
            }



            if (pipe.GetXPosition() < PIPE_DESTROY_X_POSTION)
            {
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }

        }
    }

    private void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY - gapSize * .5f, xPosition, true);
        CreatePipe(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize * .5f, xPosition, false);
        pipesSpawned++;
        SetDifficulty(GetDifficulty());
    }


    private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                gapSize = 50f;
                pipeSpawnTimerMax = 2.5f;
                break;
            case Difficulty.Medium:
                gapSize = 40f;
                pipeSpawnTimerMax = 2.4f;
                break;
            case Difficulty.Hard:
                gapSize = 30f;
                pipeSpawnTimerMax = 2.3f;
                break;
            case Difficulty.Impossible:
                gapSize = 20f;
                pipeSpawnTimerMax = 2.0f;
                break;
        }
    }

    private Difficulty GetDifficulty()
    {
        if (pipesSpawned >= 30) return Difficulty.Impossible;
        if (pipesSpawned >= 20) return Difficulty.Hard;
        if (pipesSpawned >= 10) return Difficulty.Medium;
        return Difficulty.Easy;
    }
    private void CreatePipe(float height, float xPosition, bool createBottom)
    {
        //Setup Pipe Head
        Transform pipeHead = Instantiate(GameAssets.GetAssetInstance().pfPipeHeadSprite);
        float pipeHeadYPosition;
        if (createBottom)
        {
            pipeHeadYPosition = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * 0.40f;
        }
        else
        {
            pipeHeadYPosition = +CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT * 0.40f;
        }
        pipeHead.position = new Vector3(xPosition, pipeHeadYPosition);
        // pipeList.Add(pipeHead);

        //Setup Pipe Body
        Transform pipeBody = Instantiate(GameAssets.GetAssetInstance().pfPipeBodySprite);
        float pipeBodyYPosition;
        if (createBottom)
        {
            pipeBodyYPosition = -CAMERA_ORTHO_SIZE;
        }
        else
        {
            pipeBodyYPosition = +CAMERA_ORTHO_SIZE;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }
        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);
        // pipeList.Add(pipeBody);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_BODY_WIDTH, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_BODY_WIDTH, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * 0.40f);

        Pipe pipe = new Pipe(pipeHead, pipeBody, createBottom);
        pipeList.Add(pipe);
    }

    public int GetsPipeSpawned()
    {
        return pipesSpawned;
    }

    public int GetsPipesPassedCount()
    {
        return pipesPassedCount;
    }
    private class Pipe
    {

        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;
        private bool isBottom;
        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform, bool isBottom)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
            this.isBottom = isBottom;
        }

        public void Move()
        {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetXPosition()
        {
            return pipeHeadTransform.position.x;
        }

        public bool IsBottom()
        {
            return isBottom;
        }
        public void DestroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }
}
