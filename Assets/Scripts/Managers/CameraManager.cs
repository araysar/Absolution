using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public CinemachineVirtualCamera normalCamera;
    public CinemachineVirtualCamera playerCamera;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        playerCamera.Follow = GameObject.FindGameObjectWithTag("Player").transform;
        normalCamera.Follow = playerCamera.Follow;
    }
}
