using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarShot : MonoBehaviour
{
    public Rigidbody projectilePrefab; // Asigna el prefab del proyectil en el Inspector
    public Transform firePoint; // Punto desde donde se dispara el mortero
    public Transform playerPosition { private get; set; } // Punto desde donde se dispara el mortero

    public float launchForce = 20f; // Fuerza inicial de lanzamiento
    public GameObject actualGOBullet { private get; set; }

    public void Launch(Vector3 offset)
    {
        Vector3 launchPoint = firePoint.position;
        Vector3 targetPoint = offset;
        targetPoint.y = 1f;
        Vector2 dir;
        dir.x = targetPoint.x - launchPoint.x;
        dir.y = targetPoint.z - launchPoint.z;
        float x = dir.magnitude;
        float y = -1;
        dir /= x;
        float g = 9.81f;
        float s = 12f;
        float s2 = s * s;

        float r = s2 * s2 - g * (g * x * x + 2f * y * s2);
        float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
        float cosTheta = Mathf.Cos(Mathf.Atan(tanTheta));
        float sinTheta = cosTheta * tanTheta;
        Vector3 prev = launchPoint, next;
        for (int i = 1; i <= 30; i++)
        {
            float t = i / 10f;
            float dx = s * cosTheta * t;
            float dy = s * sinTheta * t - 0.5f * g * t * t;
            next = launchPoint + new Vector3(dir.x * dx, dy, dir.y * dx);
            Debug.DrawLine(prev, next, Color.blue, 1f);
            prev = next;
        }
        Debug.DrawLine(
        new Vector3(launchPoint.x, 1, launchPoint.z),
        new Vector3(
        launchPoint.x + dir.x * x, 1, launchPoint.z + dir.y * x), Color.white, 1f);


        Debug.DrawLine(launchPoint, targetPoint, Color.red, 1f);
        if (actualGOBullet != null)
        {
            actualGOBullet.SetActive(true);
            //actualGOBullet.transform.position = launchPoint;
            //actualGOBullet.transform.rotation = transform.rotation;
            Debug.Log(playerPosition.localPosition.y);
            if (playerPosition.parent)
                actualGOBullet.GetComponent<BulletLongDistanceEnemy>().SetStartValues(launchPoint, targetPoint,
                new Vector3(s * cosTheta * dir.x, s * sinTheta, s * cosTheta * dir.y), playerPosition.position.y);
            else
                actualGOBullet.GetComponent<BulletLongDistanceEnemy>().SetStartValues(launchPoint, targetPoint,
                new Vector3(s * cosTheta * dir.x, s * sinTheta, s * cosTheta * dir.y), playerPosition.localPosition.y);

        }

    }
}
