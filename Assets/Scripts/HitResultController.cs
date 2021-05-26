using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitResultController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    private int textFloatUpSpeed = 6;
    // Update is called once per frame
    void Update()
    {
        // TODO: Rigidbody/force instead of kinematics?
        transform.Translate(Vector3.up * textFloatUpSpeed * Time.deltaTime);
    }
}
