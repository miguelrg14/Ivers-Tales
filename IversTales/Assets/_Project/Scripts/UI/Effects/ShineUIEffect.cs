using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sweeps a shine across a UI image by animating the "_ShineLocation" property of
/// its material from 1 to 0, then waits and repeats.
/// </summary>
[RequireComponent(typeof(Image))]
public class ShineUIEffect : MonoBehaviour
{
    [Tooltip("Seconds the shine takes to cross the image.")]
    public float shineDuration = 5.0f;
    [Tooltip("Seconds to wait before the next sweep.")]
    public float pauseTime = 3.0f;

    static readonly int ShineLocation = Shader.PropertyToID("_ShineLocation");

    Material material;

    void Start()
    {
        var image = GetComponent<Image>();
        // Image.material returns the shared asset: animating it would change the
        // material file in the editor and every image using it. Work on a copy.
        material = new Material(image.material);
        image.material = material;
        StartCoroutine(AnimateShine());
    }

    void OnDestroy()
    {
        if (material != null) Destroy(material);
    }

    IEnumerator AnimateShine()
    {
        var pause = new WaitForSecondsRealtime(pauseTime);
        while (true)
        {
            for (float timer = 0f; timer < shineDuration; timer += Time.unscaledDeltaTime)
            {
                material.SetFloat(ShineLocation, Mathf.Lerp(1f, 0f, timer / shineDuration));
                yield return null;
            }
            material.SetFloat(ShineLocation, 0f);
            yield return pause;
        }
    }
}
