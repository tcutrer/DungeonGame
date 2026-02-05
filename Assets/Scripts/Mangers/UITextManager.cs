using UnityEngine;
using TMPro;

public class UITextManager : MonoBehaviour
{
    public static UITextManager Instance { get; private set; }
    public TextMeshProUGUI goldText;

    private void UpdateGoldText()
    {
        if (CurrencyManager.Instance != null && goldText != null)
        {
            goldText.text = "Gold: " + CurrencyManager.Instance.Gold;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGoldText();
    }
}
