using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    [SerializeField] private TriggerBossDoor myTrigger;
    [SerializeField] private GameObject normalCamera;
    [SerializeField] private GameObject bossCamera;
    [SerializeField] private AudioClip myMusic;
    [SerializeField] private Boss_Ice myBoss;
    [SerializeField] private Coroutine myCoroutine;
    private Collider2D myCollider;
    

    [SerializeField] private CinemachineBlendDefinition.Style myTransitionEffect;
    [SerializeField] private float myCameraTransitionDuration;
    [SerializeField] private float myBossWarcryTime = 3;
    private CinemachineBrain myCameraBrain;

    public event Action EnteringBossDoorEvent;
    public event Action StartFightEvent;
    public event Action ResetFightActionEvent;

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
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
        yield return new WaitForSeconds(1);
        myCameraBrain.m_DefaultBlend.m_Style = myTransitionEffect;
        myCameraBrain.m_DefaultBlend.m_Time = myCameraTransitionDuration;
        bossCamera.SetActive(true);
        normalCamera.SetActive(false);
        yield return new WaitForSeconds(myCameraTransitionDuration);
        myBoss.myAnim.SetTrigger("preparation");
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
        bossCamera.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            myCollider.enabled = false;
            EnteringBossDoor();
        }
    }
}
