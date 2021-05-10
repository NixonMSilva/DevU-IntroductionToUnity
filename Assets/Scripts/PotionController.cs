using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionController : MonoBehaviour
{
    // Variável que refere ao poder de cura do item
    [SerializeField] private float healingFactor = 100f;

    // Variável que armazena o gerenciador de áudio
    private AudioManager audioManager;

    // Awake é executado antes do Start
    public void Awake ()
    {
        // Incializa o gerenciador de audio pegando seu component
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

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
                // Cura o player pelo fator de cura
                player.TakeDamage(-healingFactor);

                // Toca o som ao pegar o item
                audioManager.PlaySound("Pickup");
            }

            // Destruir o objeto da poção
            Destroy(gameObject);
        }
    }
}
