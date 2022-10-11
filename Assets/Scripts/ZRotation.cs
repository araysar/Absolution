using UnityEngine;

public class ZRotation : MonoBehaviour
{
    [SerializeField] private Vector3 speed;

    void Update()
    {
        transform.Rotate(speed  * Time.deltaTime);
    }
}
