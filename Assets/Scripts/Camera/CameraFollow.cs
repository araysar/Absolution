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
}
