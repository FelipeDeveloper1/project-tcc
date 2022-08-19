using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // Script do personagem 
    public static Player                pr;

    // Movimentação
    [Header("Move")]
    [SerializeField] float              moveSpeed = 4.0f;
    public bool                         blockInput; 
    private float                       inputX;              
    private int                         facingDirection;

    // Pulo
    [Header("Jump")]
    [SerializeField] float              jumpForce = 7.5f;
    [SerializeField] bool               jumping;
    private bool                        doubleJump;

    // Ataque
    [Header("Atack")]
    [SerializeField] bool               attacking;
    [SerializeField] float              timeAttack = 0.4f;
    private int                         currentAttack = 0;
    private float                       timeSinceAttack = 0.0f;

    // Bloqueo
    [Header("Block")]
    public bool                         blocking;

    // Dash
    [Header("Dash")]
    [SerializeField] bool               dashing; 
	[SerializeField] float              dashingPower;  
	[SerializeField] float              dashingTime; 
	[SerializeField] float              dashingCooldown; 
	private bool                        canDash = true; 

    // Paraquedas
    [Header("Parachute")]
    public Parachute                    parachute;

    // Reconhecer o chão
    [Header("Ground")]
    [SerializeField] bool               grounded;
    [SerializeField] LayerMask          groundLayer;
    public Transform                    groundPosition;
    public float                        sizeRadius;

    // Vida
    [Header("Health")]
    public int                          maxHealth = 100;
	public int                          currentHealth;
    
    // Componentes 
    [HideInInspector] public Animator   anim;
    private Rigidbody2D                 rb;
    private TrailRenderer               tr;

    void Start() {

        // Referencia o script
        pr = this;

        // Referencia os componentes
        rb =        GetComponent<Rigidbody2D>();
        anim =      GetComponent<Animator>();
        tr =        GetComponent<TrailRenderer>();

        // Define a vida máxima
        currentHealth = maxHealth;
        HealthBar.hlbr.SetMaxHealth(maxHealth);
        
        blockInput = false;
    }

    void Update() {

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
            anim.SetTrigger("Attacking" + currentAttack);

            // Redefine o temporizador
            timeSinceAttack = 0.0f;
        } 

        // Input de bloqueo
        if (Input.GetButtonDown("Fire2") && grounded && !attacking && !blockInput) {
            blocking = true;
            anim.SetTrigger("Blocking");
            anim.SetBool("IdleBlocking", true);
        } else if (Input.GetButtonUp("Fire2")) {
            blocking = false;
            anim.SetBool("IdleBlocking", false);
        }

        // Trava o movimento do personagem
        if (attacking && grounded || blocking) 
            inputX = 0;
        
        // Input de dash 
        if (Input.GetButtonDown("Fire3") && canDash && inputX != 0) 
			StartCoroutine(Dash());

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
            anim.SetBool("Falling", false);
            anim.SetBool("Jumping", false);

            if (rb.velocity.x != 0 && inputX != 0) {
                anim.SetBool("Running", true);
            } else {
                anim.SetBool("Running", false);
            }
        } else {
            anim.SetBool("Running", false);

            if (rb.velocity.y > 0) {
                anim.SetBool("Jumping", true);
                anim.SetBool("Falling", false);
            } if (rb.velocity.y < 0) {
                anim.SetBool("Falling", true);
                anim.SetBool("Jumping", false);
            }
        }

        // Morte do personagem
        if (currentHealth <= 0 && !blockInput) {
            anim.SetTrigger("Deading");
            blockInput = true;
            moveSpeed = 0;
            inputX = 0;
        }
    }

    void FixedUpdate() {

        // Retorna o valor de "dashing"
        if (dashing) {
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
    private IEnumerator Dash() {
		canDash = false; 
        dashing = true;
		float originalGravity = rb.gravityScale; 
		rb.gravityScale = 0f; 
        tr.emitting = true;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower * facingDirection, 0f); 
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
		rb.gravityScale = originalGravity; 
		dashing = false; 
		yield return new WaitForSeconds(dashingCooldown); 
		canDash = true; 
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
