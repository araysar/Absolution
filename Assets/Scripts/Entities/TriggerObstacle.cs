using UnityEngine;

public class TriggerObstacle : MonoBehaviour
{
    public IObstacle myObstacle;
    public float speed;
    public int myNumber;

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, myObstacle.transform.position, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == myObstacle.gameObject)
        {
            myObstacle.Trigger(myNumber);
            gameObject.SetActive(false);
        }
    }
}
