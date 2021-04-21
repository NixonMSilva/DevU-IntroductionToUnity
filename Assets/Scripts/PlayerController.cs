using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterController
{
    private float horizontalInput;

    // FixedUpdate é chamado uma quantidade fixa por segundos
    private void FixedUpdate()
    {
        // Gerencia o input horizontal
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Realiza o movimento caso o input seja diferente de zero
        // i.e. há input
        if (!Mathf.Approximately(horizontalInput, 0f))
        {
            PerformMove(new Vector2(horizontalInput, 0f), false);
        }
        // Caso contrário, pare o objeto para ele não deslizar
        else
        {
            StopBody();
        }

        // Gerencia o input do ataque
        if (!isAttacking && Input.GetButtonDown("Fire1"))
        {
            StartAttack();
        }

    }
}
