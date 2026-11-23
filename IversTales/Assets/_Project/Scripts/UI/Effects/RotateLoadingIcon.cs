using UnityEngine;

/// <summary>Spins the loading icon. Uses unscaled time so it keeps turning while the game is paused.</summary>
public class RotateLoadingIcon : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 200;

    void LateUpdate()
    {
        transform.Rotate(0f, 0f, -rotationSpeed * Time.unscaledDeltaTime);
    }
}
