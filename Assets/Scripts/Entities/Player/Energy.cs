using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Energy : MonoBehaviour
{
    public float maxEnergy;
    public float currentEnergy;
    [SerializeField] private float regenPerTick = 5;
    public float delayRegenInSeconds = 1;
    public Image uiEnergy;
    public GameObject[] uiGameObject;
    [HideInInspector] public Action EnergyRegen;
    private Coroutine regenCoroutine;

    void Start()
    {
        EnergyRegen = Regeneration;
    }

    void Update()
    {
        EnergyRegen();
    }

    public void Regeneration()
    {
        if (regenCoroutine != null) return;
        if (currentEnergy == maxEnergy) return;

        regenCoroutine = StartCoroutine(RegenerationTimer());
    }

    public void ReloadEnergy()
    {
        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
        uiEnergy.fillAmount = currentEnergy / maxEnergy;
    }

    private IEnumerator RegenerationTimer()
    {
        yield return new WaitForSeconds(delayRegenInSeconds);
        currentEnergy += regenPerTick;
        ReloadEnergy();
        regenCoroutine = null;
    }
}
