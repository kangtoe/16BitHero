using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : MonoSingleton<OutlineManager>
{   
    [SerializeField] Material outlineMaterial;
    [SerializeField] Color color = Color.white;
    [Range(0, 16)] [SerializeField] int outlineSize = 1;    

    public void SetOutlineMaterial(SpriteRenderer[] spriteRenderers)
    {
        if(!outlineMaterial)
        {
            Debug.LogError("Outline Material is not set");
            return;
        }

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material = outlineMaterial;
        }
    }

    public void SetOutline(SpriteRenderer[] spriteRenderers, bool isOutline)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            UpdateOutline(spriteRenderer, isOutline);
        }
    }

    void UpdateOutline(SpriteRenderer sprite, bool outline)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        {
            sprite.GetPropertyBlock(mpb);
            mpb.SetFloat("_Line", outline ? 1f : 0);
            mpb.SetColor("_LineColor", color);
            mpb.SetFloat("_LineSize", outlineSize);
            sprite.SetPropertyBlock(mpb);
        }                
    }
}
