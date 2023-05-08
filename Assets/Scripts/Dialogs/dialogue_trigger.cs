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
    public GameObject actionButton;
    [TextArea(3, 6)] public string[] dialogueLines;
    public float typingTime = 0.05f;
    public bool oneTime = false;
    public bool triggerOnRange = false;
    private bool onRange = false;
    private bool isTalking = false;
    private int lineIndex;
    public Action MyAction;
    private bool enable = false;

    private void Start()
    {
        StartCoroutine(TimeToStart());
    }
    private void Update()
    {
        if (onRange && enable)
        {
            if (!isTalking && (Input.GetButtonDown("Action") || triggerOnRange))
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
    }

    IEnumerator TimeToStart()
    {
        yield return new WaitForEndOfFrame();
        actionButton.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        actionButton.SetActive(true);
        enable = true;
    }
    private void StartDialogue()
    {
        isTalking = true;
        lineIndex = 0; 
        actionButton.SetActive(false);
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
            actionButton.SetActive(true);
            GameManager.instance.UnPause();
            if (oneTime)
            {
                gameObject.SetActive(false);
            }
        }
    }

    IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;

        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSecondsRealtime(typingTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Character_Attack>() != null)
        {
            actionButton.SetActive(true);
            onRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Character_Attack>() != null)
        {
            actionButton.SetActive(false);
            onRange = false;
        }
    }
}
