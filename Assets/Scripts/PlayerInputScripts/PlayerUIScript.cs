using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerUIScript : MonoBehaviour
{
    private Game_Manger gameManger;
    private int currentPageIndex = 0; 
    private int itemsPerPage = 5;

    // 0 = Left Arrow, 1 = Right Arrow, 2-6 = Tile Buttons
    public Button[] spriteButtons = new Button[7];
    // These are the Image components of buttons 2 through 6
    public Image[] spriteButtonImages = new Image[5];
    public Sprite[] spriteImages = new Sprite[10];

    void Start()
    {
        gameManger = Game_Manger.instance;

        // Arrows (Indices 0 and 1)
        spriteButtons[0].onClick.AddListener(OnLeftArrowClicked);
        spriteButtons[1].onClick.AddListener(OnRightArrowClicked);

        // Tile Buttons (Indices 2 through 6)
        for (int i = 0; i < 5; i++)
        {
            int localIndex = i; // Critical: fix for lambda capture
            spriteButtons[i + 2].onClick.AddListener(() => OnSpriteButtonClicked(localIndex));
        }

        UpdatePageDisplay();
    }

    public void OnSpriteButtonClicked(int buttonSlotIndex)
    {
        // Calculate global sprite index based on current page
        int spriteIndex = (currentPageIndex * itemsPerPage) + buttonSlotIndex;
        
        if (gameManger != null && spriteIndex < spriteImages.Length)
        {
            gameManger.SelectSprite(spriteIndex);
        }
    }

    private void OnLeftArrowClicked()
    {
        // Loop back to page 1 if going left from page 0
        currentPageIndex = (currentPageIndex == 0) ? 1 : 0;
        UpdatePageDisplay();
    }

    private void OnRightArrowClicked()
    {
        // Loop back to page 0 if going right from page 1
        currentPageIndex = (currentPageIndex + 1) % 2; 
        UpdatePageDisplay();
    }

    private void UpdatePageDisplay()
    {
        int startIdx = currentPageIndex * itemsPerPage;

        for (int i = 0; i < itemsPerPage; i++)
        {
            int spriteIdx = startIdx + i;

            if (spriteIdx < spriteImages.Length)
            {
                spriteButtons[i + 2].gameObject.SetActive(true);
                spriteButtonImages[i].sprite = spriteImages[spriteIdx];
            }
            else
            {
                // Hide or disable buttons if you had fewer than 10 sprites
                spriteButtons[i + 2].gameObject.SetActive(false);
            }
        }
    }
}
