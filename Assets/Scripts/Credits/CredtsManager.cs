using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CredtsManager : MonoBehaviour
{

    [SerializeField] private Player_Credits myPlayer;
    [SerializeField] private AudioSource chargeSfx;
    [SerializeField] private AudioSource musicChannel;

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
