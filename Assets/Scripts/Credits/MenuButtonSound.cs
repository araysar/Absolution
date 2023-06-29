using UnityEngine;

public class MenuButtonSound : MonoBehaviour
{
    public AudioClip highlightSfx;
    public AudioSource mySound;

    public void HightLightSound()
    {
        mySound.PlayOneShot(highlightSfx);
    }
}
