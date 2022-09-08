using UnityEngine;
using UnityEngine.Events;

public class Character_Action : MonoBehaviour
{
    [SerializeField] private UnityEvent[] actions;
    [HideInInspector] public bool castingUltimate = false;
    private Character_Movement charMove;

    private void Start()
    {
        charMove = GetComponent<Character_Movement>();
    }

    void Update()
    {
        if(!GameManager.instance.onPause)
        {
            if(!charMove.disableInputs)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    actions[0]?.Invoke();
                }
                if (Input.GetButtonDown("Fire2"))
                {
                    actions[1]?.Invoke();
                }
                if (Input.GetButtonDown("Ultimate1"))
                {
                    actions[2]?.Invoke();
                }
            }
        }
    }
}
