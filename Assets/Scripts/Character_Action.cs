using UnityEngine;
using UnityEngine.Events;

public class Character_Action : MonoBehaviour
{
    [SerializeField] private UnityEvent[] actions;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            actions[0]?.Invoke();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            actions[1]?.Invoke();
        }

    }
}
