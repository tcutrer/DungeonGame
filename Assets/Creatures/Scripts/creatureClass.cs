using UnityEngine;

public class creatureClass : MonoBehaviour
{
    public string creatureName {get; private set;}
    public int ID {get; private set;}
    public int conditioin {get; set;}
    public float health {get; set;}
    public float healthMax {get; private set;}
    public int level {get; set;}
    public float attackPower {get; private set;}
    public int movementSpeed {get; private set;}
    public vector2 heldItems {get; set;}
    public int inventorySize {get; private set;}
    public vector2 position {get; set;}

    public Creature(string nme = "Bob", int id = 1, int condish = 0, float hlth = 10.0,
    float mxhlth = 10.0, int lv = 1, float atk = 1, int move = 5, array held = [], int invsz = 0,
    vector2 pos = [0,0])
    {
        creatureName = nme;
        ID = id;
        condition = condish;
        health = hlth;
        healthMax = mxhlth;
        level = lv;
        attackPower = atk;
        movementSpeed = move;
        heldItems = held;
        inventorySize = invsz;
        position = pos;
    }

    public void takeDamage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(creatureName + "has died");
        Destroy(gameObject);
    }

}
