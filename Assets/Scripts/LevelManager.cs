using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public float fadeTime;
    public float reloadDelay;

    // Start is called before the first frame update
    void Start()
    {
        FadeLight[] lights = FindObjectsOfType<FadeLight>();
        for (int i = 0; i < lights.Length; i++)
        {
            StartCoroutine(lights[i].FadeInCoroutine(fadeTime));
        }
    }

    public IEnumerator LoadNextLevelCoroutine()
    {
        FadeLight[] lights = FindObjectsOfType<FadeLight>();
        for (int i = 0; i < lights.Length; i++)
        {
            StartCoroutine(lights[i].FadeOutCoroutine(fadeTime));
        }

        yield return new WaitForSeconds(fadeTime);

        if (SceneManager.GetActiveScene().buildIndex != SceneManager.sceneCountInBuildSettings - 1)
        {
            // Load level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public IEnumerator ReloadSceneCoroutine()
    {
        FadeLight[] lights = FindObjectsOfType<FadeLight>();
        for (int i = 0; i < lights.Length; i++)
        {
            StartCoroutine(lights[i].FadeOutCoroutine(reloadDelay));
        }
        yield return new WaitForSeconds(reloadDelay);

        // Reloads the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
