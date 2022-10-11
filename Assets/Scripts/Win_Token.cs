using UnityEngine;

public class Win_Token : MonoBehaviour
{
    [SerializeField] private GameObject panelWin;
    [SerializeField] private Vector2 initialPosition = new Vector2(1.5f, 0.5f);

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
        GameObject.FindGameObjectWithTag("Player").gameObject.transform.position = initialPosition;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void Button_Exit()
    {
        Application.Quit();
    }
}
