using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CredtsManager : MonoBehaviour
{

    [SerializeField] private Player_Credits myPlayer;
    [SerializeField] private AudioSource chargeSfx;
    [SerializeField] private AudioSource musicChannel;

    private void Update()
    {
        if(Input.GetButtonDown("Fire1") || Input.GetButtonDown("Cancel") || Input.GetButtonDown("Submit"))
        {
            SceneManager.LoadScene(0);
        }
    }
    public void MovePlayer()
    {
        myPlayer.PlayMovement();
    }

    public void StopPlayer()
    {
        myPlayer.StopMovement();
    }

    public void ChangeToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ChargeScream()
    {
        chargeSfx.Play();
    }

    public void StopMusic()
    {
        musicChannel.Stop();
    }
}
