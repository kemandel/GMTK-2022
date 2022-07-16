using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dice : MonoBehaviour
{
    private BaseTile activeTile { get; set; }
    private Coroutine activeCoroutine;
    private Face[] faces = new Face[6];
    private int currentRotation;
    private SpriteRenderer cubeSprite;
    private SpriteRenderer faceSprite;

    [Range(.1f, 1)]
    public float moveTime;
    public float moveDelay;

    private void Start()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        cubeSprite = spriteRenderers[0];
        faceSprite = spriteRenderers[1];
        GetComponent<Animator>().SetBool("Dead", false);

        // Initialize starting position
        activeTile = FindObjectOfType<StartTile>();
        faces[0].number = 7 - ((StartTile)activeTile).startingNumber;
        faces[5].number = ((StartTile)activeTile).startingNumber;
        int offset = 7 - ((StartTile)activeTile).startingNumber;
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].number = (1 + i) % 6;
            if (faces[i].number == 0) faces[i].number = 6;
        }
        RotateDice(((StartTile)activeTile).startingRotations);


        DrawFace();

        activeCoroutine = null;
    }

    private void Update()
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

        if (newTile != null)
        {
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

            activeTile = FindObjectOfType<C_Grid>().NodeFromWorldPoint(transform.position).tile;

            if (!CheckTile(activeTile))
            {
                Debug.Log("Died");
                StartCoroutine(DieCoroutine(2));
                yield break;
            }

            yield return new WaitForSeconds(moveDelay);
            activeCoroutine = null;
        }
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
        faceSprite.sprite = Resources.Load<Sprite>("Sprites/Dice/Face_" + faces[0].number);
    }

    private void MoveFaces(Vector2 direction)
    {
        Face[] facesOld = (Face[])faces.Clone();

        if (direction.x <= -1)
        {
            // Move Left
            faces[0].number = facesOld[4].number;
            faces[1].number = facesOld[0].number;
            faces[2].rotated = !faces[2].rotated;
            faces[3].rotated = !faces[3].rotated;
            faces[4].number = facesOld[5].number;
            faces[5].number = facesOld[1].number;
        }
        else if (direction.x >= 1)
        {
            // Move Right
            faces[0].number = facesOld[1].number;
            faces[1].number = facesOld[5].number;
            faces[2].rotated = !faces[2].rotated;
            faces[3].rotated = !faces[3].rotated;
            faces[4].number = facesOld[0].number;
            faces[5].number = facesOld[4].number;
        }
        else if (direction.y >= 1)
        {
            // Move Up
            faces[0].number = facesOld[2].number;
            faces[1].rotated = !faces[1].rotated;
            faces[2].number = facesOld[5].number;
            faces[3].number = facesOld[0].number;
            faces[4].rotated = !faces[4].rotated;
            faces[5].number = facesOld[3].number;
        }
        else if (direction.y <= -1)
        {
            // Move Down
            faces[0].number = facesOld[3].number;
            faces[1].rotated = !faces[1].rotated;
            faces[2].number = facesOld[0].number;
            faces[3].number = facesOld[5].number;
            faces[4].rotated = !faces[4].rotated;
            faces[5].number = facesOld[2].number;
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
        if (!activeTile.numbered) return true;
        if (faces[5].number != tile.startingNumber) return false;
        return true;
    }

    private bool CheckTile(Vector2 direction, BaseTile tile)
    {
        if (!activeTile.numbered) return true;
        if (direction.x >= 1 && faces[4].number != tile.startingNumber) return false;
        else if (direction.x <= -1 && faces[1].number != tile.startingNumber) return false;
        else if (direction.y >= 1 && faces[3].number != tile.startingNumber) return false;
        else if (direction.y <= -1 && faces[2].number != tile.startingNumber) return false;
        return true;
    }

    private IEnumerator DieCoroutine(float seconds)
    {
        activeTile.gameObject.SetActive(false);
        GetComponent<Animator>().SetBool("Dead", true);
        faceSprite.sprite = null;

        yield return new WaitForSeconds(seconds);

        // Reloads the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

public struct Face
{
    public int number;
    public bool rotated;
}
