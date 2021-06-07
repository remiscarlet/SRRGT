using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatLineController : Chart.EventController {
    // Update is called once per frame
    void Update() {
        base.Update();
        if (IsOutOfBounds()) {
            Destroy(gameObject);
        }
    }
}
