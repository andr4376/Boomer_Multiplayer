using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessingScript : MonoBehaviour
{

    public Material pixelationMaterial;

    private const string resolutionKW = "_Resolution";
    private float x, y;
    [Range(0, 1)]
    public float resolutionPercentage;


    private void OnEnable()
    {
        if (pixelationMaterial == null)
        {
            pixelationMaterial = new Material(Shader.Find("PostProcessing/PixelationShader"));

        }

        x = Screen.width * resolutionPercentage;
        y = Screen.height * resolutionPercentage;

        pixelationMaterial.SetVector(resolutionKW, new Vector2(
           x,
           y
           ));
    }

#if UNITY_EDITOR
    private void FixedUpdate()
    {
        x = Screen.width * resolutionPercentage;
        y = Screen.height * resolutionPercentage;

        pixelationMaterial.SetVector(resolutionKW, new Vector2(
           x,
           y
           ));
    }
#endif

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (pixelationMaterial == null)
        {
            return;
        }

        Graphics.Blit(source, destination, pixelationMaterial);

    }
}
