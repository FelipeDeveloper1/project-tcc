using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calcio : MonoBehaviour {

    // Movimentação
    [Header("Move")]
    public float                           moveSpeed;

    // Ataque
    [Header("Attack")]
    public float                           attackDistance; // Minimum distance for attack
    private bool                           attackMode;
    public float                           timer; // Timer for cooldown between attacks
    private float                          intTimer;
    private bool                           cooling; // Check if Enemy is cooling after attack

    // Área limite
    [Header("Limit")]
    public Transform                       leftLimit;
    public Transform                       rightLimit;

    // Reconhecimento do personagem
    [Header("Recognition")]
    [HideInInspector] public Transform     target;
    [HideInInspector] public bool          inRange; // Check if Player is in range
    public GameObject                      hotZone;
    public GameObject                      triggerArea;
    private float                          distance; // Store the distance b/w enemy and player

    // Componente
    private Animator anim;

    void Awake() {
        SelectTarget();
        intTimer = timer; // Store the inital value of timer
        anim = GetComponent<Animator>();
    }

    void Update() {
        if (!attackMode) {
            Move();
        }

        if (!InsideOfLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack")) {
            SelectTarget();
        }

        if (inRange) {
            EnemyLogic();
        }
    }

    void EnemyLogic() {
        distance = Vector2.Distance(transform.position, target.position);

        if (distance > attackDistance) {
            StopAttack();
        } else if (attackDistance >= distance && cooling == false) {
            Attack();
        }

        if (cooling) {
            Cooldown();
            anim.SetBool("Attack", false);
        }
    }

    void Move() {
        anim.SetBool("canWalk", true);

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack")) {
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void Attack() {
        timer = intTimer; //Reset Timer when Player enter Attack Range
        attackMode = true; //To check if Enemy can still attack or not

        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);
    }

    void Cooldown() {
        timer -= Time.deltaTime;

        if (timer <= 0 && cooling && attackMode) {
            cooling = false;
            timer = intTimer;
        }
    }

    void StopAttack() {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
    }

    public void TriggerCooling() {
        cooling = true;
    }

    private bool InsideOfLimits() {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }

    public void SelectTarget() {
        float distanceToLeft = Vector3.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector3.Distance(transform.position, rightLimit.position);

        if (distanceToLeft > distanceToRight) {
            target = leftLimit;
        } else {
            target = rightLimit;
        }

        //Ternary Operator
        //target = distanceToLeft > distanceToRight ? leftLimit : rightLimit;

        Flip();
    }

    public void Flip() {
        Vector3 rotation = transform.eulerAngles;
        if (transform.position.x > target.position.x) {
            rotation.y = 180;
        } else {
            rotation.y = 0;
        }

        //Ternary Operator
        //rotation.y = (currentTarget.position.x < transform.position.x) ? rotation.y = 180f : rotation.y = 0f;

        transform.eulerAngles = rotation;
    }
}
