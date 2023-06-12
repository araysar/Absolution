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
        if(playerCamera != null) playerCamera.Follow = GameObject.FindGameObjectWithTag("Player").transform;
        if(normalCamera != null) normalCamera.Follow = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
