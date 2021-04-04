﻿using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public HealthBar healthBar;
    public Slider slider;
    public int currentHealth;
    public Gradient gradient;
    public Image fill;
    public void SetMaxHeath(int Health)
    {
        slider.maxValue = Health;
        slider.value = Health;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetHeath(int health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            currentHealth -= 20;
            this.SetHeath(currentHealth);
        }

    }

}
