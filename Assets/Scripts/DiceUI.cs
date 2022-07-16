using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceUI : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponentInParent<Canvas>().worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Image[] images = GetComponentsInChildren<Image>();
        Dice dice = FindObjectOfType<Dice>();

        for (int i = 0; i < dice.faces.Length; i++)
        {
            images[i].sprite = Resources.Load<Sprite>("Sprites/UI/UIFace_" + dice.faces[i]);
        }
    }
}
