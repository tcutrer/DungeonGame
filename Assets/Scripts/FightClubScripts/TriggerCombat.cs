using UnityEngine;

public class TriggerCombat : MonoBehaviour
{
    private Adventurer adventurer;
    private Creature creature;
    void Start()
    {
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
       //Creature attacks first
        creature = GetComponent<Creature>();
        int hitpointsC = creature.Attack(1);
        collision.gameObject.SendMessage("TakeDamage", hitpointsC);

        adventurer = collision.gameObject.GetComponent<Adventurer>();
        int hitpointsA = adventurer.Attack(1);
        creature.TakeDamage(hitpointsA);

       //Adventurer attcks second
       
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Enemy left range");
    }
}
