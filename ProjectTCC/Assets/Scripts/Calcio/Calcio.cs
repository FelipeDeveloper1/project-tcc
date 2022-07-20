using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calcio : MonoBehaviour {

    // Movimentação
    [Header("Move")]
    public float                           moveSpeed;
    private bool                           blockMove;

    // Ataque
    [Header("Attack")]
    public float                           attackDistance; // Minimum distance for attack
    private bool                           attackMode;
    public float                           timer; // Timer for cooldown between attacks
    private float                          intTimer;
    private bool                           cooling; // Check if Enemy is cooling after attack

    // Vida do inimigo
    [Header("Health")]
    [SerializeField] int          damagePlayer;
    public int                    maxHealth = 100;
	public int                    currentHealth;
    private bool                  colliding;

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
        
        anim = GetComponent<Animator>();

        SelectTarget();

        //Armazenar o valor inicial do tempo
        intTimer = timer; 

        // Define a vida máxima
        currentHealth = maxHealth;

        blockMove = false;
    }

    void Update() {

        // Detecta se está colidindo
        colliding = false; 

        // Chama a movimentação do inimigo
        if (!attackMode) {
            Move();
        }

        if (!InsideOfLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack")) {
            SelectTarget();
        }

        if (inRange) {
            EnemyLogic();
        }

        // Animação de morte do inimigo
        if (currentHealth <= 0 && !blockMove) {
            anim.SetTrigger("Dead");
            blockMove = true;
            moveSpeed = 0;
            transform.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            transform.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }

    // Lógica de ataque do inimigo
    void EnemyLogic() {
        if (!blockMove) {
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
    }

    // Movimentação do inimigo
    void Move() {
        if (!blockMove) {
            anim.SetBool("canWalk", true);

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack")) {
                Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
        }
    }

    // Ataque do inimigo
    void Attack() {
        // Redefine o tempo quando o jogador entrar no alcance de ataque
        timer = intTimer; 
        attackMode = true; 

        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);
    }

    // Tempo de espera do inimigo
    void Cooldown() {
        timer -= Time.deltaTime;

        if (timer <= 0 && cooling && attackMode) {
            cooling = false;
            timer = intTimer;
        }
    }

    // Interrompe o ataque
    void StopAttack() {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
    }

    public void TriggerCooling() {
        cooling = true;
    }

    // Mantem o inimigo dentro dos limites
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

    // Inverte a posição do inimigo 
    public void Flip() {
        if (!blockMove) {
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

    // Dano do inimigo
    void OnTriggerEnter2D(Collider2D other) {
        if(colliding)
            return;
        colliding = true;

        if (other.gameObject.tag == "AttackPlayer" && currentHealth > 0) {
            currentHealth -= damagePlayer;
            if (currentHealth > 0)
            anim.SetTrigger("Hit");
        }
    }
}
