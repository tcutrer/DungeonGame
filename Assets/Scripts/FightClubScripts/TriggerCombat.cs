using UnityEngine;

public class TriggerCombat : MonoBehaviour
{
    void Start()
    {
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enemy entered range. Attack!");
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Enemy left range");
    }
}
