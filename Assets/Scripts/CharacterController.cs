using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // Health e speed serão variáveis comuns para ambos o jogador e os inimigos
    [SerializeField] private float health = 100f;
    [SerializeField] private float speed = 100f;

    // Precisamos do rigidbody para computar os movimentos
    private Rigidbody2D rb;

    // Verifica se o sprite está olhando para a direita ou não
    private bool isFacingRight = true;

    // Verifica se o player pode mexer ou não
    private bool canMove = true;

    // Velocidade atual do objeto usado para cálculos
    private float currentSpeed = 0f;

    // Tempo para suavizar o movimento
    [SerializeField] private float dampeningTime = 0.15f;

    // Awake é executado antes do Start
    private void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();        
    }

    // Update é chamado uma vez por frame
    private void Update()
    {
        
    }

    protected void PerformMove (Vector2 movement, bool canNormalize)
    {
        // Realiza as operações de movimento
        // com base no motor físico
        if (canNormalize)
        {
            movement.Normalize();
        }

        if (canMove)
        {
            AdjustOrientation(movement);
            rb.AddForce(movement * Time.fixedDeltaTime * speed, ForceMode2D.Impulse);
            // rb.MovePosition(rb.position + movement * Time.fixedDeltaTime * speed);
        }

    }

    private void AdjustOrientation (Vector2 movement)
    {
        // Verifica a direção do movimento e 
        // a qual direção o sprite está olhando
        if (isFacingRight && movement.x < 0)
        {
            isFacingRight = false;
            Flip();
            
        }

        if (!isFacingRight && movement.x > 0)
        {
            isFacingRight = true;
            Flip();
        }
    }

    private void Flip ()
    {
        // Muda o sprite de direção dependendo do movimento
        Vector3 objectScale = transform.localScale;
        objectScale.x *= -1;
        transform.localScale = objectScale;
    }

    protected void StopBody ()
    {
        // Queremos manter a velocidade em y por conta da gravidade
        // SmoothDamp suaviza o movimento com base em dampeningTime
        Vector2 dampenedVelocity = new Vector2(Mathf.SmoothDamp(rb.velocity.x, 0f, ref currentSpeed, dampeningTime), rb.velocity.y);
        rb.velocity = dampenedVelocity;
    }
}
