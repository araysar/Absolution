using UnityEngine;

public class AreaBounds : MonoBehaviour
{
    [SerializeField] private bool changeMusic = false;
    [SerializeField] private AudioClip music;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(changeMusic)
            {
                SoundManager.instance.PlaySound(SoundManager.SoundChannel.Music, music, transform);
            }
        }
    }
}
