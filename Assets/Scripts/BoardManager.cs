using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    private Transform environmentHierarchyTransform;
    private GameObject noteBoundaryPrefab;
    private Quaternion noteBoundaryRot;

    private List<GameObject> boundaries;

    private double boardWidth = 10.0; // TODO: Tie this to config/class property of something else.

    // Start is called before the first frame update
    void Start() {
        environmentHierarchyTransform = ReferenceManager.EnvironmentHierarchyTransform;
        noteBoundaryPrefab = ReferenceManager.Prefabs.NoteBoundaryLine;
        noteBoundaryRot = Quaternion.Euler(noteBoundaryPrefab.transform.forward);

        boundaries = new List<GameObject>();
    }

    public void InitializeNoteBoundaries(int numKeys) {
        float rowWidth = (float) boardWidth / numKeys;
        Vector3 position;
        for (int idx = 0; idx < numKeys; idx++) {
            position = new Vector3(idx * rowWidth, 0.0f, 0.0f);
            GameObject boundary = Instantiate(noteBoundaryPrefab, position, noteBoundaryRot, environmentHierarchyTransform);

            boundaries.Add(boundary);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
