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
        UpdateGoldText();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGoldText();
    }
}
