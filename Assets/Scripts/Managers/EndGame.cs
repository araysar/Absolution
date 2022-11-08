using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public void ExitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        GameManager.instance.TriggerAction(GameManager.ExecuteAction.DestroyEvent);
        SceneManager.LoadScene(0);
    }


}
