using UnityEngine;

public class ExitZone : MonoBehaviour
{
    [SerializeField] private int sceneNumber = 1;
    [SerializeField] private Vector2 nextPosition;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Character_Movement>() != null)
        {
            collision.transform.position = nextPosition;
            GameManager.instance.ChangeScene(sceneNumber);
        }
    }
}
