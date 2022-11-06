using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    [SerializeField] private TriggerBossDoor myTrigger;
    [SerializeField] private GameObject myCamera;

    [SerializeField] private CinemachineBlendDefinition.Style myTransitionEffect;
    [SerializeField] private float myTransitionDuration;
    private CinemachineBrain myCameraBrain;

    public event Action EnteringBossDoorEvent;
    public event Action StartFightEvent;
    public event Action ResetFightActionEvent;

    void Start()
    {
        myCameraBrain = FindObjectOfType<CinemachineBrain>();
        ResetFightActionEvent += myTrigger.ResetPosition;
        EnteringBossDoorEvent += Character_Movement.instance.StopMovement;
        StartFightEvent += Character_Movement.instance.ResumeMovement;
    }

    public void EnteringBossDoor()
    {
        StartCoroutine(EnteringTimer());
    }

    private IEnumerator EnteringTimer()
    {
        EnteringBossDoorEvent();
        yield return new WaitForSeconds(1);
        myCameraBrain.m_DefaultBlend.m_Style = myTransitionEffect;
        myCameraBrain.m_DefaultBlend.m_Time = myTransitionDuration;
        myCamera.SetActive(true);
        yield return new WaitForSeconds(myTransitionDuration);

    }
    public void TriggerBattle()
    {
        StartFightEvent();
    }

    public void ResetBattle()
    {
        myCameraBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
        myCamera.SetActive(false);
    }
}
