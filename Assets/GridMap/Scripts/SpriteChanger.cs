using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    [SerializeField] private Sprite[] availableSprites;  // Array to hold your sprites
    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeSprite()
    {
        if (availableSprites == null || availableSprites.Length == 0) 
        {
            Debug.LogWarning("No sprites assigned to SpriteChanger!");
            return;
        }

        currentSpriteIndex = (currentSpriteIndex + 1) % availableSprites.Length;
        spriteRenderer.sprite = availableSprites[currentSpriteIndex];
    }
}
