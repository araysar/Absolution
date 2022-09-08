using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEnd : MonoBehaviour
{
    public void EndAnimation()
    {
        gameObject.SetActive(false);
    }
}
