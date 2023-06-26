using UnityEngine;

public class DoorChangeScene : MonoBehaviour
{
    public GameObject actionButton;
    private bool onRange = false;
    [SerializeField] private string nextScene;
    [SerializeField] private Vector2 nextPosition;
    public bool bossDoor = false;

    void Update()
    {
        if (onRange && Input.GetButtonDown("Action"))
        {
            if(!GameManager.instance.onPause && Time.timeScale != 0)
            {
                GameManager.instance.nextPosition = nextPosition;
                GameManager.instance.nextScene = nextScene;
                if (!bossDoor)
                    SoundManager.instance.PlaySound(SoundManager.SoundChannel.Unscalled, SoundManager.instance.openCommonDoor, transform);
                else
                    SoundManager.instance.PlaySound(SoundManager.SoundChannel.Unscalled, SoundManager.instance.bossDoor, transform);
                GameManager.instance.Transition(GameManager.EventType.DoorTransition, 0);
                onRange = false;
                actionButton.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            onRange = true;
            actionButton.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            onRange = false;
            actionButton.SetActive(false);
        }
    }
    
    public void Trigger()
    {
        GameManager.instance.nextPosition = nextPosition;
        GameManager.instance.nextScene = nextScene;
        GameManager.instance.Transition(GameManager.EventType.DoorTransition, 0);
    }
}
