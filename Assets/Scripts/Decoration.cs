using UnityEngine;

public class Decoration : MonoBehaviour
{
    public float speed;
    public float verticalSpeed;
    public int direction;

    void FixedUpdate()
    {
        transform.Translate(new Vector3(direction * speed, verticalSpeed, 0));

        if (transform.position.x > 20) transform.position = new Vector3(-19, transform.position.y, 0);
        if (transform.position.x < -20) transform.position = new Vector3(19, transform.position.y, 0);
    }
}
