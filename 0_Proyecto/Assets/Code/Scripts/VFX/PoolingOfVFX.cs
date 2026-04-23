using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingOfVFX : MonoBehaviour
{
    private List<ParticleSystem> poolVFX = new List<ParticleSystem>();
    [SerializeField]
    private float amountPool = 5;
    [SerializeField]
    private ParticleSystem psPrefab;
    void Start()
    {
        for (int i = 0; i < amountPool; i++)
        {
            ParticleSystem vfx = Instantiate(psPrefab);  //instantiates all the bullets
            vfx.gameObject.transform.parent = this.transform;
            vfx.gameObject.SetActive(false);
            poolVFX.Add(vfx);
        }
    }
    public ParticleSystem GetPooledObject()
    {
        for (int i = 0; i < poolVFX.Count; i++)
        {
            if (!poolVFX[i].gameObject.activeInHierarchy)  //activates and desactivates objects
            {
                return poolVFX[i];
            }
        }

        return null;
    }
    public void EnableParticleSystem(Vector3 enemyPosition,Quaternion enemyRotation)
    {
        ParticleSystem particleSystem =GetPooledObject();
        if (particleSystem!=null)
        {
            particleSystem.gameObject.transform.position = enemyPosition;
            particleSystem.gameObject.transform.rotation = enemyRotation;
            particleSystem.gameObject.SetActive(true);
            particleSystem.Play();
        }

    }

}
