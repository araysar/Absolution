using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Character_Movement player;
    private Animator myAnim;
    public bool onPause = false;
    private float gravity;

    public event Action EnemyRespawnEvent = delegate { };
    public event Action AllwaysRespawnEvent = delegate { };
    public event Action HealAllEnemiesEvent = delegate { };
    public event Action PlayerDisableEvent = delegate { };
    public event Action PlayerRespawnEvent = delegate { };
    public event Action SaveDataEvent = delegate { };
    public event Action LoadDataEvent = delegate { };
    public event Action DestroyEvent = delegate { };
    public event Action StartEvent = delegate { };
    public event Action StopMovementEvent = delegate { };
    public event Action ResumeMovementEvent = delegate { };
    public event Action StopPlayerMovementEvent = delegate { };
    public event Action ResumePlayerMovementEvent = delegate { };
    public event Action TransitionEvent = delegate { };

    [Header("Particles")]
    [SerializeField] private GameObject commonEnemyDeathEffect;
    [SerializeField] private GameObject playerDeathEffect;
    [SerializeField] private GameObject screamParticleEffect;

    [Header("NextZone")]
    [HideInInspector] public int nextScene = 1;
    [HideInInspector] public Vector2 nextPosition = Vector2.zero;



    public enum EventType
    {
        PlayerDeathTransition,
        DoorTransition,
        FadeTransition,
    };

    public enum ParticleType
    {
        CommonEnemyDeathEffect,
        PlayerDeathEffect,
        ScreamParticleEffect,
    };

    public enum ExecuteAction
    {
        EnemyRespawnEvent,
        HealAllEnemiesEvent,
        PlayerDisableEvent,
        PlayerRespawnEvent,
        SaveData,
        LoadData,
        DestroyEvent,
        StartEvent,
        AllwaysRespawnEvent,
        StopMovementEvent,
        ResumeMovementEvent,
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            GameManager.instance.DestroyEvent += Destroy;
        }
        else
        {
            Destroy(gameObject);
        }
        player = FindObjectOfType<Character_Movement>();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        gravity = Physics2D.gravity.y;
        StopMovementEvent += NoGravity;
        ResumeMovementEvent += RecoverGravity;
        StartEvent += RecoverGravity;
        myAnim = GetComponent<Animator>();

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {

        }
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
        Character_Movement.instance.gameObject.transform.position = nextPosition;
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
                StopMovementEvent();
                StopMovementEvent = delegate { };
                StopPlayerMovementEvent();
                myAnim.SetTrigger("doorTransition");
                break;
            case EventType.FadeTransition:
                player.StopMovement();
                myAnim.SetTrigger("endTransition");
                break;
        }
    }
    public void TriggerAction(ExecuteAction actions)
    {
        switch (actions)
        {
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
            case ExecuteAction.StartEvent:
                StartEvent();
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
        }

    }

    #region Particle Effects

    public void ParticleEffect(ParticleType myType, GameObject myObject)
    {
        switch (myType)
        {
            case ParticleType.CommonEnemyDeathEffect:
                Instantiate(commonEnemyDeathEffect, myObject.transform.position, Quaternion.identity);
                break;
            case ParticleType.PlayerDeathEffect:
                Instantiate(playerDeathEffect, myObject.transform.position, Quaternion.identity);
                break;
            case ParticleType.ScreamParticleEffect:
                Instantiate(screamParticleEffect, myObject.transform.position, Quaternion.identity);
                break;
            default:
                Instantiate(commonEnemyDeathEffect, myObject.transform.position, Quaternion.identity);
                break;
        }

    }

    public void Sound(AudioClip myClip)
    {
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, myClip);
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