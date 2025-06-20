using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : MonoSingleton<OutlineManager>
{   
    [SerializeField] Material outlineMaterial;
    [SerializeField] Color color = Color.white;
    [Range(0, 16)] [SerializeField] int outlineSize = 1;    

    public void SetOutlineMaterial(SpriteRenderer spriteRenderer)
    {
        if(!outlineMaterial)
        {
            Debug.LogError("Outline Material is not set");
            return;
        }
        
        spriteRenderer.material = outlineMaterial;
    }

    public void SetOutline(SpriteRenderer spriteRenderer, bool isOutline)
    {
        UpdateOutline(spriteRenderer, isOutline);
    }

    void UpdateOutline(SpriteRenderer sprite, bool outline)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        {
            sprite.GetPropertyBlock(mpb);
            mpb.SetFloat("_Outline", outline ? 1f : 0);
            mpb.SetColor("_OutlineColor", color);
            mpb.SetFloat("_OutlineSize", outlineSize);
            sprite.SetPropertyBlock(mpb);
        }                
    }
}
