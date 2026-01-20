using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerUIScript : MonoBehaviour
{
    private Game_Manger gameManger;
    private int currentPageIndex = 0; // Track which page of sprites we're viewing
    private int spritesPerPage = 5; // 5 sprite buttons + 2 arrows
    
    public Button[] spriteButtons = new Button[7];
    public Image[] spriteButtonImages = new Image[5];  // Only the 5 middle buttons
    public Sprite[] spriteImages = new Sprite[16];
    
    // List of all available sprites (costume indices)
    private List<int> allSprites = new List<int> {5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20};

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManger = Game_Manger.instance;
        
        // Setup button listeners
        for (int i = 0; i < 5; i++)
        {
            int index = i; // Local copy for closure
            spriteButtons[i].onClick.AddListener(() => OnSpriteButtonClicked(index));
        }
        
        // Left arrow (button 5)
        spriteButtons[5].onClick.AddListener(OnLeftArrowClicked);
        
        // Right arrow (button 6)
        spriteButtons[6].onClick.AddListener(OnRightArrowClicked);
        
        UpdatePageDisplay();
    }

    public void OnSpriteButtonClicked(int buttonIndex)
    {
        // Calculate which sprite from the full list this button represents
        int spriteIndex = (currentPageIndex * spritesPerPage) + buttonIndex;
        
        if (spriteIndex < allSprites.Count)
        {
            int selectedSprite = allSprites[spriteIndex];
            if (gameManger != null)
            {
                gameManger.SelectSprite(selectedSprite);
            }
        }
    }
    
    private void OnLeftArrowClicked()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePageDisplay();
        }
    }
    
    private void OnRightArrowClicked()
    {
        // Calculate how many pages exist
        int totalPages = (allSprites.Count + spritesPerPage - 1) / spritesPerPage;
        
        if (currentPageIndex < totalPages - 1)
        {
            currentPageIndex++;
            UpdatePageDisplay();
        }
    }
    
    private void UpdatePageDisplay()
    {
        // Update the 5 sprite buttons with current page sprites
        for (int i = 0; i < 5; i++)
        {
            int spriteIndex = (currentPageIndex * spritesPerPage) + i;
            
            if (spriteIndex < allSprites.Count)
            {
                // Button should be active and show sprite
                spriteButtons[i].interactable = true;
                // Display the image for this sprite
                if (spriteImages[spriteIndex] != null)
                {
                    spriteButtonImages[i].sprite = spriteImages[spriteIndex];
                }
            }
            else
            {
                // Disable button if no sprite for this slot
                spriteButtons[i].interactable = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
