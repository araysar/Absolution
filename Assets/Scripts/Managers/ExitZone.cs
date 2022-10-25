using UnityEngine;

public class ExitZone : MonoBehaviour
{
    [SerializeField] private int nextScene = 1;
    [SerializeField] private Vector2 nextPosition;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Character_Movement>() != null)
        {
            GameManager.instance.nextPosition = nextPosition;
            GameManager.instance.nextScene = nextScene;
            GameManager.instance.Transition(GameManager.EventType.DoorTransition, 0);
        }
    }
}
