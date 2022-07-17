using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static string displayText { get; set; }

    void Start()
    {
        gameObject.GetComponentInParent<Canvas>().worldCamera = Camera.main;
        Text levelDisplay = GetComponentInChildren<Text>();
        levelDisplay.text = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        Text levelDisplay = GetComponentInChildren<Text>();
        levelDisplay.text = displayText;

        Image[] images = GetComponentsInChildren<Image>();
        Dice dice = FindObjectOfType<Dice>();

        for (int i = 0; i < dice.faces.Length; i++)
        {
            images[i].sprite = Resources.Load<Sprite>("Sprites/UI/UIFace_" + dice.faces[i]);
        }
    }
}
