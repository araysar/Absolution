using UnityEngine;

public class Win_Token : MonoBehaviour
{
    [SerializeField] private AudioClip myGrabSfx;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, myGrabSfx);
            GameManager.instance.nextPosition = transform.position;
            GameManager.instance.nextScene = 4;
            GameManager.instance.Transition(GameManager.EventType.DoorTransition, 0);
            gameObject.SetActive(false);
        }
    }
}
