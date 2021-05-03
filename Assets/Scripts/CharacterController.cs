using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // Health e speed serão variáveis comuns para ambos o jogador e os inimigos
    [SerializeField] private float health = 100f;
    [SerializeField] private float speed = 7f;

    // Precisamos do rigidbody para computar os movimentos
    private Rigidbody2D rb;

    // Precisamos do animator para ativar as transições entre as animações
    private Animator anim;

    // Precisamos do renderizador de sprites para efetuar a piscagem
    private SpriteRenderer sprite;

    // Verifica se o sprite está olhando para a direita ou não
    private bool isFacingRight = true;

    // Verifica se o personagem pode mexer ou não
    private bool canMove = true;

    // Verifica se o personagem pode tomar dano
    // (se não está na fase de invulnerabilidade)
    public bool canTakeDamage = true;

    // Verifica se o personagem está atacando ou não
    protected bool isAttacking = false;

    // Verifica se o personagem tem ataque melee ou ranged
    protected bool isMelee = true;

    // Velocidade atual do objeto usado para cálculos
    private float currentSpeed = 0f;

    // Tempo para suavizar o movimento
    [SerializeField] private float dampeningTime = 0.15f;

    // Objeto de ataque (players e inimigos)
    [SerializeField] protected GameObject attackObject;

    // Tempo de duração da invulnerabilidade pós dano
    [SerializeField] private float invulnerabilityTime = 1.5f;

    // Variável de contagem de tempo de espera
    private float invulnerabilityCooldownTimer;

    // Tempo de duração da piscagem de invulnerabilidade
    [SerializeField] private float blinkingTime = 0.4f;

    // Variável de contagem do tempo de piscagem
    private float blinkingCooldownTimer;

    // Variável de controle da piscagem
    private bool isBlinking = false;

    // Cor padrão do sprite
    private Color defaultColor;

    // Cor da piscagem
    [SerializeField] private Color blinkingColor;


    // Awake é executado antes do Start
    protected void Awake ()
    {
        // Inicializa os auxiliares de components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        // Incializa a cor padrão
        defaultColor = sprite.color;

        // Inicializa o contador de invulnerabilidade
        invulnerabilityCooldownTimer = invulnerabilityTime;

        // Inicilizar o contador de piscagem
        blinkingCooldownTimer = blinkingTime;

        // Verifica se o blinking time não é maior que o
        // tempo de invulnerabilidade ou é menor que zero, se sim, defina o
        // para 3 vezes o tempo de invulnerabilidade para
        // garantir no mínimo, 3 piscadas
        if (blinkingTime > invulnerabilityTime || blinkingTime <= 0f)
        {
            blinkingTime = invulnerabilityTime / 3f;
        }
    }

    // Update é chamado uma vez por frame
    protected void Update ()
    {
        // Se a vida do personagem for 0 ou menor, mate-o
        if (health <= 0)
        {
            anim.SetBool("isDying", true);
        }

        // Atualiza o contador de invulnerabilidade, se aplicável
        if (!canTakeDamage)
        {
            UpdateInvulnerabilityTimer();
            UpdateBlinkingTimer();
        }
        else
        {
            // Função de segurança para garantir que o personagem
            // "despisque" quando perder a invulnerabilidade
            sprite.color = defaultColor;
            isBlinking = false;
        }

        // Atualiza as variáveis de controle do animator
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("isMelee", isMelee);
    }

    // Função para pegar ou definir health
    protected float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
        }
    }

    protected bool IsMelee
    {
        get
        {
            return isMelee;
        }
        set
        {
            isMelee = value;
        }
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
            rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
        }

    }

    protected void AdjustOrientation (Vector2 movement)
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

    protected void StartAttack ()
    {
        // Ativa o estado de ataque
        isAttacking = true;
    }

    public void PerformAttack ()
    {
        // Performa a função de ataque
        // um evento chamado através da animação
        isAttacking = false;
    }

    public virtual void ChangeHealth (float delta)
    {
        // Muda o valor de health com base em delta
        Health = Health + delta;
    }

    public void Die ()
    {
        // Para o objeto
        rb.velocity = Vector2.zero;

        // Destrói o objeto
        Destroy(this.gameObject);
    }

    public void TakeDamage (float damage)
    {
        // Verifica se o personagem não está num
        // período de invulnerabilidade e então
        // aplica o dano
        if (canTakeDamage)
        {
            canTakeDamage = false;
            ChangeHealth(-damage);
        }
    }

    private void UpdateInvulnerabilityTimer ()
    {
        // Se o contador tiver zerado, faça alguma coisa
        if (invulnerabilityCooldownTimer <= 0)
        {
            // Reativa a capacidade de receber dano
            canTakeDamage = true;
            invulnerabilityCooldownTimer = invulnerabilityTime;
        }
        invulnerabilityCooldownTimer -= Time.deltaTime;
    }

    private void UpdateBlinkingTimer ()
    {
        // Se o contador tiver zerado, faça alguma coisa
        if (blinkingCooldownTimer <= 0)
        {
            // Pisque
            Blink();
            blinkingCooldownTimer = blinkingTime;
        }
        blinkingCooldownTimer -= Time.deltaTime;
    }

    private void Blink ()
    {
        if (isBlinking)
        {
            // Se estiver piscando, volte ao normal
            sprite.color = defaultColor;
            isBlinking = false;
        }
        else
        {
            // Se não estiver piscando, pisque
            sprite.color = blinkingColor;
            isBlinking = true;
        }
    }
}
