    using UnityEngine;

public abstract class Attack_Type : MonoBehaviour
{
    public float currentEnergy = 100;
    public float maxEnergy = 100;
    public float regenEnergyRate = 1;
    public float energyPerShot = 10;
    public string weaponName;
    public AudioClip[] soundClips;
    public Color myColor;
    public float damage;
    public bool isAttacking;
    public Sprite myImage;
    public Character_Attack myAttack;
    public Character_Movement player;
    public abstract void Setup();
    public abstract void CreateResource();
    public abstract void Interrupt();

    public virtual void PrimaryAttack()
    {
        currentEnergy -= energyPerShot;
    }

    public abstract void SecondaryAttack();
    public abstract void EndAttack();
    public abstract void EnteringMode();


    private void Awake()
    {
        player = GetComponentInParent<Character_Movement>();
        myAttack = GetComponentInParent<Character_Attack>();
    }

    protected void Update()
    {
        Regeneration();
    }

    private void Regeneration()
    {
        if(currentEnergy < maxEnergy)
        {
            currentEnergy += regenEnergyRate * Time.deltaTime;

            if (currentEnergy > maxEnergy)
            {
                currentEnergy = maxEnergy;
            }
            else if (currentEnergy < 0) currentEnergy = 0;
        }
    }
}
