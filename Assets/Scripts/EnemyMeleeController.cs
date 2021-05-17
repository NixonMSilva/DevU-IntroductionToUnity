using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeController : EnemyController
{
    // CharacterController do player é armazenado para não
    // ser pego toda que toma dano vez por um GetComponent<>
    private CharacterGenericController playerController;

    // Valor do dano do ataque
    [SerializeField] private float meleeDamage = 25f;

    // Awake é chamado uma vez por frame
    // base.Awake() copia os conteúdos
    // de EnemyController : Awake ()
    private new void Awake ()
    {
        base.Awake();

        // Armazena o characterController do Player para futuros usos
        playerController = player.GetComponent<CharacterGenericController>();

        // Uma forma mais eficiente de se fazer a ação acima
        // (para várias instâncias desta classe, por exemplo)
        // seria criar um PlayerListener no mapa, que conteria
        // armazenado o CharacterController do player, e toda
        // vez que for preciso aplicar um dano nele, essa classe
        // do inimigo acessa a referência do PlayerListener e então
        // a partir dela aplica o dano.
    }

    // Update é chamado uma vez por frame
    // base.Update() copia os conteúdos
    // de EnemyController : Update ()
    private new void Update ()
    {
        base.Update();

        // Antes de verificar o threshold, ir para o estado
        // padrão: idle
        enemyMovementStatus = -1;

        // Verifica o 'threshold' em que a distância se encaixa
        // 0..1 - Ataque | 2 - Aproximação
        for (int i = 0; i < enemyMovementThreshold.Length; ++i)
        {
            if (enemyMovementThreshold[i] > distPlayer)
            {
                enemyMovementStatus = i;
                break;
            }
        }

        // Verificação para normalizar os resultados
        if (enemyMovementStatus == 0)
            enemyMovementStatus = 1;

        // Ações executadas dependendo do valor do 'threshold'
        switch (enemyMovementStatus)
        {
            case 2:
                MoveTowardsPlayer(true);
                break;
            case 1:
                StartAttack();
                break;
        }
    }

    // base.PerformAttack() copia os conteúdos
    // de EnemyController : PerformAttack ()
    public new void PerformAttack ()
    {
        // Antes de tudo, verifica se é
        // possível atacar
        if (canAttack)
        {
            base.PerformAttack();

            // Faz um Raycast, isto é, dispara um raio virtual
            // que então verifica se há algo colidindo com este
            // raio
            RaycastHit2D[] targetsHit = Physics2D.RaycastAll(attackObject.transform.position, Vector3.right, 1f);

            Debug.DrawRay(attackObject.transform.position, Vector3.right, Color.magenta, 3f);

            // Itera através dos objetos com que o raio colide
            // e procura pelo player, para aplicar o dano
            foreach(RaycastHit2D hit in targetsHit)
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    // Executa o som do ataque conectando
                    audioManager.PlaySound("Bite");

                    // Aplica o dano
                    playerController.TakeDamage(meleeDamage);
                }
            }

            // Desativa capacidade de ataque até que possa atacar novamente
            canAttack = false;
        }
    }
}
