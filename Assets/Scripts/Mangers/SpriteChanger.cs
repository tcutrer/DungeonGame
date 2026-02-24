using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    [SerializeField] private Sprite[] availableSprites;  // Array to hold your sprites
    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;
    private List<int> costs = new List<int> {0, 5, 0, 0, 20, 25, 30, 35, 40, 45};

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

    public Sprite GetCurrentSprite()
    {
        return spriteRenderer.sprite;
    }

    public Sprite GetSpriteAtIndex(int index)
    {
        if (index < 0 || index >= availableSprites.Length)
        {
            Debug.LogWarning("Index out of bounds in GetSpriteAtIndex!");
            return null;
        }
        return availableSprites[index];
    }

    public void SetSpriteAtIndex(int index)
    {
        if (index < 0 || index >= availableSprites.Length)
        {
            Debug.LogWarning("Index out of bounds in SetSpriteAtIndex!");
            return;
        }
        currentSpriteIndex = index;
        spriteRenderer.sprite = availableSprites[index];
    }

    public void ResetSprite()
    {
        currentSpriteIndex = 0;
        spriteRenderer.sprite = availableSprites[currentSpriteIndex];
    }

    public SpriteRenderer getSpriteRenderer()
    {
        return spriteRenderer;
    }

    public int GetCost(int index)
    {
        return costs[index];
    }
}
