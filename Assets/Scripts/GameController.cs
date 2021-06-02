using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameController : MonoBehaviour {
    private ChartReader chartReader;
    private AudioManager audioManager;

    private string chartPath = @"D:\Unity\SRRGT\Assets\chart1\chart1.txt"; // TODO: Obvi don't hardcode this

    [CanBeNull] public Chart CurrChart { private set; get; }

    // Start is called before the first frame update
    void Start() {
        chartReader = new ChartReader();
        audioManager = ReferenceManager.AudioManager;
    }

    public void StartChart(string chartPath) {
        Chart chart = chartReader.InstantiateChart(chartPath);
        CurrChart = chart;

        ReferenceManager.BoardManager.InitializeNoteBoundaries(chart.NumKeys);

        audioManager.ToggleTrackMusic();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Return) && ! audioManager.IsPlayingTrack) {
            StartChart(chartPath);
        }

        if (audioManager.IsPlayingTrack) {
            CurrChart.PlayChart();
        }
    }
}
