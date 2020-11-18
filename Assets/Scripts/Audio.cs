using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    private float _oldTime = 0f;
    [SerializeField]
    private AudioSource _annoyance = null;
    private bool _playing = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _oldTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time - _oldTime == 60 & !_playing) {
            _annoyance.Play();
            _playing = true;
        }
    }

    public void StopSong() {
        _oldTime = Time.time;
        _playing = false;
        _annoyance.Stop();
    }
}
