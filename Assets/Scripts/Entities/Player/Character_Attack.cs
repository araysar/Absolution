using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Character_Attack : MonoBehaviour
{
    private Character_Movement player;
    public bool canAttack = true;

    //Attack system
    public Cube myCubePrefab;
    public Cube myCube;
    public Transform cubeTransform;
    public Attack_Type[] myAttacks;
    public Attack_Type currentAttack;
    public float timeToShuffle = 30;
    public float currentTime;
    public bool shuffleActivated = true;
    public TMP_Text timerText;
    public TMP_Text nameText;
    public Image uiImage;

    public void ActivateShuffle()
    {
        shuffleActivated = true;
    }

    public void DisableShuffle()
    {
        shuffleActivated = false;
    }

    public void ChangeWeapon()
    {
        int nextWeapon = Random.Range(0, myAttacks.Length);

        if (myAttacks[nextWeapon] == currentAttack)
        {
            ChangeWeapon();
        }
        else
        {
            currentAttack.EndAttack();
            currentAttack = myAttacks[nextWeapon];
            currentTime = timeToShuffle; 
            AttackCube(true);
            uiImage.sprite = currentAttack.myImage;
        }
    }

    private void Awake()
    {
        CreateCube();
        player = GetComponent<Character_Movement>();
        currentTime = timeToShuffle;
        currentAttack = myAttacks[0];// myAttacks[Random.Range(0, myAttacks.Length)];
        uiImage.sprite = currentAttack.myImage;
    }

    public void CreateCube()
    {
        if(myCube == null)
        {
            myCube = Instantiate(myCubePrefab);
            myCube.gameObject.transform.position = cubeTransform.position;
            myCube.myDestination = cubeTransform;
        }
    }

    private void Update()
    {
        if(!GameManager.instance.onPause)
        {
            if (!player.disableInputs && canAttack)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (!currentAttack.isAttacking)
                    {
                        currentAttack.PrimaryAttack();
                    }
                }
                else if (Input.GetButtonDown("Fire2"))
                {
                    if (!currentAttack.isAttacking)
                    {
                        currentAttack.SecondaryAttack();
                    }
                }
            }
            if(shuffleActivated)
            {
                currentTime -= Time.deltaTime;
                TimerUI();
            }
            if(currentTime <= 0 && shuffleActivated)
            {
                ChangeWeapon();
            }
        }
    }


    public void AttackCube(bool value)
    {
        myCube.gameObject.SetActive(value);
    }

    private void TimerUI()
    {
       // timerText.text = Mathf.RoundToInt(currentTime).ToString();
    }

}
