using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] private AudioClip myClip;

    public void Sound()
    {
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, myClip);
    }
}
