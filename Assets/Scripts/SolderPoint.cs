using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SolderPoint : MonoBehaviour
{
    public float temperature = 0f;
    public bool isBurned = false;
    Material material;
    Color originalColor;
    public XRBaseController rController;
    public XRBaseController lController;
    // Start is called before the first frame update
    void Start()
    {
        material = transform.GetComponent<Renderer>().material;
        originalColor = material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if(temperature < 1f && temperature > 0f && !isBurned){
            temperature -= 0.001f;
        }
        material.color = Color.Lerp(originalColor, new Color(originalColor.r, originalColor.g, originalColor.b, 1), temperature);
        if(temperature > 1f){
            material.color = new Color(0, originalColor.g, 0, 1f);
        }
    }
}
