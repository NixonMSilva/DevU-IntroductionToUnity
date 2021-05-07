using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    [HideInInspector]
    public AudioSource audioSource;

    // Nome do áudio
    public string name;

    // Clipe de áudio (o arquivo)
    public AudioClip clip;

    // Volume do áudio, pode ir de 0 (0%) à 1 (100%)
    [Range(0f, 1f)]
    public float volume;

    // Tom do áudio, pode ir de 0.1 à 3, quanto menor,
    // mais grave, quanto maior, mais agudo
    [Range(.1f, 3f)]
    public float pitch;

}
