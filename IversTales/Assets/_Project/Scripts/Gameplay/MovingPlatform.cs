using UnityEngine;

/// <summary>
/// Moves back and forth between two points. The player is parented to the
/// platform while standing on it so it is carried along.
/// </summary>
public class MovingPlatform : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float speed = 2.0f;
    [SerializeField] float arrivalThreshold = 0.1f;

    Transform target;

    void Start()
    {
        target = endPoint;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target.position) < arrivalThreshold)
            target = target == startPoint ? endPoint : startPoint;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.SetParent(transform);
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.transform.parent == transform)
            collision.transform.SetParent(null);
    }
}
