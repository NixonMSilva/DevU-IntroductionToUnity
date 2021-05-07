using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // Lista de sons
    [SerializeField] private Sound[] sounds;

    // Instância do manager (singleton)
    public static AudioManager instance;

    void Awake ()
    {
        // Verifica se já existe uma instância desse
        // manager, que deve ser um singleton (única instância)
        // caso já exista, delete esta.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Itera pela lista de sons e adiciona uma fonte
        // de áudio no objeto
        foreach (Sound sound in sounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.clip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
        }
    }

    // Executa um som com o nome adequado
    public void PlaySound (string name)
    {
        // Encontre na array onde temos um som cujo sound.name é igual a name
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.audioSource.Play();
    }
}
