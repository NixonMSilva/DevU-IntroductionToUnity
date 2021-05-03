using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Slider de Health
    [SerializeField] private Slider healthSlider;

    public void UI_UpdateHealth (float value)
    {
        // Verificação para evitar over/underflow das variáveis
        if (value > 100f)
            value = 100;
        else if (value < 0f)
            value = 0;

        // Atualização dos valores do slider
        healthSlider.value = value;
        
    }
}
