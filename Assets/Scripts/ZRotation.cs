using UnityEngine;

public class ZRotation : MonoBehaviour
{
    [SerializeField] Vector3 speed;

    void Update()
    {
        transform.Rotate(speed * Time.deltaTime);
    }
}
