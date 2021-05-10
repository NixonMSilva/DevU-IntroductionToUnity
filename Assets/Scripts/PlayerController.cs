using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : CharacterController
{
    // Variável que recebe o input horizontal
    private float horizontalInput;

    // Colisor da espada
    private BoxCollider2D attackCollider;

    // Filtro de inimigos
    [SerializeField] private ContactFilter2D enemyFilter;

    // Dano do golpe de espada
    [SerializeField] private float swordDamage = 20f;

    // Força do pulo do personagem
    [SerializeField] private float jumpForce = 10f;

    // Verifica se o personagem está tocando no chão
    [SerializeField] private bool isGrounded = true;

    // Posição onde verificaremos se o player toca no chão
    [SerializeField] private Transform groundCheck;

    // Lista de coisas que consideramos como chão
    [SerializeField] private LayerMask groundMask;

    // Controlador da User Interface
    private UIController UI_Controller;

    // Variável de contagem do cooldown do som de passo
    private float stepSoundCooldownTimer;

    // Tempo de cooldown do som de passo
    [SerializeField] private float stepSoundTime = 0.25f;

    // Verifica se toca ou não o som de passo
    private bool canPlayStepSound = true;

    // Armazena o painel do GameOver
    // Temos que pegar o objeto pelo inspector
    // pois ele está inativo
    [SerializeField] private GameObject gameOverPanel;

    // Awake é executado antes do Start
    // base.Awake() copia os conteúdos
    // de CharacterController : Awake ()
    private new void Awake ()
    {
        base.Awake();

        // Incializa o colisor do objeto de ataque (a espada)
        attackCollider = attackObject.GetComponent<BoxCollider2D>();

        // Pega o controlador da User Interface
        // ATENÇÃO: As funções GameObject.Find e afins
        // tem performance ruim, tenha certeza
        // de não usá-las constantemente
        UI_Controller = GameObject.Find("UI").GetComponent<UIController>();

        // Inicializa o contador de cooldown do som de passo
        stepSoundCooldownTimer = stepSoundTime;

    }

    // FixedUpdate é chamado uma quantidade fixa por segundos
    private void FixedUpdate()
    {
        // Gerencia o input horizontal
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Verifica se o player não está no chão
        VerifyGrounded();

        // Realiza o movimento caso o input seja diferente de zero
        // i.e. há input
        if (!Mathf.Approximately(horizontalInput, 0f))
        {
            // Executa o movimento
            PerformMove(new Vector2(horizontalInput, 0f), false);

            // Executa o som de passo somente se o player estiver no chão
            // e pode tocar o som de passo
            if (isGrounded && canPlayStepSound)
            {
                canPlayStepSound = false;
                audioManager.PlaySound("Walk");
            }
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

        // Gerencia o input de pulo
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            PerformJump();
        }

        // Atualiza a variável do animator
        anim.SetBool("isGrounded", isGrounded);
    }

    // Update é chamado uma vez por frame
    // base.Update() copia os conteúdos de
    // CharacterController : Update ()
    private new void Update ()
    {
        base.Update();

        // Inicia o cooldown do som do passo
        // caso ele tenha tocado recentemente
        if (!canPlayStepSound)
        {
            UpdateWalkSoundTimer();
        }
    }

    // base.PerformAttack() copia os conteúdos
    // de CharacterController : PerformAttack ()
    public new void PerformAttack ()
    {
        // Aplica dano aos inimigos
        base.PerformAttack();

        // Lista que receberá os objetos atingidos pelo player
        List<Collider2D> hitObjects = new List<Collider2D>();

        // Executa o som do ataque
        audioManager.PlaySound("Slash");

        // Verifica quais objetos tocam a espada, e os filtra
        // com base na tag "enemy"
        if (attackCollider.OverlapCollider(enemyFilter.NoFilter(), hitObjects) > 0)
        {
            foreach (Collider2D collision in hitObjects)
            {
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    collision.gameObject.GetComponent<CharacterController>().TakeDamage(swordDamage);
                }
            }
        }
    }

    // base.ChangeHealth copia os conteúdos de
    // CharacterController : ChangeHealth (float)
    public override void ChangeHealth (float delta)
    {
        base.ChangeHealth(delta);

        // Atualiza o valor da barra de health
        UI_Controller.UI_UpdateHealth(Health);
    }

    private void PerformJump ()
    {
        // Executa o som do pulo
        audioManager.PlaySound("Jump");

        // Adicionar uma força no eixo Y ao RigidBody do personagem
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;
    }

    private void VerifyGrounded ()
    {
        // Verifica através de um círculo de raio 0.2 para ver se o player não
        // está 'tocando' em algum objeto cuja camada é Ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.1f, groundMask);

        // Para cada objeto que colidiu que não seja o próprio player
        // então ele está no chão
        foreach (Collider2D collision in colliders)
        {
            if (collision.gameObject != this.gameObject)
            {
                isGrounded = true;
            }
        }
    }

    private void UpdateWalkSoundTimer ()
    {
        // Se o contador tiver zerado, faça alguma coisa
        if (stepSoundCooldownTimer <= 0)
        {
            // Reativa a capacidade de executar
            // o som de passo
            canPlayStepSound = true;
            stepSoundCooldownTimer = stepSoundTime;
        }
        stepSoundCooldownTimer -= Time.deltaTime;
    }

    // Reescreve a função Die() em CharacterController
    // para processar a morte do Player, que é única
    public new void Die ()
    {
        // Para o objeto
        rb.velocity = Vector2.zero;

        // Deleta o sprite do player
        GetComponent<SpriteRenderer>().enabled = false;

        // Manda o player para longe pra impedir que os inimigos continuem atacando
        transform.position = new Vector3(-32f, 10f, 0f);

        // Chama a tela de GameOver
        gameOverPanel.SetActive(true);

        // Impede o player de ser controlado e destroi esse script (PlayerController.cs)
        // o animator e o rigidbody
        DestroyComponents();
    }

    public void EndLevel ()
    {
        // Para o objeto
        rb.velocity = Vector2.zero;

        // Chama a tela de GameOver
        gameOverPanel.SetActive(true);

        // Impede o player de ser controlado e destroi esse script (PlayerController.cs)
        // o animator e o rigidbody
        DestroyComponents();
    }

    private void DestroyComponents ()
    {
        // Impede o player de ser controlado e destroi esse script (PlayerController.cs)
        // e o animator
        Destroy(this.GetComponent<CapsuleCollider2D>());
        Destroy(rb);
        Destroy(anim);
        Destroy(this);
    }
}
