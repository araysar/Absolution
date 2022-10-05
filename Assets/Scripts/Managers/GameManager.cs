using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Character_Movement player;
    public bool onPause = false;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        player = FindObjectOfType<Character_Movement>();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void Pause()
    {
        onPause = true;
        Time.timeScale = 0;
        SoundManager.instance.PauseChannels();
    }

    public void ChangeScene(int sceneNumber)
    {
        StatsManager.SaveStats(player);
        SceneManager.LoadScene(sceneNumber);
    }

    public void UnPause()
    {
        onPause = false;
        SoundManager.instance.UnPauseChannels();
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        StartCoroutine(RestartCoroutine());
    }

    private IEnumerator RestartCoroutine()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(0);
    }
}
