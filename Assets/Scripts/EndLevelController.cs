using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelController : MonoBehaviour
{
    // OnTriggerEnter2D é executado quando um colisor externo
    // colide com o colisor do objeto
    private void OnTriggerEnter2D (Collider2D collision)
    {
        // Verificar se colidiu com o player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player;

            // Se conseguir extrair o player controller do player
            if (collision.gameObject.TryGetComponent<PlayerController>(out player))
            {
                player.GetComponent<PlayerController>().EndLevel();
            }
        }
    }
}
