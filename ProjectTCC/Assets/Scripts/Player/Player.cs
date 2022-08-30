using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // Movimentação
    [Header("Move")]
    [SerializeField] float        moveSpeed = 4.0f;
    public static bool            blockInput; 
    private float                 inputX;              
    private int                   facingDirection;
    private float                 slowspeed = 1.0f;

    // Pulo
    [Header("Jump")]
    [SerializeField] float        jumpForce = 7.5f;
    [SerializeField] bool         jumping;
    private bool                  doubleJump;

    // Ataque
    [Header("Atack")]
    [SerializeField] bool         attacking;
    [SerializeField] float        timeAttack = 0.4f;
    private int                   currentAttack = 0;
    private float                 timeSinceAttack = 0.0f;

    // Bloqueo
    [Header("Block")]
    [SerializeField] bool         blocking;

    // Dash
    [Header("Dash")]
    public float                dashVel;
    public float                dashTime;
    private Vector2             dashDir;
    private bool                isDashing;
    private bool                canDash;
    public Ghost                ghost;

    // Paraquedas
    [Header("Parachute")]
    public Parachute              parachute;

    // Reconhecer o chão
    [Header("Ground")]
    [SerializeField] bool         grounded;
    [SerializeField] LayerMask    groundLayer;
    public Transform              groundPosition;
    public float                  sizeRadius;

    // Vida do personagem
    [Header("Health")]
    [SerializeField] int          damageEnemy;
    public int                    maxHealth = 100;
	public int                    currentHealth;
	public HealthBar              healthBar;
    private bool                  colliding;

    // Componentes 
    private Rigidbody2D           rb;
    private Animator              animator;
    private TrailRenderer         tr;

    void Start() {

        // Referencia os componentes
        rb =        GetComponent<Rigidbody2D>();
        animator =  GetComponent<Animator>();
        tr =        GetComponent<TrailRenderer>();

        // Define a vida máxima
        currentHealth = maxHealth;
		healthBar.SetMaxHealth(maxHealth);
        
        blockInput = false;
    }

    void Update() {
        Dash();
        // Detecta se está colidindo
        colliding = false; 

        // Reconhece o chão
        grounded = Physics2D.OverlapCircle(groundPosition.position, sizeRadius, groundLayer);
        
        // Input de movimentação do personagem 
        if (!blockInput) 
            inputX = Input.GetAxis("Horizontal");

        // input do pulo do personagem 
        if (Input.GetButtonDown("Jump") && !blockInput) {
            if (grounded) {
                jumping = true;
                doubleJump = true;
            } else if (!grounded && doubleJump) {
                jumping = true;
                doubleJump = false;
            } 
        }

        // Input do ataque do personagem 
        if (Input.GetButtonDown("Fire1") && timeSinceAttack > timeAttack && !blockInput) {
            attacking = true;
            currentAttack++;

            // Loop de volta para um após o terceiro ataque
            if (currentAttack > 3)
                currentAttack = 1;

            // Redefine o combo de ataque se o tempo desde o último ataque for muito grande
            if (timeSinceAttack > 1.0f)
                currentAttack = 1;

            // Chama uma das três animações de ataque "Attacking1", "Attacking2", "Attacking3"
            animator.SetTrigger("Attacking" + currentAttack);

            // Redefine o temporizador
            timeSinceAttack = 0.0f;
        } 

        // Input de bloqueo
        if (Input.GetButtonDown("Fire2") && grounded && !attacking && !blockInput) {
            blocking = true;
            animator.SetTrigger("Blocking");
            animator.SetBool("IdleBlocking", true);
        } else if (Input.GetButtonUp("Fire2")) {
            blocking = false;
            animator.SetBool("IdleBlocking", false);
        }

        // slow no movimento do personagem
        if(blocking)  {
            moveSpeed = slowspeed;
        }
        else { 
            moveSpeed = 4;  
        }
      
        
        // Input de dash 
        if (grounded){
			canDash = true;
            
        }

        // Input do paraquedas
        if (grounded) {
			this.parachute.CloseParachute ();
		} else {
		    if (Input.GetButtonDown("Jump")) {
				this.parachute.OpenParachute (); 
			} else if (Input.GetButtonUp("Jump")) {
				this.parachute.CloseParachute ();
			}
		}

        // Inverte a posição do personagem 
        if (inputX > 0) {
            facingDirection = 1;
            transform.eulerAngles = new Vector2(0, 0);
        } else if (inputX < 0) {
            facingDirection = -1;
            transform.eulerAngles = new Vector2(0, 180);
        }

        // Animação de movimentação do personagem 
        if (grounded) {
            animator.SetBool("Falling", false);
            animator.SetBool("Jumping", false);

            if (rb.velocity.x != 0 && inputX != 0) {
                animator.SetBool("Running", true);
            } else {
                animator.SetBool("Running", false);
            }
        } else {
            animator.SetBool("Running", false);

            if (rb.velocity.y > 0) {
                animator.SetBool("Jumping", true);
                animator.SetBool("Falling", false);
            } if (rb.velocity.y < 0) {
                animator.SetBool("Falling", true);
                animator.SetBool("Jumping", false);
            }
        }

        // Morte do personagem
        if (currentHealth <= 0 && !blockInput) {
            animator.SetTrigger("Deading");
            blockInput = true;
            moveSpeed = 0;
            inputX = 0;
        }
    }

    void FixedUpdate() {

      
        if (isDashing) {
            return;
        }
        

        // Movimentação do personagem 
        rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);

        // Pulo do personagem
        if (jumping) {
            rb.velocity = Vector2.up * jumpForce;
            jumping = false;
        }

        // Aumenta o cronômetro que controla o combo de ataque
        timeSinceAttack += Time.deltaTime; 
    }
    
    // Dash do personagem 
  private void Dash() {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            ghost.makeGhost = true;
            isDashing = true;
            canDash = false;
            dashDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (dashDir == Vector2.zero)
            {
                dashDir = new Vector2(transform.localScale.x, 0);
            }
            StartCoroutine(stopDash());
        }

        if (isDashing)
        {
            this.parachute.CloseParachute();
            rb.velocity = dashDir.normalized * dashVel;
            return;
        }

        IEnumerator stopDash()
        {
            yield return new WaitForSeconds(dashTime);
            ghost.makeGhost = false;
            isDashing = false;
            rb.velocity = Vector2.zero;
            if (grounded){
                yield return new WaitForSeconds(dashTime);
                canDash = true;
            }
        }
    }

    // Dano do personagem
    void OnTriggerEnter2D(Collider2D other) {
        if(colliding)
            return;
        colliding = true;

        if (other.gameObject.tag == "Damage" && currentHealth > 0) {
            TakeDamage(damageEnemy);
            if (currentHealth > 0)
            animator.SetTrigger("Hunting");
        }
    }

    // Diminui a barra de vida
    void TakeDamage(int damage) {
		currentHealth -= damage;
		healthBar.SetHealth(currentHealth);
	}

    // Finalização do ataque
    void EndAnimationATK() {
        attacking = false;
    }

    // Mostra o raio que detecta o chão 
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundPosition.position, sizeRadius);       
    } 

}
