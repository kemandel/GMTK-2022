using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour
{
    private void Update() 
    {
        Transform dice = FindObjectOfType<Dice>().transform;
        transform.position = new Vector3 (dice.position.x, dice.position.y, transform.position.z);
    }
}
