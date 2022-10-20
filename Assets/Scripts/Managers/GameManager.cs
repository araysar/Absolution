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
    [SerializeField] private float timeToTransition = 3;

    public event Action EnemyRespawnEvent = delegate { };
    public event Action AllwaysRespawnEvent = delegate { };
    public event Action HealAllEnemiesEvent = delegate { };
    public event Action PlayerDisableEvent = delegate { };
    public event Action PlayerRespawnEvent = delegate { };
    public event Action SaveDataEvent = delegate { };
    public event Action LoadDataEvent = delegate { };
    public event Action DestroyEvent = delegate { };
    public event Action StartEvent = delegate { };

    [Header("Death")]
    [SerializeField] private GameObject commonEnemyDeathEffect;
    [SerializeField] private GameObject playerDeathEffect;

    [Header("NextZone")]
    [HideInInspector] public int nextScene = 1;
    [HideInInspector] public Vector2 nextPosition = Vector2.zero;



    public enum EventType
    {
        PlayerDeathTransition,
        DoorTransition,
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
        
    };

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        player = FindObjectOfType<Character_Movement>();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        myAnim = GetComponent<Animator>();

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            TransitionEvent(EventType.PlayerDeathTransition);
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


    public void TransitionEvent(EventType eventType)
    {
        switch (eventType)
        {
            case EventType.PlayerDeathTransition:
                myAnim.SetTrigger("playerTransition");
                break;
            case EventType.DoorTransition:
                myAnim.SetTrigger("doorTransition");
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
        }

    }

    #region Death Effects

    public void DeathEffect(Health.EntityType myType, GameObject myObject)
    {
        switch (myType)
        {
            case Health.EntityType.common:
                Instantiate(commonEnemyDeathEffect, myObject.transform.position, Quaternion.identity);
                break;
            case Health.EntityType.special:
                break;
            case Health.EntityType.boss:
                break;
            case Health.EntityType.player:
                Instantiate(playerDeathEffect, myObject.transform.position, Quaternion.identity);
                break;
            default:
                Instantiate(commonEnemyDeathEffect, myObject.transform.position, Quaternion.identity);
                break;
        }

    }

    #endregion
}