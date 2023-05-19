using UnityEngine;

public class Win_Token : MonoBehaviour
{
    [SerializeField] private AudioClip myGrabSfx;
    [SerializeField] private string nextScene = "EndScene-Words";
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, myGrabSfx, transform);
            GameManager.instance.nextPosition = transform.position;
            GameManager.instance.nextScene = nextScene;
            GameManager.instance.Transition(GameManager.EventType.EndGameTransition, 0);
            gameObject.SetActive(false);
        }
    }
}
