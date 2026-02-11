using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class UITextManager : MonoBehaviour
{
    public static UITextManager Instance { get; private set; }
    public static bool isRoomMenuOpen = false;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI roomPurchaseText;
    public GameObject roomPurchasePanel;
    private Vector3 roomPurchasePosition;

    private void UpdateGoldText()
    {
        if (CurrencyManager.Instance != null && goldText != null)
        {
            goldText.text = "Gold: " + CurrencyManager.Instance.Gold;
        }
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (goldText == null)
        {
            Debug.LogError("goldText TextMeshProUGUI is not assigned in UITextManager!");
        }
        if (roomPurchaseText == null)
        {
            Debug.LogError("roomPurchaseText TextMeshProUGUI is not assigned in UITextManager!");
        }
        roomPurchasePanel.SetActive(false);
        UpdateGoldText();
    }

    public void ShowRoomPurchaseText(int roomCost, Vector3 position)
    {
        if (roomPurchaseText != null)
        {
            roomPurchaseText.text = "Do you want to buy this room for:" + System.Environment.NewLine + System.Environment.NewLine + roomCost + System.Environment.NewLine + System.Environment.NewLine + "?";
            roomPurchasePosition = position;
            roomPurchasePanel.SetActive(true);
            isRoomMenuOpen = true;
        }
    }

    public void DenyRoomPurchaseText()
    {
        roomPurchasePanel.SetActive(false);
        isRoomMenuOpen = false;
    }

    public void ConfirmRoomPurchaseText()
    {
        if (CurrencyManager.Instance != null)
        {
            int roomCost = 50; // This should be dynamically set based on the room being purchased
            if (CurrencyManager.Instance.SpendGold(roomCost))
            {
                // Logic to unlock the room goes here
                Debug.Log("Room purchased successfully!");
                Game_Manger.instance.unlockRoom(roomPurchasePosition); // Assuming you have an UnlockRoom method in your Game_Manger
            }
            else
            {
                Debug.Log("Not enough gold to purchase the room.");
            }
        }
        roomPurchasePanel.SetActive(false);
        isRoomMenuOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGoldText();
    }
}
