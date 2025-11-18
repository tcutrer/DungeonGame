using UnityEngine;

public class creatureClass : MonoBehaviour
{
    [SerializeField]
    private string name;
    public string name
    {
        get{return name;}
        set{name = name;}
    }
    private int id;
    private string condition;
    private float health;
    private float healthMax;
    private int level;
    private float attack;
    private int movement;
    private array heldItems;
    private int inventorySize;
}
