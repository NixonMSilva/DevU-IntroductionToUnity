using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterController
{
    private float horizontalInput;

    // Colisor da espada
    private BoxCollider2D attackCollider;

    // Filtro de inimigos
    [SerializeField] private ContactFilter2D enemyFilter;

    // Dano do golpe de espada
    [SerializeField] private float swordDamage = 20f;

    // Controlador da User Interface
    private UIController UI_Controller;

    // Awake é executado antes do Start
    // base.Awake() copia os conteúdos
    // de CharacterController : Awake ()
    private new void Awake ()
    {
        base.Awake();

        attackCollider = attackObject.GetComponent<BoxCollider2D>();

        // Pega o controlador da User Interface
        // ATENÇÃO: As funções GameObject.Find e afins
        // tem performance ruim, tenha certeza
        // de não usá-las constantemente
        UI_Controller = GameObject.Find("UI").GetComponent<UIController>();
    }

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

    // base.PerformAttack() copia os conteúdos
    // de CharacterController : PerformAttack ()
    public new void PerformAttack ()
    {
        // Aplica dano aos inimigos
        base.PerformAttack();

        // Lista que receberá os objetos atingidos pelo player
        List<Collider2D> hitObjects = new List<Collider2D>();

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

    // Essa função sobescreve completamente
    // CharacterController : ChangeHealth (float)
    public override void ChangeHealth (float delta)
    {
        // Muda o valor de health com base em delta
        Health = Health + delta;

        // Atualiza o valor da barra de health
        UI_Controller.UI_UpdateHealth(Health);
    }
}
