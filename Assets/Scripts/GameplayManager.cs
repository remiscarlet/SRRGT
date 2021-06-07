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
    protected Chart.Reader chartReader;
    protected AudioManager audioManager;

    protected string chartPath = @"D:\Unity\SRRGT\Assets\chart1\chart1.txt"; // TODO: Obvi don't hardcode this

    public bool IsPaused { protected set; get; }

    [CanBeNull] public Chart.Chart CurrChart { protected set; get; }

    // Start is called before the first frame update
    protected void Start() {
        Initialize();
    }

    protected void Initialize(bool autoStartTrack = true) {
        chartReader = new Chart.Reader();
        audioManager = ReferenceManager.AudioManager;
        PrefabManager prefabs = ReferenceManager.Prefabs;
        Debug.Log($">>> prefabs: {prefabs} - {prefabs.NoteBoundaryLine}");

        InitializeChart(chartPath, autoStartTrack);
    }

    public void InitializeChart(string chartPath, bool autoStartTrack) {
        Chart.Chart chart = chartReader.InstantiateChart(chartPath);
        CurrChart = chart;

        ReferenceManager.BoardManager.InitializeNoteBoundaries(chart.NumKeys);
        if (autoStartTrack) {
            audioManager.ToggleTrackMusic();
        }
    }

    // Update is called once per frame
    protected void Update() {
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
