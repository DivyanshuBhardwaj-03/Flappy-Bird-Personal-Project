using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest : MonoBehaviour
{
    private const float PIPE_BODY_WIDTH = 7f;
    private const float PIPE_HEAD_HEIGHT = 6.43f;
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_MOVE_SPEED = 3f;
    private const float PIPE_DESTROY_X_POSTION = -100f;
    private const float PIPE_SPAWN_X_POSTION = +100f;

    private List<Pipe> pipeList;
    private int pipesSpawned;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible
    }
    private void Awake()
    {
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 8.0f;
        gapSize = 50f;
        SetDifficulty(Difficulty.Easy);
    }

    private void Start()
    {
        // CreatePipe(50f, 20f, true);
        //CreatePipe(50f, 20f, false);
        //  CreateGapPipes(50f, 20f, 20f);

    }
    private void Update()
    {
        HandlePipeMovement();
        HandlePipeSpawning();
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
            pipe.Move();
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
                pipeSpawnTimerMax = 8.2f;
                break;
            case Difficulty.Medium:
                gapSize = 40f;
                pipeSpawnTimerMax = 8.1f;
                break;
            case Difficulty.Hard:
                gapSize = 30f;
                pipeSpawnTimerMax = 8.0f;
                break;
            case Difficulty.Impossible:
                gapSize = 20f;
                pipeSpawnTimerMax = 7.8f;
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

        Pipe pipe = new Pipe(pipeHead, pipeBody);
        pipeList.Add(pipe);
    }

    private class Pipe
    {
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
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

        public void DestroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }
}
