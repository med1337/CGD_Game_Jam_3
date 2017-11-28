using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManRandomiser : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] ManRandomiserSettings settings;

    [Header("Reference")]
    [SerializeField] MeshRenderer man_mesh;

    [Header("Debug")]
    [SerializeField] bool re_roll;

    private enum MatIndex
    {
        SKIN,
        SHIRT,
        JEANS,
        HAIR,
        JACKET
    }


    void Start()
    {
        RandomiseAll();
    }


    void Update()
    {
        if (re_roll)
        {
            re_roll = false;
            RandomiseAll();
        }
    }


    void RandomiseAll()
    {
        if (settings == null)
        {
            Debug.LogWarning("RandomiseAll failed: No settings in ManRandomiser");
            return;
        }

        Randomise(MatIndex.HAIR, settings.hair_colors);
        Randomise(MatIndex.SKIN, settings.skin_colors);
        Randomise(MatIndex.JACKET, settings.jacket_colors);
        Randomise(MatIndex.SHIRT, settings.shirt_colors);
        Randomise(MatIndex.JEANS, settings.jeans_colors);
    }


    void Randomise(MatIndex _index, List<Color> _colors)
    {
        if (_colors.Count == 0)
            return;

        Material mat = GetMaterialInstance(_index);
        Color color = GetRandomColor(_colors);

        mat.color = color;
    }


    Material GetMaterialInstance(MatIndex _index)
    {
        return man_mesh.materials[(int)_index];
    }


    Color GetRandomColor(List<Color> _colors)
    {
        Color color = new Color();

        color = _colors[Random.Range(0, _colors.Count)];

        return color;
    }


    void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }

}
