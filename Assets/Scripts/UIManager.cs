using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    private GameObject loadingPanel;
    private Slider loadingBar;

    // Start is called before the first frame update
    void Start() {
        loadingPanel = transform.Find("ProgressBarPanel").gameObject;
        loadingBar = loadingPanel.transform.Find("ProgressBar").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
