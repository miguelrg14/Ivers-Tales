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
    bool enableDeactivateGO=false;

    GameObject gameObjectToDeactivate;


    private void Awake()
    {
        shaderProperty = Shader.PropertyToID("_cutoff");
        _renderer = GetComponentInChildren<Renderer>();
        ps = GetComponentInChildren<ParticleSystem>();
        if (ps == null) return; // some enemies use the dissolve effect without particles
        ps.Stop();
        var main = ps.main;
        main.duration = spawnEffectTime;
    }
    public void StartDisableDeadEffect(GameObject go)
    {
        this.gameObject.GetComponent<Collider>().enabled = false;
        if (ps != null) ps.Play();
        enableEffect = true;
        gameObjectToDeactivate = go;

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
                enableDeactivateGO = true;
                DeactivateMesh();

            }


            // Apply the dissolve effect to every material of the renderer
            foreach (Material material in _renderer.materials)
            {
                material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer)));
            }

        }
        else if (enableDeactivateGO)
        {
            //DeactivateMesh();
            //material.SetFloat(shaderProperty,0);
            if (ps == null || !ps.IsAlive())
            {
                enableDeactivateGO = false;
                if (gameObjectToDeactivate != null)
                    gameObjectToDeactivate.SetActive(false);
            }
        }

    }
    void DeactivateMesh()
    {
        if (this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>())
            this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = (false);
        else
            this.gameObject.GetComponentInChildren<MeshRenderer>().enabled = (false);

    }
}
