using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    [SerializeField] private TriggerBossDoor myTrigger;
    [SerializeField] private GameObject myCamera;

    private event Action ResetFightAction;
    void Start()
    {
        ResetFightAction += myTrigger.ResetPosition;
    }

    public void TriggerBattle()
    {
        
    }
}
