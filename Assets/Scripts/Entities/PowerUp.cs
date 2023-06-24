using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public Character_Movement.PowerUp myPower;
    private bool isGrabed = false;
    [SerializeField] private GameObject uiMessage;
    [SerializeField] private AudioClip getSound;
    [SerializeField] private Animator myAnim;
    [SerializeField] private Color myColor;
    [SerializeField] private AudioClip myGetSound;


    private void Start()
    {
        myAnim = GetComponent<Animator>();
        myAnim.SetBool("exit", false);
        myAnim.SetBool("enter", false);
        if (Character_Movement.instance.myUpgrades.Contains(myPower))
        {
            isGrabed = true;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Character_Movement>() != null && !isGrabed)
        {
            if(!isGrabed) isGrabed = true;

            GameManager.instance.Pause();
            SoundManager.instance.PlaySound(SoundManager.SoundChannel.Unscalled, getSound, transform);
            uiMessage.SetActive(true);
            myAnim.SetBool("enter", true);
        }
    }

    public void Respawn()
    {
        isGrabed = false;
        gameObject.SetActive(true);
        GameManager.instance.EnemyRespawnEvent += Respawn;
        myAnim.SetBool("exit", false);
        myAnim.SetBool("enter", false);
    }

    public void BTN_Exit()
    {
        myAnim.SetBool("enter", false);
        myAnim.SetBool("exit", true);
    }

    public void ExitAnimation()
    {
        GameManager.instance.UnPause();
        Character_Movement.instance.myUpgrades.Add(myPower);
        Character_Movement.instance.PowerUpGrab();
        GameManager.instance.EnemyRespawnEvent += Respawn;
        Character_Movement.instance.NewPowerUpEffect(myColor);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Unscalled, myGetSound, transform);
        uiMessage.SetActive(false);
        gameObject.SetActive(false);
    }
}
