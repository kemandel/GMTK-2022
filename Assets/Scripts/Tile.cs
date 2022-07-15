using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Node node { get; private set;}

    void Start()
    {
        node = FindObjectOfType<C_Grid>().NodeFromWorldPoint(transform.position);
        node.tile = this;
        if (node != null)
        {
            FindObjectOfType<C_Grid>().tiles.Add(this);
        }
    }
}
