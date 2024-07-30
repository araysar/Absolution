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
                if(FindObjectOfType<BossFightManager>() != null && GameManager.instance.fightingBoss)
                {
                    SoundManager.instance.PlaySound(SoundManager.SoundChannel.Music, SoundManager.instance.bossFightMusic, transform);
                }
                else
                {
                    SoundManager.instance.PlaySound(SoundManager.SoundChannel.Music, music, transform);
                }
            }
        }
    }
}
