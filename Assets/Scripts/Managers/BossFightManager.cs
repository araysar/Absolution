using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    [SerializeField] private TriggerBossDoor myTrigger;
    [SerializeField] private GameObject myCamera;
    [SerializeField] private GameObject bossDamageBounds;
    [SerializeField] private AudioClip myMusic;
    [SerializeField] private GameObject myBoss;
    [SerializeField] private Coroutine myCoroutine;
    

    [SerializeField] private CinemachineBlendDefinition.Style myTransitionEffect;
    [SerializeField] private float myCameraTransitionDuration;
    [SerializeField] private float myBossWarcryTime = 3;
    private CinemachineBrain myCameraBrain;

    public event Action EnteringBossDoorEvent;
    public event Action StartFightEvent;
    public event Action ResetFightActionEvent;

    void Start()
    {
        myCameraBrain = FindObjectOfType<CinemachineBrain>();
        EnteringBossDoorEvent += SoundManager.instance.StopSong;
        EnteringBossDoorEvent += Character_Movement.instance.StopMovement;
        StartFightEvent += ChangeMusic;
        StartFightEvent += Character_Movement.instance.ResumeMovement;

        GameManager.instance.ResetBossBattleEvent += myTrigger.ResetPosition;
        GameManager.instance.ResetBossBattleEvent += ResetBattle;
    }

    private void ChangeMusic()
    {
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Music, myMusic, transform);
    }

    public void EnteringBossDoor()
    {
        if (myCoroutine != null) return;
        myCoroutine = StartCoroutine(EnteringTimer());
    }

    private IEnumerator EnteringTimer()
    {
        EnteringBossDoorEvent();
        GameManager.instance.TriggerAction(GameManager.ExecuteAction.EnterBossDoor);
        bossDamageBounds.SetActive(true);
        yield return new WaitForSeconds(1);
        myCameraBrain.m_DefaultBlend.m_Style = myTransitionEffect;
        myCameraBrain.m_DefaultBlend.m_Time = myCameraTransitionDuration;
        myCamera.SetActive(true);
        yield return new WaitForSeconds(myCameraTransitionDuration);
        myBoss.GetComponent<Animator>().SetTrigger("preparation");
        yield return new WaitForSeconds(myBossWarcryTime);
        TriggerBattle();
    }
    public void TriggerBattle()
    {
        myCoroutine = null;
        StartFightEvent();
    }

    public void ResetBattle()
    {
        myCameraBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
        bossDamageBounds.SetActive(false);
        myCamera.SetActive(false);
    }
}
