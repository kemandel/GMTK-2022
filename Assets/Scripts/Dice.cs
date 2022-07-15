using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private Tile activeTile { get; set; }
    private Coroutine activeCoroutine;

    [Range(.1f, 2)]
    public float moveTime;

    private void Start()
    {
        activeTile = FindObjectOfType<C_Grid>().NodeFromWorldPoint(transform.position).tile;
    }

    private void Update()
    {
        Vector3 inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (activeCoroutine == null)
        {
            if (inputDirection.x != 0)
            {
                StartCoroutine(MoveCoroutine(inputDirection));
            }
            else if (inputDirection.y != 0)
            {

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

        float startTime = Time.time;
        while (transform.position != targetPosition)
        {
            float passedTime = Time.time - startTime;
            float interRatio = passedTime / moveTime;
            Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, interRatio);

            yield return null;
        }
    }
}
