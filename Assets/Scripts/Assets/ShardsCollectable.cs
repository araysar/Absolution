using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardsCollectable : MonoBehaviour
{
    public int myNumber;
    public AudioClip myClip;

    private void Start()
    {
        if (GameManager.instance.saveManager.shards.Contains(myNumber))
        {
            Destroy(gameObject);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character_Attack player = collision.GetComponent<Character_Attack>();
        if(player != null)
        {
            SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, myClip);
            player.AddShard(myNumber);
            GameManager.instance.EnemyRespawnEvent += Respawn;
            gameObject.SetActive(false);
        }
    }

    private void Respawn()
    {
        gameObject.SetActive(true);
    }
}
