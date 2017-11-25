using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlasher : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] Color flash_color = Color.white;
    [SerializeField] float flash_duration = 0.05f;

    [Header("References")]
    [SerializeField] List<MeshRenderer> renderers = new List<MeshRenderer>();
    
    private List<MatPack> originals = new List<MatPack>();
    private Texture default_texture;

    [System.Serializable]
    private class MatPack
    {
        public List<Texture> original_textures;
        public List<Color> original_colors;

        public MatPack()
        {
            original_textures = new List<Texture>();
            original_colors = new List<Color>();
        }
    }


    public void DamageFlash()
    {
        ResetMaterials();
        StopAllCoroutines();

        StartCoroutine(FlashRoutine());
    }


    IEnumerator FlashRoutine()
    {
        for (int i = 0; i < renderers.Count; ++i)
        {
            var r = renderers[i];

            for (int j = 0; j < r.materials.Length; ++j)
            {
                r.materials[j].SetTexture("_MainTex", default_texture);
                r.materials[j].color = flash_color;
            }
        }

        yield return new WaitForSeconds(flash_duration);

        ResetMaterials();
    }


    void Start()
    {
        default_texture = GameManager.default_texture;

        for (int i = 0; i < renderers.Count; ++i)
        {
            var r = renderers[i];

            MatPack pack = new MatPack();
            
            for (int j = 0; j < r.materials.Length; ++j)
            {
                var mat = r.materials[j];
                pack.original_textures.Add(mat.mainTexture);
                pack.original_colors.Add(mat.color);
            }

            originals.Add(pack);
        }
    }


    void ResetMaterials()
    {
        for (int i = 0; i < renderers.Count; ++i)
        {
            var r = renderers[i];
            var mat_pack = originals[i];

            for (int j = 0; j < r.materials.Length; ++j)
            {
                r.materials[j].SetTexture("_MainTex", mat_pack.original_textures[j]);
                r.materials[j].color = mat_pack.original_colors[j];
            }
        }
    }


    void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }

}
