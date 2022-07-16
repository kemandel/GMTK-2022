using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FadeLight : MonoBehaviour
{
    private float defaultIntensity;

    private void Awake() 
    {
        defaultIntensity = GetComponent<Light2D>().intensity;
    }

    public IEnumerator FadeInCoroutine(float seconds)
    {
        float startTime = Time.time;
        Light2D myLight = GetComponent<Light2D>();
        myLight.intensity = 0;
        while(myLight.intensity != defaultIntensity)
        {
            float interRatio = 1;
            if (seconds != 0)
            {
                interRatio = (Time.time - startTime) / seconds;
            }

            myLight.intensity = Mathf.Lerp(0, defaultIntensity, interRatio);
            yield return null;
        }
    }

    public IEnumerator FadeOutCoroutine(float seconds)
    {
        float startTime = Time.time;
        Light2D myLight = GetComponent<Light2D>();
        myLight.intensity = defaultIntensity;
        while(myLight.intensity != 0)
        {
            float interRatio = 1;
            if (seconds != 0)
            {
                interRatio = (Time.time - startTime) / seconds;
            }

            myLight.intensity = Mathf.Lerp(defaultIntensity, 0, interRatio);
            yield return null;
        }
    }
}
