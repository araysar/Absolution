using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    private Animator myAnim;
    private AudioSource mySound;
    public AudioClip selectSfx;
    public AudioClip highlightSfx;
    public AudioClip startGameSfx;
    public Button[] buttons;

    private void Start()
    {
        myAnim = GetComponent<Animator>();
        mySound = GetComponent<AudioSource>();
    }

    public void DisableButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
    }


    public void NewGame()
    {
        mySound.PlayOneShot(startGameSfx);
        myAnim.SetTrigger("StartGame");
    }

    public void Credits()
    {
        mySound.PlayOneShot(selectSfx);
        myAnim.SetTrigger("Credits");
    }

    public void ChangeLevel(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void ExitGame()
    {
        mySound.PlayOneShot(selectSfx);
        Application.Quit();
    }
}
