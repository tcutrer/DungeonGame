using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    [SerializeField] private Sprite[] availableSprites;  // Array to hold your sprites
    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = availableSprites[currentSpriteIndex];
    }

    public void ChangeSprite(int index = -1)
    {

        if (availableSprites == null || availableSprites.Length == 0)
        {
            Debug.LogWarning("No sprites assigned to SpriteChanger!");
            return;
        }

        if (index == -1)
        {
            currentSpriteIndex = (currentSpriteIndex + 1) % availableSprites.Length;
            spriteRenderer.sprite = availableSprites[currentSpriteIndex];
            return;
        }
        currentSpriteIndex = index;
        spriteRenderer.sprite = availableSprites[index];
    }
    
    public int GetCurrentSpriteIndex()
    {
        return currentSpriteIndex;
    }

    public int GetTotalSprites()
    {
        return availableSprites.Length;
    }
}
