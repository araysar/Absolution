using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public SaveManager saveManager;
    public Character_Movement player;
    private Animator myAnim;
    public bool onPause = false;
    public bool isBusy = false;
    private float gravity;

    public event Action SetupPlayerAttacks = delegate { };
    public event Action EnemyRespawnEvent = delegate { };
    public event Action AllwaysRespawnEvent = delegate { };
    public event Action HealAllEnemiesEvent = delegate { };
    public event Action PlayerDisableEvent = delegate { };
    public event Action PlayerRespawnEvent = delegate { };
    public event Action SaveDataEvent = delegate { };
    public event Action LoadDataEvent = delegate { };
    public event Action DestroyEvent = delegate { };
    public event Action StopMovementEvent = delegate { };
    public event Action ResumeMovementEvent = delegate { };
    public event Action StopPlayerMovementEvent = delegate { };
    public event Action ResumePlayerMovementEvent = delegate { };
    public event Action TransitionEvent = delegate { };
    public event Action EnterBossDoorEvent = delegate { };
    public event Action ResetBossBattleEvent = delegate { };
    public event Action EndGameEvent = delegate { };

    [Header("NextZone")]
    [HideInInspector] public string nextScene;
    [HideInInspector] public Vector2 nextPosition = Vector2.zero;


    public enum EventType
    {
        PlayerDeathTransition,
        DoorTransition,
        EndGameTransition,
    };

    public enum ParticleType
    {
        CommonEnemyDeathEffect,
        PlayerDeathEffect,
        ScreamParticleEffect,
        BossDeathEffect,
    };

    public enum ExecuteAction
    {
        SetupPlayerAttacks,
        EnemyRespawnEvent,
        HealAllEnemiesEvent,
        PlayerDisableEvent,
        PlayerRespawnEvent,
        SaveData,
        LoadData,
        DestroyEvent,
        AllwaysRespawnEvent,
        StopMovementEvent,
        ResumeMovementEvent,
        ResumePlayerMovementEvent,
        StopPlayerMovementEvent,
        EnterBossDoor,
        ResetBossBattle,
        EndGame,
    };

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DestroyEvent += Destroy;
        }

        saveManager = GetComponent<SaveManager>();
        player = FindObjectOfType<Character_Movement>();
        nextPosition = player.transform.position;

        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;

        gravity = Physics2D.gravity.y;
        StopMovementEvent += NoGravity;
        ResumeMovementEvent += RecoverGravity;
        myAnim = GetComponent<Animator>();
    }

    public void EndGameTransition()
    {
        nextScene = "EndGame-Words";
    }


    public void Pause()
    {
        onPause = true;
        Time.timeScale = 0;
        SoundManager.instance.PauseChannels();
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(nextScene);
    }

    public void UnPause()
    {
        onPause = false;
        SoundManager.instance.UnPauseChannels();
        Time.timeScale = 1;
    }

    public void NoGravity()
    {
        Physics2D.gravity = new Vector2(0, 0);
    }

    public void RecoverGravity()
    {
        Physics2D.gravity = new Vector2(0, gravity);
    }

    public void Transition(EventType eventType, float time)
    {
        StartCoroutine(TransitionTime(eventType, time));
    }

    private IEnumerator TransitionTime(EventType eventType, float time)
    {
        switch (eventType)
        {
            case EventType.PlayerDeathTransition:
                TriggerAction(ExecuteAction.PlayerDisableEvent);
                yield return new WaitForSeconds(time);
                myAnim.SetTrigger("playerTransition");
                break;
            case EventType.DoorTransition:
                yield return new WaitForSeconds(time);
                StopMovementEvent();
                HealAllEnemiesEvent = delegate { };
                EnemyRespawnEvent = delegate { };
                StopMovementEvent = delegate { };
                ResumeMovementEvent = delegate { };
                ResetBossBattleEvent = delegate { };
                StopMovementEvent += NoGravity;
                ResumeMovementEvent += RecoverGravity;
                StopPlayerMovementEvent();
                myAnim.SetTrigger("doorTransition");
                break;
            case EventType.EndGameTransition:
                myAnim.SetTrigger("endTransition");
                break;
        }
    }
    public void TriggerAction(ExecuteAction actions)
    {
        switch (actions)
        {
            case ExecuteAction.SetupPlayerAttacks:
                SetupPlayerAttacks();
                player.transform.position = nextPosition;
                break;
            case ExecuteAction.EnemyRespawnEvent:
                EnemyRespawnEvent();
                EnemyRespawnEvent = delegate { };
                break;
            case ExecuteAction.HealAllEnemiesEvent:
                HealAllEnemiesEvent();
                break;
            case ExecuteAction.PlayerDisableEvent:
                PlayerDisableEvent();
                break;
            case ExecuteAction.PlayerRespawnEvent:
                PlayerRespawnEvent();
                CameraManager.instance.normalCamera.gameObject.SetActive(true);
                CameraManager.instance.playerCamera.gameObject.SetActive(false);
                break;
            case ExecuteAction.SaveData:
                SaveDataEvent();
                EnemyRespawnEvent = delegate { };
                break;
            case ExecuteAction.LoadData:
                LoadDataEvent();
                break;
            case ExecuteAction.DestroyEvent:
                DestroyEvent();
                break;
            case ExecuteAction.AllwaysRespawnEvent:
                AllwaysRespawnEvent();
                break;
            case ExecuteAction.StopMovementEvent:
                StopPlayerMovementEvent();
                StopMovementEvent();
                break;
            case ExecuteAction.ResumeMovementEvent:
                ResumePlayerMovementEvent();
                ResumeMovementEvent();
                break;
            case ExecuteAction.ResumePlayerMovementEvent:
                ResumePlayerMovementEvent();
                break;
            case ExecuteAction.StopPlayerMovementEvent:
                StopPlayerMovementEvent();
                break;
            case ExecuteAction.EndGame:
                EndGameEvent();
                break;
            case ExecuteAction.ResetBossBattle:
                ResetBossBattleEvent();
                break;
            case ExecuteAction.EnterBossDoor:
                EnterBossDoorEvent();

                break;
        }

    }

    #region Particle Effects


    public void Sound(AudioClip myClip)
    {
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, myClip, transform);
    }

    public void Scream()
    {
        TransitionEvent();
        TransitionEvent = delegate { };
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
    #endregion
}