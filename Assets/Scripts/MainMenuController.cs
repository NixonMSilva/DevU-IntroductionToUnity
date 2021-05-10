using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Função que executa quando o botão "Play" é clicado
    public void PlayGame ()
    {
        SceneManager.LoadScene("Map01");
    }

    // Função que executa quando o botão "Exit" é clicado

    public void ExitGame ()
    {
        Application.Quit();
    }

    // Função que executa quando o botão dentro do jogo "To Main Menu" é clicado
    public void ToMainMenu ()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
