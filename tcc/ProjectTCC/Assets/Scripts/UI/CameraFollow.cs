using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    private Transform       target;
    [SerializeField] float  positionY = 1f;
    [SerializeField] float  smoothSpeed = 1f;

    void Start() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate() {
        Vector3 startPosition = new Vector3(target.position.x, target.position.y + positionY, -1f);
        Vector3 smoothPosition = Vector3.Lerp(transform.position, startPosition, smoothSpeed);
        transform.position = smoothPosition;
    }
}
