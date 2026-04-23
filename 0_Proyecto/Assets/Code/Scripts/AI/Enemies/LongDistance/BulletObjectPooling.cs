/* ================================================================
   ----------------------------------------------------------------
   Project   :   Iver Tales
   Publisher :   IguanaGo Studios
   Developer :   Lucas García Domínguez
   ----------------------------------------------------------------
   Copyright 2023-2024 IguanaGoStudios All rights reserved.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObjectPooling : MonoBehaviour
{
    private List<GameObject> poolBullets = new List<GameObject>();
    [SerializeField]
    private float amountPool = 60;
    [SerializeField]
    private GameObject bulletPrefab;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < amountPool; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);  //instantiates all the bullets
            bullet.transform.parent = this.transform;
            bullet.SetActive(false);
            poolBullets.Add(bullet);
        }
    }
    public GameObject GetPooledObject()
    {
        for(int i = 0;i < poolBullets.Count;i++)
        {
            if (!poolBullets[i].activeInHierarchy)  //activates and desactivates objects
            {
                return poolBullets[i];
            }
        }

        return null;
    }
}
