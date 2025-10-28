using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour {

    private Grid grid;

    // Start is called before the first frame update
    private void Start() {
        grid = new Grid(4, 2, 10f, new Vector3(20, 0));
    }

    // Update is called once per frame
    private void Update() {
    }
}
