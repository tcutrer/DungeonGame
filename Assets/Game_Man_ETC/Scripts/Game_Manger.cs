using UnityEngine;

public class Game_Manger : MonoBehaviour
{
    // Private Variables
    private bool Is_play = false;
    private float Cycle_time = 0f;
    private int Unit_nums = 0;
    private int Phase = 0;

    // Getters
    public bool Get_Is_play()
    {
        return Is_play;
    }
    public float Get_Cycle_time()
    {
        return Cycle_time;
    }
    public int Get_Unit_nums()
    {
        return Unit_nums;
    }
    public int Get_Phase()
    {
        return Phase;
    }

    // Setters
    public void Set_Is_play(bool value)
    {
        Is_play = value;
    }
    public void Set_Cycle_time(float value)
    {
        Cycle_time = value;
    }
    public void Set_Unit_nums(int value)
    {
        Unit_nums = value;
    }
    public void Set_Phase(int value)
    {
        Phase = value;
    }

    // Other Methods
    

    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
