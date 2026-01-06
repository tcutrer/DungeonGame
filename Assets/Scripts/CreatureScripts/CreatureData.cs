using UnityEngine;

[CreateAssetMenu(fileName = "CreatureData", menuName = "Scriptable Objects/CreatureData")]
public class CreatureData : ScriptableObject
{
    public int health;
    public int speed;
    public int attackPower;
}