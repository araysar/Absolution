using UnityEngine;

public class WeaponObstacle : MonoBehaviour
{
    public Action_Shoot.AttackType attackType;

    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private AudioClip destroySfx;
    [SerializeField] private DamageStay myDamage;
    private void Start()
    {
        GameManager.instance.EnemyRespawnEvent += Enable;
    }

    private void Enable()
    {
        gameObject.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (attackType)
        {
            case Action_Shoot.AttackType.FrozenDaggers:
                break;
            case Action_Shoot.AttackType.PyroSphere:
                if(collision.gameObject.GetComponent<PyroSphere_Explosion>() != null)
                {
                    Destroy();
                }
                break;
        }
    }
    public void Destroy()
    {
        if (destroyEffect != null) Instantiate(destroyEffect, transform.position, Quaternion.identity);
        if (destroySfx != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, destroySfx);

        gameObject.SetActive(false);
    }
}