using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dice : MonoBehaviour
{
    public BaseTile activeTile { get; private set; }
    public int[] faces { get; private set; }
    private Coroutine activeCoroutine;
    private int currentRotation;
    private SpriteRenderer cubeSprite;
    private SpriteRenderer faceSprite;

    [Range(.1f, 1)]
    public float moveTime;
    public float moveDelay;

    private void Start()
    {
        faces = new int[6];
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        cubeSprite = spriteRenderers[0];
        faceSprite = spriteRenderers[1];
        GetComponent<Animator>().SetBool("Dead", false);

        if (activeTile != null && activeTile.start)
        {
            // Initialize starting position
            faces[0] = 7 - activeTile.startingNumber;
            faces[5] = (7 - faces[0]) % 6;
            faces[1] = ((7 - activeTile.startingNumber + 2) % 6);
            faces[4] = (7 - faces[1]) % 6;
            faces[2] = ((7 - activeTile.startingNumber + 4) % 6);
            faces[3] = (7 - faces[2]) % 6;

            for (int i = 0; i < faces.Length; i++)
            {
                if (faces[i] == 0)
                {
                    faces[i] = 6;
                }
            }

            RotateDice(activeTile.startingRotations);
        }


        DrawFace();

        activeCoroutine = null;
    }

    private void Update()
    {
        if (!LevelManager.Paused)
        {
            Vector3 inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (activeCoroutine == null)
            {
                if (inputDirection.x != 0)
                {
                    activeCoroutine = StartCoroutine(MoveCoroutine(inputDirection));
                }
                else if (inputDirection.y != 0)
                {
                    activeCoroutine = StartCoroutine(MoveCoroutine(inputDirection));
                }
            }
        }
    }

    private IEnumerator MoveCoroutine(Vector3 inputDirection)
    {
        // Make sure only one direction is active
        if (inputDirection.x != 0)
        {
            inputDirection.y = 0;
        }

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + (inputDirection * FindObjectOfType<C_Grid>().nodeRadius * 2);
        BaseTile newTile = FindObjectOfType<C_Grid>().NodeFromWorldPoint(targetPosition).tile;

        float startTime = Time.time;

        // Roll the cube
        SetCubeRotation(inputDirection);
        GetComponent<Animator>().SetTrigger("Roll");
        GetComponent<Animator>().SetFloat("RollSpeed", 1 / moveTime);

        // interpolate between the cube's current and next positions
        while (transform.position != targetPosition)
        {
            float passedTime = Time.time - startTime;
            float interRatio = passedTime / moveTime;
            Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, interRatio);
            transform.position = newPosition;

            yield return null;
        }

        // Set the new faces of the die
        MoveFaces(inputDirection);
        DrawFace();

        if (activeTile.shadow)
        {
            activeTile.FadeAway(moveDelay + moveTime);
        }

        activeTile = FindObjectOfType<C_Grid>().NodeFromWorldPoint(transform.position).tile;

        if (activeTile != null && activeTile.quit)
        {
            GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/Lose"));
            FindObjectOfType<LevelManager>().QuitGame();
            yield break;
        }

        if (!CheckTile(activeTile))
        {
            Lose();
            yield break;
        }
        
        if (activeTile.goal){
            Win();
            yield break;
        }

        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/Note_" + faces[5]));

        yield return new WaitForSeconds(moveDelay);
        activeCoroutine = null;
    }

    public void SetActiveTile(BaseTile tile)
    {
        activeTile = tile;
        transform.position = tile.node.worldPosition;
    }

    private void SetCubeRotation(Vector2 direction)
    {
        if (direction.x >= 1)
        {
            // Rotate Facing Right
            cubeSprite.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (direction.x <= -1)
        {
            // Rotate Facing Left
            cubeSprite.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        else if (direction.y >= 1)
        {
            // Rotate Facing Up
            cubeSprite.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        else if (direction.y <= -1)
        {
            // Rotate Facing Down
            cubeSprite.transform.eulerAngles = new Vector3(0, 0, 270);
        }
    }

    private void DrawFace()
    {
        faceSprite.sprite = Resources.Load<Sprite>("Sprites/Dice/Face_" + faces[0]);
        int i = ((((faces[1] + faces[2]) % 2) == 1) ? 1 : 0);
        faceSprite.transform.eulerAngles = new Vector3(0, 0, 90 * i);
    }

    private void MoveFaces(Vector2 direction)
    {
        int[] facesOld = (int[])faces.Clone();

        if (direction.x <= -1)
        {
            // Move Left
            faces[0] = facesOld[4];
            faces[1] = facesOld[0];
            faces[4] = facesOld[5];
            faces[5] = facesOld[1];
        }
        else if (direction.x >= 1)
        {
            // Move Right
            faces[0] = facesOld[1];
            faces[1] = facesOld[5];
            faces[4] = facesOld[0];
            faces[5] = facesOld[4];
        }
        else if (direction.y >= 1)
        {
            // Move Up
            faces[0] = facesOld[2];
            faces[2] = facesOld[5];
            faces[3] = facesOld[0];
            faces[5] = facesOld[3];
        }
        else if (direction.y <= -1)
        {
            // Move Down
            faces[0] = facesOld[3];
            faces[2] = facesOld[0];
            faces[3] = facesOld[5];
            faces[5] = facesOld[2];
        }
    }

    /// <summary>
    /// Rotates the die by 90 degrees `rotations` times.
    /// </summary>
    /// <param name="rotations"></param>
    private void RotateDice(int rotations)
    {
        for (int i = 0; i < rotations; i++)
        {
            MoveFaces(Vector2.up);
            MoveFaces(Vector2.left);
            MoveFaces(Vector2.down);
        }
    }

    private bool CheckTile(BaseTile tile)
    {
        if (tile == null) return false;
        if (!tile.active) return false;
        if (activeTile.numbered && faces[5] != tile.startingNumber) return false;
        return true;
    }

    private void Lose()
    {
        if (activeTile != null)
        {
            if (activeTile.isActiveAndEnabled) activeTile.FadeAway(.5f);
        }
        GetComponent<Animator>().SetFloat("RollSpeed", 1 / (moveTime * 2));
        GetComponent<Animator>().SetBool("Dead", true);
        faceSprite.sprite = null;

        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/Lose"));

        FindObjectOfType<LevelManager>().ReloadScene();
    }

    private void Win()
    {
        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/Win"));
        FindObjectOfType<LevelManager>().LoadNextLevel();
    }
}

public struct Face
{
    public int number;
    public bool rotated;
}
