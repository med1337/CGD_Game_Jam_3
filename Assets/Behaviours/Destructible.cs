using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] List<MeshRenderer> meshes = new List<MeshRenderer>();
    
    List<Material> original_materials = new List<Material>();
    List<Color> original_colors = new List<Color>();


    public void DamageFlash()
    {
        ResetMaterials();
        StopAllCoroutines();

        StartCoroutine(FlashRoutine());
    }


    IEnumerator FlashRoutine()
    {
        foreach (var mesh in meshes)
        {
            mesh.material = null;
            mesh.material.color = Color.white;
        }

        yield return new WaitForSeconds(0.05f);

        ResetMaterials();
    }


    void Start()
    {
        foreach (var mesh in meshes)
        {
            original_materials.Add(mesh.material);
            original_colors.Add(mesh.material.color);
        }
    }


    void ResetMaterials()
    {
        int i = 0;
        foreach (var mesh in meshes)
        {
            mesh.material = original_materials[i];
            mesh.material.color = original_colors[i];

            ++i;
        }
    }

}
