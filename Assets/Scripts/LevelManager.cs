using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static bool Paused;

    public float fadeTime;
    public float reloadDelay;
    public bool canPause;
    public string levelText;

    private Canvas pauseCanvas;

    // Start is called before the first frame update
    void Start()
    {
        FadeLight[] lights = FindObjectsOfType<FadeLight>();
        for (int i = 0; i < lights.Length; i++)
        {
            StartCoroutine(lights[i].FadeInCoroutine(fadeTime));
        }
        pauseCanvas = GetComponentInChildren<Canvas>();
        pauseCanvas.worldCamera = Camera.main;
        Unpause();
        GameUI.displayText = levelText;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canPause)
        {
            if (Paused)
            {
                Unpause();
            }
            else 
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        Paused = true;
        pauseCanvas.gameObject.SetActive(true);
    }

    public void Unpause()
    {
        Paused = false;
        pauseCanvas.gameObject.SetActive(false);
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadNextLevelCoroutine());
    }

    public void ReloadScene()
    {
        StartCoroutine(ReloadSceneCoroutine());
    }

    public void QuitGame()
    {
        StartCoroutine(QuitGameCoroutine());
    }

    private IEnumerator LoadNextLevelCoroutine()
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

    private IEnumerator ReloadSceneCoroutine()
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

    private IEnumerator QuitGameCoroutine()
    {
        FadeLight[] lights = FindObjectsOfType<FadeLight>();
        for (int i = 0; i < lights.Length; i++)
        {
            StartCoroutine(lights[i].FadeOutCoroutine(fadeTime));
        }

        yield return new WaitForSeconds(fadeTime);

        Debug.Log("Application.Quit()");
        Application.Quit();
    }
}
