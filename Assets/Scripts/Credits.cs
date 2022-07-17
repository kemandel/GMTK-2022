using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public string[] textToDisplay;
    public float fadeSpeed;
    public float textTimeDifference;
    public float displayTime;

    private void Start() {
        StartCoroutine(CreditsCoroutine());
    }

    private IEnumerator CreditsCoroutine()
    {
        Text[] texts = GetComponentsInChildren<Text>();

        for (int i = 0; i < textToDisplay.Length; i += 2)
        {
            texts[0].text = textToDisplay[i];
            if (i < textToDisplay.Length - 1)
            {
                texts[1].text = textToDisplay[i + 1];
            }

            StartCoroutine(FadeInTextCoroutine(fadeSpeed, texts[0]));
    
            if (texts[1].text != "")
            {
                yield return new WaitForSeconds(textTimeDifference);
                StartCoroutine(FadeInTextCoroutine(fadeSpeed, texts[1]));
            }

            yield return new WaitForSeconds(displayTime);


            StartCoroutine(FadeOutTextCoroutine(fadeSpeed, texts[0]));

            if (texts[1].text != "")
            {
                yield return new WaitForSeconds(textTimeDifference);
                StartCoroutine(FadeOutTextCoroutine(fadeSpeed, texts[1]));
            }

            yield return new WaitForSeconds(displayTime/2);
        }

        FindObjectOfType<LevelManager>().QuitGame();
    }

    private IEnumerator FadeInTextCoroutine(float fadeTime, Text textbox)
    {
        float startTime = Time.time;
        Color color = textbox.color;
        while (textbox.color.a < 1)
        {
            float interRatio = (Time.time - startTime) / fadeTime;
            color.a = Mathf.Lerp(0, 1, interRatio);
            textbox.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeOutTextCoroutine(float fadeTime, Text textbox)
    {
        float startTime = Time.time;
        Color color = textbox.color;
        while (textbox.color.a > 0)
        {
            float interRatio = (Time.time - startTime) / fadeTime;
            color.a = Mathf.Lerp(1, 0, interRatio);
            textbox.color = color;
            yield return null;
        }
    }
}
