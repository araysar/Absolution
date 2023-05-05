public class PlayerCamera : CameraFollow
{
    void Start()
    {
        FindObjectOfType<Player_Health>().myCamera = gameObject;
        gameObject.SetActive(false);
    }
}
