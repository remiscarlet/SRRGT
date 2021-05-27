using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitResultController : MonoBehaviour
{
    private int maxY = 20;
    private int textFloatUpSpeed = 6;
    // Update is called once per frame
    void Update()
    {
        // TODO: Rigidbody/force instead of kinematics?
        transform.Translate(Vector3.up * textFloatUpSpeed * Time.deltaTime);

        if (transform.position.y > maxY) {
            Destroy(gameObject);
        }
    }
}
