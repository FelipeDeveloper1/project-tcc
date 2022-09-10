using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
	
	// Script da barra de vida
	public static HealthBar        hlbr;
	
	public Slider                  slider;
	public Gradient                gradient;
	public Image                   fill;

	void Start() {
		// Referencia o script
		hlbr = this;
	}

	// Define a saúde máxima
	public void SetMaxHealth(int health) {
		slider.maxValue = health;
		slider.value = health;

		fill.color = gradient.Evaluate(1f);
	}

	// Altera o preenchimento da barra de vida
    public void SetHealth(int health) {
		slider.value = health;

		fill.color = gradient.Evaluate(slider.normalizedValue);
	}

}
