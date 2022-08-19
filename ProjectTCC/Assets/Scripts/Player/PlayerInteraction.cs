using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

    // Script de interação
    public static PlayerInteraction      prIn;

    // Vida do personagem
	[HideInInspector] public HealthBar   healthBar;
    private bool                         colliding;

    void Start() {
        // Referencia o script
        prIn = this;
    }

    void Update() {
        // Detecta se está colidindo
        colliding = false; 
    }

    // Dano do personagem
    void OnTriggerEnter2D(Collider2D other) {
        if(colliding)
            return;
        colliding = true;

        if (other.gameObject.tag == "Damage" && Player.pr.currentHealth > 0 && !Player.pr.blocking ) {
            TakeDamage(Calcio.cal.damageTaken);
            if (Player.pr.currentHealth > 0)
                Player.pr.anim.SetTrigger("Hunting");
        }
    }

    // Diminui a barra de vida
    void TakeDamage(int damage) {
		Player.pr.currentHealth -= damage;
		healthBar.SetHealth(Player.pr.currentHealth);
	}
}
