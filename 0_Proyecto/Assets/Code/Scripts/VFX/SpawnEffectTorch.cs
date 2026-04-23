using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnEffectTorch : MonoBehaviour
{
    public float spawnEffectTime = 2;
    public float pause = 1;
    public AnimationCurve fadeIn;
    ParticleSystem ps;
    float timer = 0;
    Renderer _renderer;
    int shaderProperty;
    
    public bool enableEffect = true;
    public bool disableMesh = false;

    void Start()
    {
        shaderProperty = Shader.PropertyToID("_cutoff");
        _renderer = GetComponentInChildren<Renderer>();
        ps = GetComponentInChildren<ParticleSystem>();

        var main = ps.main;
        main.duration = spawnEffectTime;

    }
    public void StartDissableDeadEffect(GameObject go)
    {
        this.gameObject.GetComponent<Collider>().enabled = false;
        ps.Play();
        enableEffect = true;

    }
    void Update()
    {
        if (enableEffect)
        {
            if (timer < spawnEffectTime + pause)
            {
                timer += Time.deltaTime;
            }
            else
            {
                enableEffect = false;
                timer = 0;
                if (disableMesh)
                {
                    DesactiveMesh();

                }

            }


            // Iterar a través de todos los materiales del Renderer y aplicar el efecto disolver
            foreach (Material material in _renderer.materials)
            {
                float time = fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer));
                if (time < 1)
                {
                    material.SetFloat(shaderProperty, time);

                }
            }
        }

    }
    void DesactiveMesh()
    {
        if (this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>())
            this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = (false);
        else
            this.gameObject.GetComponentInChildren<MeshRenderer>().enabled = (false);

        gameObject.GetComponent<BoxCollider>().enabled = false;

    }
}
