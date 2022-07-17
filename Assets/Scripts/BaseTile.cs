using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BaseTile : MonoBehaviour
{
    public Node node { get; private set;}

    public bool active;
    public bool start;
    public bool goal;
    public bool quit;
    public bool numbered;
    public bool shadow;

    [Range(1,6)]
    public int startingNumber;

    [Range(0,3)]
    public int startingRotations;

    public void Awake(){
        if (Application.isPlaying)
        {
            node = FindObjectOfType<C_Grid>().NodeFromWorldPoint(transform.position);
            node.tile = this;
            if (node != null)
            {
                FindObjectOfType<C_Grid>().tiles.Add(this);
            }

            if (start)
            {
                FindObjectOfType<Dice>().SetActiveTile(this);
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

    public void FadeAway(float fadeTime)
    {
        active = false;
        StartCoroutine(FadeAwayCoroutine(fadeTime));
    }

    private IEnumerator FadeAwayCoroutine(float fadeTime)
    {
        float startTime = Time.time;

        Color color = GetComponent<SpriteRenderer>().color;
        while(GetComponent<SpriteRenderer>().color.a > 0)
        {
            float interRatio = 1;
            if (fadeTime > 0)
            {
                interRatio = (Time.time - startTime) / fadeTime;
            }
            color.a = Mathf.Lerp(1, 0, interRatio);
            GetComponent<SpriteRenderer>().color = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
