using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
    public int Gold { get; private set; }

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Gold = 50;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddGold(int amount)
    {
        Debug.Log("Added " + amount + " gold.");
        Gold += amount;
    }
}
