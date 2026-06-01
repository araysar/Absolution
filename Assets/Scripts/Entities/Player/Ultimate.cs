using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Ultimate: MonoBehaviour
{
    public bool canUse = true;
    public float chargingTime = 1f;
    public float skillTime = 2.5f;
    [SerializeField] private float regenerationDelay = 0.5f;
    [SerializeField] private float regenerationPerTick = 1;
    [SerializeField] private string inputName = "Ultimate1";
    [SerializeField] private Animator ultimateAnimator;

    [SerializeField] private Image uiImage;
    [SerializeField] private Image uiWhiteImage;
    [SerializeField] private GameObject readyText;
    [SerializeField] private GameObject readyVfx;

    [SerializeField] private AudioClip chargeSfx;
    [SerializeField] private AudioClip launch1Sfx;
    [SerializeField] private AudioClip launch2Sfx;
    [SerializeField] private AudioClip castVoice;
    [SerializeField] private AudioClip launchVoice;
    Coroutine regenerationAction;

    public bool ultiReady = false;

    [Header("Configuración Giga Crush")]
    public float damage = 100f;
    public LayerMask capaEnemigos;

    void Start()
    {
        Check();
    }

    public void Check()
    {
        if (Character_Movement.instance.ulti1Stacks >= Character_Movement.instance.ulti1Required)
        {
            Character_Movement.instance.ulti1Stacks = Character_Movement.instance.ulti1Required;
            readyText.SetActive(true);
            readyVfx.SetActive(true);
            ultimateAnimator.SetTrigger("ready");
            ultiReady = true;
        }
        else
        {
            readyText.SetActive(false);
            readyVfx.SetActive(false);
            ultiReady = false;
            ultimateAnimator.SetTrigger("notReady");
        }
        uiWhiteImage.fillAmount = Character_Movement.instance.ulti1Stacks / Character_Movement.instance.ulti1Required;
        uiImage.fillAmount = Character_Movement.instance.ulti1Stacks / Character_Movement.instance.ulti1Required;
    }
    private void Update()
    {
        if (regenerationAction == null)
        {
            regenerationAction = StartCoroutine(Regeneration());
        }
    }

    public void RefreshStacks(bool changeState)
    {
        if(Character_Movement.instance.ulti1Stacks >= Character_Movement.instance.ulti1Required && !ultiReady)
        {
            Character_Movement.instance.ulti1Stacks = Character_Movement.instance.ulti1Required;
            uiWhiteImage.fillAmount = 1;
            uiImage.fillAmount = 1;
            ultiReady = true;
            readyVfx.SetActive(true);
            readyText.SetActive(true);
            ultimateAnimator.SetTrigger("ready");

        }
        else if(Character_Movement.instance.ulti1Stacks <= Character_Movement.instance.ulti1Required && !ultiReady)
        {
            readyText.SetActive(false);
            uiImage.fillAmount = Character_Movement.instance.ulti1Stacks / Character_Movement.instance.ulti1Required;
            uiWhiteImage.fillAmount = uiImage.fillAmount;
            if(changeState) ultimateAnimator.SetTrigger("notReady");
        }
    }

    public void ActivateUltimate()
    {
        if(canUse && ultiReady)
        {
            Character_Movement.instance.isCharging = true;
            Character_Movement.instance.pauseTraps = true;
            ultiReady = false;
            GameManager.instance.TriggerAction(GameManager.ExecuteAction.StopMovementEvent);
            Character_Movement.instance.ControlAnimations();
        }
    }

    IEnumerator Regeneration()
    {
        yield return new WaitForSeconds(regenerationDelay);
        
        if (!Character_Movement.instance.ulti1.ultiReady)
        {
            Character_Movement.instance.ulti1Stacks += regenerationPerTick;
        }
        Character_Movement.instance.ulti1.RefreshStacks(false);
        regenerationAction = null;

    }
    private void ChargingUltimate()
    {
        Character_Movement.instance.myHealth.invulnerable = true;
        Character_Movement.instance.ulti1Stacks = 0;
        RefreshStacks(true);
        ultimateAnimator.SetTrigger("notReady");
        readyText.SetActive(false);
        readyVfx.SetActive(false);
        if (chargeSfx != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, chargeSfx, transform);
        if (castVoice != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, castVoice, transform);
    }
    private void UsingUltimate()
    {
        if (launch1Sfx != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, launch1Sfx, transform);
        if (launch2Sfx != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, launch2Sfx, transform);
        if (castVoice != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, launchVoice, transform);
        Character_Movement.instance.isCharging = false;
        Character_Movement.instance.isUlting = true;
        Character_Movement.instance.ControlAnimations();
    }
    private void EndUltimate()
    {
        Character_Movement.instance.pauseTraps = false;
        Character_Movement.instance.myHealth.invulnerable = false;
        GameManager.instance.TriggerAction(GameManager.ExecuteAction.ResumeMovementEvent);
        Character_Movement.instance.isUlting = false;
    }
    
    // Esta función se llama en el frame exacto de la animación donde explota
    public void EjecutarExplosionPantalla()
    {
        // 1. OBTENEMOS EL TAMAÑO EXACTO DE LA PANTALLA
        Camera cam = Camera.main;

        // El Alto es siempre el Orthographic Size multiplicado por 2
        float altoPantalla = cam.orthographicSize * 2f;

        // El Ancho se calcula multiplicando el Alto por el Aspect Ratio (ej: 16:9)
        float anchoPantalla = altoPantalla * cam.aspect;
        Vector2 tamañoPantalla = new Vector2(anchoPantalla, altoPantalla);

        // 2. BUSCAMOS A TODOS LOS ENEMIGOS EN ESE RECUADRO
        // Usamos la posición de la cámara como centro de nuestra caja de impacto
        Collider2D[] enemigosGolpeados = Physics2D.OverlapBoxAll(cam.transform.position, tamañoPantalla, 0f, capaEnemigos);

        // 3. APLICAMOS EL DAÑO
        foreach (Collider2D enemigo in enemigosGolpeados)
        {
            // Aquí llamas a tu script de vida del enemigo. Ejemplo:
            if(enemigo.GetComponent<IDamageable>() != null) enemigo.GetComponent<IDamageable>().TakeDamage(damage);
        }
    }

    // --- PARA DIBUJAR LA CAJA EN EL EDITOR Y VER QUE FUNCIONE ---
    private void OnDrawGizmosSelected()
    {
        if (Camera.main != null)
        {
            Camera cam = Camera.main;
            float alto = cam.orthographicSize * 2f;
            float ancho = alto * cam.aspect;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(cam.transform.position, new Vector3(ancho, alto, 1f));
        }
    }
}
