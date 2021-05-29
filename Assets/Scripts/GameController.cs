using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameController : MonoBehaviour {
    private ChartReader chartReader;

    public Chart currChart;
    // Start is called before the first frame update
    void Start() {
        chartReader = new ChartReader();

        Chart chart = chartReader.InstantiateChart(@"D:\Unity\SRRGT\Assets\chart1\chart1.txt");
        currChart = chart;
    }

    // Update is called once per frame
    void Update() {
        if (ReferenceManager.AudioManager.IsPlayingTrack) {
            currChart.PlayChart();
        }
    }
}
