using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAreaCheck : MonoBehaviour {
    
    private Calcio  calcioParent;

    void Awake() {
        calcioParent = GetComponentInParent<Calcio>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            gameObject.SetActive(false);
            calcioParent.target = other.transform;
            calcioParent.inRange = true;
            calcioParent.hotZone.SetActive(true);
        }
    }
}
