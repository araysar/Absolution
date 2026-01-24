using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class dialogue_trigger : MonoBehaviour
{
    [Header("Dialogue")]
    public GameObject dialogueMenu;
    public TMP_Text dialogueText;
    public Image dialogueImage;
    public AudioSource myAudio;
    [TextArea(3, 6)] public string[] dialogueLines;
    public float typingTime = 0.05f;
    public int charsToPlaySound = 6;
    private bool isTalking = false;
    private int lineIndex;
    public Action MyAction = delegate { };
    public float timeToTrigger = 0;
    public int dialogueNumber = 0;

    private void Start()
    {
        myAudio = GetComponent<AudioSource>();
        SoundManager.instance.audioSources.Add(myAudio);
        myAudio.volume = SoundManager.instance.sfxVolume;
        if (GameManager.instance.saveManager.dialogues.Contains(dialogueNumber))
        {
            gameObject.SetActive(false);
        }

    }
    private void Update()
    {
        MyAction();
    }

    private void Action()
    {
        if (!isTalking)
        {
            StartDialogue();
        }
        else if (isTalking && dialogueText.text == dialogueLines[lineIndex] && Input.anyKeyDown)
        {
            NextDialogueLine();
        }
        else if (isTalking && dialogueText.text != dialogueLines[lineIndex] && Input.anyKeyDown)
        {
            StopAllCoroutines();
            dialogueText.text = dialogueLines[lineIndex];
        }
    }

    private void StartDialogue()
    {
        isTalking = true;
        lineIndex = 0; 
        GameManager.instance.Pause();
        dialogueMenu.SetActive(true);
        StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        lineIndex++;
        if(lineIndex < dialogueLines.Length)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            isTalking = false;
            dialogueMenu.SetActive(false); 
            GameManager.instance.UnPause();
            GameManager.instance.isBusy = false;
            GameManager.instance.player.disableInputs = false;
            GameManager.instance.player.ui.SetActive(true);
            GameManager.instance.player.ResumeMovement();
            GameManager.instance.saveManager.dialogues.Add(dialogueNumber);
            MyAction = delegate { };
            GameManager.instance.EnemyRespawnEvent += Respawn;
            gameObject.SetActive(false);
        }
    }

    private void Respawn()
    {
        GameManager.instance.saveManager.dialogues.Remove(dialogueNumber);
        gameObject.SetActive(true);
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(timeToTrigger);
        MyAction = Action;

    }
    public void EnableDialogue()
    {
        GameManager.instance.isBusy = true;
    }
    IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;
        int charIndex = 0;

        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            if(charIndex % charsToPlaySound == 0) myAudio.Play();
            charIndex++;
            yield return new WaitForSecondsRealtime(typingTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Character_Attack>() != null)
        {
            StartCoroutine(Timer());
        }
    }
}
