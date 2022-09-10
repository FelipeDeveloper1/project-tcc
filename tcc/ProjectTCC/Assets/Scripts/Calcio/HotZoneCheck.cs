using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotZoneCheck : MonoBehaviour {

    private Calcio calcioParent;
    private bool inRange;
    private Animator anim;

    void Awake() {
        calcioParent = GetComponentInParent<Calcio>();
        anim = GetComponentInParent<Animator>();
    }

    void Update() {
        if (inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack")) {
            calcioParent.Flip();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            inRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            inRange = false;
            gameObject.SetActive(false);
            calcioParent.triggerArea.SetActive(true);
            calcioParent.inRange = false;
            calcioParent.SelectTarget();
        }
    }
}
