using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterializeEffect : MonoBehaviour
{
    /// <summary>
    /// Materialize effect coroutine - used for the materialiuse special effect
    /// </summary>
    public IEnumerator MaterializeRoutine(Shader materializeShader, Color materializeColor, float materializeTime, SpriteRenderer[] spriteRendererArray, Material normalMaterial)
    {
        Debug.LogError($"MaterializeRoutine materializeShader: ");
        var materializeMaterial = new Material(materializeShader);

        materializeMaterial.SetColor("_EmissionColor", materializeColor);
        Debug.LogError("MaterializeRoutine");
        // Set materialize material in sprite renderers
        foreach (var spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = materializeMaterial;
            Debug.LogError($"MaterializeRoutine: {materializeMaterial}");
        }
        
        var dissolveAmount = 0f;
        
        // materialize enemy
        while (dissolveAmount < 1f)
        {
            dissolveAmount += Time.deltaTime / materializeTime;
            Debug.LogError($"dissolveAmount: {dissolveAmount}");
            materializeMaterial.SetFloat("_DissolveAmount", dissolveAmount);
        
            yield return null;
        
        }


        // Set standard material in sprite renderers
        foreach (var spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = normalMaterial;
            Debug.LogError($"spriteRenderer.material: {spriteRenderer.material}");
        }

        yield return spriteRendererArray;
    }
}
