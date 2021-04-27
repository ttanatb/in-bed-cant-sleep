using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caw : Utilr.Singleton<Caw>
{
    AudioSource m_audio = null;
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out m_audio);
    }

    public void Play()
    {
        m_audio.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
