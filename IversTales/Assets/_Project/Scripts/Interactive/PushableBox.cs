using UnityEngine;

/// <summary>Box that the wind spell pushes a fixed distance.</summary>
public class PushableBox : MonoBehaviour
{
    /// <summary>Moves the box <paramref name="distance"/> units along <paramref name="direction"/>.</summary>
    public void Move(Vector3 direction, float distance)
    {
        transform.position += direction.normalized * distance;
    }
}
