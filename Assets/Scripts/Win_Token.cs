using UnityEngine;

public class Win_Token : MonoBehaviour
{
    [SerializeField] private GameObject panelWin;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            panelWin.SetActive(true); 
            GameManager.instance.Pause();
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void Button_Restart()
    {
        GameManager.instance.UnPause();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void Button_Exit()
    {
        Application.Quit();
    }
}
