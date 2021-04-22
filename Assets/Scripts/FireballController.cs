﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    // Velocidade de voô do projétil
    [SerializeField] private float flightSpeed = 1f;

    // Quais tipos de objetos esse projétil pode se colidir com
    [SerializeField] private string[] collidableTags;

    // Precisamos do rigidbody para computar os movimentos
    private Rigidbody2D rb;

    // Precisamos do animator para ativar as transições entre as animações
    private Animator anim;

    // Verifica se o projétil não está colidindo com algo
    private bool isColliding = false;

    // Awake é executado antes do Start
    protected void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // FixedUpdate é chamado uma quantidade fixa de vezes
    // por tempo
    // base.Update() copia os conteúdos
    // de CharacterController : Update ()
    private void FixedUpdate ()
    {
        // Verifica se não há uma colisão ocorrendo
        if (!isColliding)
        {
            Vector2 movement = new Vector2(transform.localScale.x * flightSpeed * Time.fixedDeltaTime, 0f);

            // Translada o projétil
            rb.MovePosition((Vector2)transform.position + movement);
        }
    }

    // Quando o colisor da fireball encosta noutro
    private void OnTriggerEnter2D (Collider2D collision)
    {
        // Verifica se não encostou num objeto cuja
        // tag está marcada como colisível
        foreach (string tag in collidableTags)
        {
            

            if (collision.gameObject.CompareTag(tag))
            {
                // Impede o projétil de se mover durante a colisão
                isColliding = true;

                Debug.Log("Atingiu alvo");

                // Ativa animação de colisão
                anim.SetBool("isColliding", true);
            }
            
        }
    }

    // Quando o objeto sai da tela
    // i.e. nenhuma câmera está apontada p/ ele
    private void OnBecameInvisible ()
    {
        // Destrói esse objeto logo após ele sair da tela em 3 segundos
        Destroy(gameObject, 3f);
    }

    // Destrói o objeto quando termina a animação de explosão
    public void DestroyFireball ()
    {
        Destroy(gameObject);
    }
}