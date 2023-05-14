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
    [TextArea(3, 6)] public string[] dialogueLines;
    public float typingTime = 0.05f;
    public bool oneTime = false;
    private bool isTalking = false;
    private int lineIndex;
    public Action MyAction = delegate { };

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
            MyAction = Action;
        }
    }
}
