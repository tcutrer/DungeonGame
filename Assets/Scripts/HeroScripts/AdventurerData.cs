using UnityEngine;

[CreateAssetMenu(fileName = "AdventurerData", menuName = "Scriptable Objects/AdventurerData")]
public class AdventurerData : ScriptableObject
{
    public int health;
    public float speed;
    public int attackPower;
    public string adventurerName;
}
