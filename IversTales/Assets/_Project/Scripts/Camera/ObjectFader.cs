using System.Collections.Generic;
using IversTales.Core;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// "See-through walls" system. Casts a ray from the camera to the player every
/// frame; walls hit by the ray become semi-transparent and get their original
/// look back as soon as they stop blocking the view.
/// </summary>
public class ObjectFader : MonoBehaviour
{
    [SerializeField] LayerMask layerPlayer;
    [Tooltip("Layers that can be faded when they block the view of the player.")]
    [SerializeField] LayerMask layerWalls;
    [Tooltip("Alpha applied to walls between the camera and the player.")]
    [SerializeField, Range(0f, 1f)] float fadedAlpha = 0.26f;
    [Tooltip("Height of the point on the player the camera looks at.")]
    [SerializeField] float playerAimHeight = 1f;

    static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
    static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
    static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
    static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    struct MaterialState
    {
        public Color Color;
        public Color Emission;
        public bool HasEmission;
        public int RenderQueue;
        public float SrcBlend, DstBlend, ZWrite;
        public string RenderType;
    }

    readonly OcclusionTracker<Renderer> tracker = new OcclusionTracker<Renderer>();
    readonly Dictionary<Material, MaterialState> originalStates = new Dictionary<Material, MaterialState>();
    readonly List<Renderer> hitRenderers = new List<Renderer>();
    RaycastHit[] hits = new RaycastHit[16];
    PlayerController player;

    void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    void OnDisable()
    {
        tracker.Clear();
        foreach (var renderer in tracker.StoppedOccluding) Restore(renderer);
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 origin = transform.position;
        Vector3 target = player.transform.position + Vector3.up * playerAimHeight;
        Vector3 toPlayer = target - origin;
        float distance = toPlayer.magnitude;

        hitRenderers.Clear();
        int count = Physics.RaycastNonAlloc(origin, toPlayer / distance, hits, distance, layerWalls, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < count; i++)
        {
            int layerBit = 1 << hits[i].transform.gameObject.layer;
            if ((layerPlayer.value & layerBit) != 0) continue;
            var renderer = hits[i].transform.GetComponent<Renderer>();
            if (renderer != null) hitRenderers.Add(renderer);
        }

        tracker.Update(hitRenderers);
        foreach (var renderer in tracker.StartedOccluding) Fade(renderer);
        foreach (var renderer in tracker.StoppedOccluding) Restore(renderer);
    }

    void Fade(Renderer renderer)
    {
        if (renderer == null) return;
        foreach (var material in renderer.materials)
        {
            if (!originalStates.ContainsKey(material))
                originalStates[material] = Capture(material);

            material.SetOverrideTag("RenderType", "Transparent");
            material.SetFloat(SrcBlend, (float)BlendMode.One);
            material.SetFloat(DstBlend, (float)BlendMode.OneMinusSrcAlpha);
            material.SetFloat(ZWrite, 0f);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)RenderQueue.Transparent;

            var state = originalStates[material];
            if (state.HasEmission)
            {
                float brightness = state.Emission.maxColorComponent;
                if (brightness > 0f)
                    material.SetColor(EmissionColor, state.Emission / brightness * fadedAlpha);
            }
            else
            {
                Color faded = state.Color;
                faded.a = fadedAlpha;
                material.color = faded;
            }
        }
    }

    void Restore(Renderer renderer)
    {
        if (renderer == null) return;
        foreach (var material in renderer.materials)
        {
            if (!originalStates.TryGetValue(material, out var state)) continue;

            material.SetOverrideTag("RenderType", state.RenderType);
            material.SetFloat(SrcBlend, state.SrcBlend);
            material.SetFloat(DstBlend, state.DstBlend);
            material.SetFloat(ZWrite, state.ZWrite);
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = state.RenderQueue;
            material.color = state.Color;
            if (state.HasEmission) material.SetColor(EmissionColor, state.Emission);

            originalStates.Remove(material);
        }
    }

    static MaterialState Capture(Material material)
    {
        bool hasEmission = material.IsKeywordEnabled("_EMISSION") && material.HasProperty(EmissionColor);
        return new MaterialState
        {
            Color = material.color,
            HasEmission = hasEmission,
            Emission = hasEmission ? material.GetColor(EmissionColor) : Color.black,
            RenderQueue = material.renderQueue,
            SrcBlend = material.HasProperty(SrcBlend) ? material.GetFloat(SrcBlend) : (float)BlendMode.One,
            DstBlend = material.HasProperty(DstBlend) ? material.GetFloat(DstBlend) : (float)BlendMode.Zero,
            ZWrite = material.HasProperty(ZWrite) ? material.GetFloat(ZWrite) : 1f,
            RenderType = material.GetTag("RenderType", false, "")
        };
    }
}
