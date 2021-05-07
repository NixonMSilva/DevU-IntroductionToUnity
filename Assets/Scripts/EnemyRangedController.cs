using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedController : EnemyController
{
    // Projétil usado para o ataque
    [SerializeField] private GameObject attackProjectile;

    //-SOLUÇÃO FIREBALL-//
    // Objeto do projétil instanciado
    private GameObject projectile;

    // Start é chamado quando a cena é carregada
    private void Start ()
    {
        isMelee = false;
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
        // 0 - Recuo | 1 - Ataque | 2 - Aproximação
        for (int i = 0; i < enemyMovementThreshold.Length; ++i)
        {
            if (enemyMovementThreshold[i] > distPlayer)
            {
                enemyMovementStatus = i;
                break;
            }
        }

        /* Forma menos eficiente de se escrever o código acima
         * 
        if (distPlayer <= pursueRange && distPlayer > attackRange)
        {
            enemyMovementStatus = 2;
        }
        else if (distPlayer <= attackRange && distPlayer > retreatRange)
        {
            enemyMovementStatus = 1;
        }
        else if (distPlayer <= retreatRange)
        {
            enemyMovementStatus = 0;
        }
        else
        {
            enemyMovementStatus = -1;
        } 
         *
        */

        // Ações executadas dependendo do valor do 'threshold'
        switch (enemyMovementStatus)
        {
            case 2:
                MoveTowardsPlayer(true);
                break;
            case 1:
                StartAttack();
                break;
            case 0:
                MoveTowardsPlayer(false);
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

            // Cria o projétil com base na posição de attackObject
            projectile = Instantiate(attackProjectile, attackObject.transform.position, attackObject.transform.rotation);

            //-SOLUÇÃO FIREBALL-//
            projectile.GetComponent<FireballController>().SetOrientation(transform.localScale);

            // Executa o som da bola de fogo
            audioManager.PlaySound("Fire");

            // Desativa capacidade de ataque até que possa atacar novamente
            canAttack = false;
        }
    }
}
