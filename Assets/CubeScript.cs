using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    [SerializeField] private Light cubeLight;
    private List<Color> colors;
    [SerializeField] private Material material;
    private int i;
    // Start is called before the first frame update
    void Start()
    {
        colors = new List<Color>();
        colors.Add(new Color(0.0f, 0.5f, 1.0f)); // Light Blue
        colors.Add(new Color(1.0f, 0.5f, 0.0f)); // Orange
        colors.Add(new Color(0.5f, 1.0f, 0.5f)); // Light Green
        colors.Add(new Color(1.0f, 0.0f, 1.0f));  // Magenta
        i = 0;
        material.color = colors[i];
    }

    // Update is called once per frame
    public void CubeClicked()
    {

        i = i % 4;
        cubeLight.color = colors[i];
        material.color = colors[i];
        material.SetColor("_EmissionColor", colors[i]);
        i++;
    }
}
