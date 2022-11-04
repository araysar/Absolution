using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManager.instance.TriggerAction(GameManager.ExecuteAction.SaveData);
            gameObject.SetActive(false);
        }
    }
}
