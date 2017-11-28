using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Man Randomiser Settings")]
public class ManRandomiserSettings : ScriptableObject
{
    public List<Color> hair_colors = new List<Color>();
    public List<Color> skin_colors = new List<Color>();
    public List<Color> jacket_colors = new List<Color>();
    public List<Color> shirt_colors = new List<Color>();
    public List<Color> jeans_colors = new List<Color>();
}
