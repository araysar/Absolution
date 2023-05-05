using UnityEngine;

public abstract class Attack_Type : MonoBehaviour
{
    public string weaponName;
    public AudioClip[] soundClips;
    public Color myColor;
    public float damage;
    public bool isAttacking;
    public Sprite myImage;
    public Character_Attack myAttack;
    public Character_Movement player;
    public abstract void Setup();
    public abstract void Interrupt();
    public abstract void PrimaryAttack();
    public abstract void SecondaryAttack();
    public abstract void EndAttack();
    public abstract void EnteringMode();

    private void Awake()
    {
        player = GetComponentInParent<Character_Movement>();
        myAttack = GetComponentInParent<Character_Attack>();
    }
}
