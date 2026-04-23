using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnEffect : MonoBehaviour
{
    public float spawnEffectTime = 2;
    public float pause = 1;
    public AnimationCurve fadeIn;
    ParticleSystem ps;
    float timer = 0;
    Renderer _renderer;
    int shaderProperty;
    bool enableEffect=false;
    bool enableDesactivateGO=false;

    GameObject gameObjectToDesactivate;


    private void Awake()
    {
        shaderProperty = Shader.PropertyToID("_cutoff");
        _renderer = GetComponentInChildren<Renderer>();
        ps = GetComponentInChildren<ParticleSystem>();
        ps.Stop();
        var main = ps.main;
        main.duration = spawnEffectTime;
    }
    public void StartDissableDeadEffect(GameObject go)
    {
        this.gameObject.GetComponent<Collider>().enabled = false;
        ps.Play();
        enableEffect = true;
        gameObjectToDesactivate = go;

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
                enableDesactivateGO = true;
                DesactiveMesh();

            }


            // Iterar a través de todos los materiales del Renderer y aplicar el efecto disolver
            foreach (Material material in _renderer.materials)
            {
                material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer)));
            }

        }
        else if (enableDesactivateGO)
        {
            //DesactiveMesh();
            //material.SetFloat(shaderProperty,0);
            if (!ps.IsAlive()) 
            {
                enableDesactivateGO = false;
                if (gameObjectToDesactivate != null)
                    gameObjectToDesactivate.SetActive(false);
            }
        }

    }
    void DesactiveMesh()
    {
        if (this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>())
            this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = (false);
        else
            this.gameObject.GetComponentInChildren<MeshRenderer>().enabled = (false);

    }
}
