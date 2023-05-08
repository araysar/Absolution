using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardsColectable : MonoBehaviour
{
    public int myNumber;
    public AudioClip myClip;

    private void Start()
    {
        if(FindObjectOfType<Character_Attack>().myShards.Contains(myNumber))
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character_Attack player = collision.GetComponent<Character_Attack>();
        if(player != null)
        {
            SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, myClip);
            player.AddShard(myNumber);
            gameObject.SetActive(false);
        }
    }
}
