using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }

    void Start()
    {
        Destroy(gameObject, 5f);
    }
}