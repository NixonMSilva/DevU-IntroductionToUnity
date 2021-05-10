using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : CharacterController
{
    // pursueRange  : Persegue player a partir dessa distância
    // attackRange  : Ataca player a partir dessa distância
    // retreatRange : Recua do player a partir dessa distância
    [SerializeField] private float pursueRange, attackRange, retreatRange;

    // Objeto do player usado como referência
    // em diversas verificações
    protected GameObject player;

    // Distância do inimigo ao player
    protected float distPlayer;

    // Direção do player, em coordenadas x
    protected float playerDirection;

    // Verifica o estado de movimento
    // -1 - Idle | 2 - Aproximação | 1 - Ataque | 0 - Recuo
    protected float enemyMovementStatus = -1;

    // Threshold de movimento, essa lista é usada
    // para evitar excessivos aninhamentos de if-elses
    protected float[] enemyMovementThreshold = new float[3];

    // Verifica se pode atacar
    protected bool canAttack = true;

    // Tempo de espera para criar outro projétil
    [SerializeField] private float attackCooldown = 2f;

    // Variável de contagem do tempo de espera
    private float attackCooldownTimer;

    // Awake é executado antes do Start
    // base.Awake() copia os conteúdos
    // de CharacterController : Awake ()
    protected new void Awake ()
    {
        base.Awake();

        // Procurar o objeto do player na cena
        // ATENÇÃO: As funções GameObject.Find e afins
        // tem performance ruim, tenha certeza
        // de não usá-las constantemente
        player = GameObject.Find("Player");

        // Cria uma lista de 'thresholds' the ações
        // Imagem em anexo para visualizar
        enemyMovementThreshold = new float[]{ retreatRange, attackRange, pursueRange };

        // Inicializa o contador do ataque
        attackCooldownTimer = attackCooldown;
    }

    // Update é chamado uma vez por frame
    // base.Update() copia os conteúdos
    // de CharacterController : Update ()
    protected new void Update()
    {
        base.Update();

        // Calcula a distância ao player
        distPlayer = Vector2.Distance(transform.position, player.transform.position);

        // Atualiza o contador caso a condição seja verdadeira
        if (!canAttack)
        {
            UpdateTimer();
        }
    }

    // Função genérica de movimentação
    // isNotRetreat verifica se é aproximação
    // ou recuo.
    protected void MoveTowardsPlayer (bool isNotRetreat)
    {
        playerDirection = (this.transform.position.x - player.transform.position.x);
        if (isNotRetreat)
        {
            playerDirection *= -1;
        }

        PerformMove(new Vector2(playerDirection, 0f), true);
    }

    // base.PerformAttack() copia os conteúdos
    // de CharacterController : StartAttack ()
    protected new void StartAttack ()
    {
        if (canAttack)
        {
            base.StartAttack();
        }

        // Se volta em direção ao player quando começar o ataque
        LookAtPlayer();
    }

    // base.PerformAttack() copia os conteúdos
    // de CharacterController : PerformAttack ()
    public new void PerformAttack ()
    {
        base.PerformAttack();
    }

    // Verifica orientação do player e olha pra ele
    private void LookAtPlayer ()
    {
        // Subtração do vetor posição do player com o do inimigo nos dá a resposta
        AdjustOrientation(player.transform.position - transform.position);
    }

    private void UpdateTimer ()
    {
        // Se o contador tiver zerado, faça alguma coisa
        if (attackCooldownTimer <= 0)
        {
            // Reativa a capacidade de ataque
            canAttack = true;
            attackCooldownTimer = attackCooldown;
        }
        attackCooldownTimer -= Time.deltaTime;
    }
}
