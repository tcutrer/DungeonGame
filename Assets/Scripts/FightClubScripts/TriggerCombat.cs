using UnityEngine;
using System;
using System.Collections;

public class TriggerCombat : MonoBehaviour
{
    private Adventurer adventurer;
    private Creature creature;
    private bool isInside = false;
    private Coroutine fightCoroutine;

    void Start()
    {
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        isInside = true;
        fightCoroutine = StartCoroutine(Fight(collision));
       
    }

    IEnumerator Fight(Collider2D collision)
    {

        creature = GetComponent<Creature>();
        int hitpointsC = creature.Attack(1);
        collision.gameObject.SendMessage("TakeDamage", hitpointsC);

        adventurer = collision.gameObject.GetComponent<Adventurer>();
        int hitpointsA = adventurer.Attack(1);
        creature.TakeDamage(hitpointsA);
        yield return new WaitForSeconds(1);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        isInside = false;
        Debug.Log("Enemy left range");
        if (fightCoroutine != null) {
            StopCoroutine(fightCoroutine);
        }
    }
}
