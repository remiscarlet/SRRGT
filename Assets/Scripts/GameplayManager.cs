using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// What's the purpose of this controller? Could I move everything elsewhere? Different name?
/// </summary>
public class GameplayManager : MonoBehaviour {
    private ChartReader chartReader;
    private AudioManager audioManager;

    private string chartPath = @"D:\Unity\SRRGT\Assets\chart1\chart1.txt"; // TODO: Obvi don't hardcode this

    public bool IsPaused { private set; get; }

    [CanBeNull] public Chart CurrChart { private set; get; }

    // Start is called before the first frame update
    void Start() {
        chartReader = new ChartReader();
        audioManager = ReferenceManager.AudioManager;

        InitializeChart(chartPath);
    }

    public void InitializeChart(string chartPath) {
        Chart chart = chartReader.InstantiateChart(chartPath);
        CurrChart = chart;

        ReferenceManager.BoardManager.InitializeNoteBoundaries(chart.NumKeys);
        audioManager.ToggleTrackMusic();
    }

    // Update is called once per frame
    void Update() {
        if (!IsPaused) {
            CurrChart.PlayChart();
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            TogglePauseState();
        }
    }

    public void TogglePauseState() {
        IsPaused = !IsPaused;

        ReferenceManager.AudioManager.ToggleTrackMusic();
        ReferenceManager.GameplayUIManager.TogglePauseScreen();
    }
}
