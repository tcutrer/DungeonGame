using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
    public int Gold { get; private set; }
    [SerializeField]
    private readonly int startingGold = 100;

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
        Gold = startingGold;
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

    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            Debug.Log("Spent " + amount + " gold.");
            return true;
        }
        else
        {
            Debug.Log("Not enough gold to spend " + amount + ".");
            return false;
        }
    }

    bool HasEnoughGold(int amount)
    {
        return Gold >= amount;
    }
}
