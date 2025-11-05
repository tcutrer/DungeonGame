using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Sprite
{
    public GameObject CreateSprite()
    {
        GameObject prefab = Resources.Load<GameObject>("Test_Sprite");
        if (prefab == null) {
            Debug.LogError("Test_Sprite prefab not found in Resources folder!");
            return null;
        }
        GameObject instance = GameObject.Instantiate(prefab);
        instance.transform.localScale = new Vector3(10f, 10f, 1f); // Adjust scale to match cell size
        instance.SetActive(true);
        return instance;
    }
}
