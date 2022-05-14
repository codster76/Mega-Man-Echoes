using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColourPalette : MonoBehaviour
{
    // Used by animations to modify the colour palette, since you can't modify materials directly using animation.
    private SpriteRenderer spriterenderer;

    void Start()
    {
        spriterenderer = transform.GetComponent<SpriteRenderer>();
    }

    public void changePalette(int paletteNumber)
    {
        spriterenderer.material.SetFloat("_Offset",paletteNumber);
    }
}
