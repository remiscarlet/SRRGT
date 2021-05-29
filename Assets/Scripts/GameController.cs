using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameController : MonoBehaviour {
    private ChartReader chartReader;
    // Start is called before the first frame update
    void Start() {
        chartReader = new ChartReader();

        chartReader.ReadAndParseChart(@"D:\Unity\SRRGT\Assets\chart1\chart1.txt");
    }

    // Update is called once per frame
    void Update() { }
}
