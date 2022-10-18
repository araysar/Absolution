using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{
    CinemachineVirtualCamera cinemachine;
    void Awake()
    {
        cinemachine = GetComponent<CinemachineVirtualCamera>();
        cinemachine.Follow = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if(cinemachine.Follow == null)
        {
            cinemachine.Follow = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
}
