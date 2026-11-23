using UnityEngine;

/// <summary>
/// Platform that is invisible until the player casts the light spell. The spell
/// plays a dissolve effect ("_Dissolve" shader property) that makes the platform
/// appear; casting it again makes it disappear. The collider is always active,
/// so the platform can be walked on even when it cannot be seen.
/// </summary>
public class HiddenPlatform : MonoBehaviour
{
    public AnimationCurve fadeIn;
    public float spawnEffectTime = 2;
    public float pause = 1;
    [Tooltip("Speed of the dissolve effect (full cycle per second).")]
    [SerializeField] float dissolveSpeed = 1f;

    static readonly int DissolveProperty = Shader.PropertyToID("_Dissolve");

    Renderer rend;
    Material[] materials;
    bool initiallyVisible;
    bool isPlayerOnPlatform;
    bool effectRunning;
    bool appearing = true;
    float dissolve;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        materials = rend.materials; // instanced once, reused by the effect
        initiallyVisible = rend.enabled;
        rend.enabled = false;
    }

    void Update()
    {
        if (!effectRunning) return;

        if (appearing)
        {
            dissolve -= Time.deltaTime * dissolveSpeed;
            if (dissolve <= 0f) { dissolve = 0f; appearing = false; effectRunning = false; }
        }
        else
        {
            dissolve += Time.deltaTime * dissolveSpeed;
            if (dissolve >= 1f) { dissolve = 1f; appearing = true; effectRunning = false; rend.enabled = false; }
        }

        foreach (var material in materials)
            material.SetFloat(DissolveProperty, dissolve);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) isPlayerOnPlatform = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) isPlayerOnPlatform = false;
    }

    /// <summary>Called by the light spell. Ignored while the player is standing on the platform.</summary>
    public void Activate()
    {
        if (isPlayerOnPlatform) return;
        effectRunning = true;
        rend.enabled = true;
        dissolve = appearing ? 1f : 0f;
    }

    public void RestoreInitialState()
    {
        if (!isPlayerOnPlatform) rend.enabled = initiallyVisible;
    }
}
