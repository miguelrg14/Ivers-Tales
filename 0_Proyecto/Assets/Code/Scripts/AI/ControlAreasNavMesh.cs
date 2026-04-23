using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class ControlAreasNavMesh : MonoBehaviour
{
//    public NavMeshSurface surface;
//    public float radioDeteccion = 10f;
//    public LayerMask enemigoTipoA; // Capa de enemigos del tipo A
//    public LayerMask enemigoTipoB; // Capa de enemigos del tipo B

//    void Start()
//    {
//        surface.BuildNavMesh(); // Construir el NavMesh al inicio
//    }

//    void Update()
//    {
//        Collider[] colliders = Physics.OverlapSphere(transform.position, radioDeteccion);

//        foreach (Collider collider in colliders)
//        {
//            if (collider.CompareTag("Player"))
//            {
//                ModifyNavMeshAreas(enemigoTipoA, NavMeshAreaModifier.VolumeExclude);
//                ModifyNavMeshAreas(enemigoTipoB, NavMeshAreaModifier.VolumeInclude);
//                return;
//            }
//        }

//        // Si el jugador no está cerca, se restauran las áreas originales del NavMesh
//        ModifyNavMeshAreas(enemigoTipoA, NavMeshAreaModifier.VolumeInclude);
//        ModifyNavMeshAreas(enemigoTipoB, NavMeshAreaModifier.VolumeExclude);
//        //surface.BuildNavMesh(); // Reconstruir el NavMesh después de modificar las áreas

//    }

//    void ModifyNavMeshAreas(LayerMask enemigoLayer, NavMeshAreaModifier modifier)
//    {
//        Collider[] colliders = Physics.OverlapSphere(transform.position, radioDeteccion, enemigoLayer);

//        foreach (Collider collider in colliders)
//        {
//            NavMeshModifierVolume navModifier = collider.GetComponent<NavMeshModifierVolume>();

//            if (navModifier != null)
//            {
//                navModifier.area = (int)modifier;
//                surface.BuildNavMesh(); // Reconstruir el NavMesh después de modificar las áreas
//            }
//        }
//    }
}
