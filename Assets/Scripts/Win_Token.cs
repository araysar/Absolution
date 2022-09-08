using UnityEngine;

public class Win_Token : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
