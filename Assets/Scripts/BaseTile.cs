using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BaseTile : MonoBehaviour
{
    public Node node { get; private set;}

    public bool numbered;

    [Range(1,6)]
    public int startingNumber;

    public void Start()
    {
        if (Application.isPlaying)
        {
            node = FindObjectOfType<C_Grid>().NodeFromWorldPoint(transform.position);
            node.tile = this;
            if (node != null)
            {
                FindObjectOfType<C_Grid>().tiles.Add(this);
            }
        }
    }

    void Update()
    {
        if (numbered)
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tiles/Tile_" + startingNumber);
        }
        else 
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tiles/Tile_Base");
        }
    }
}
