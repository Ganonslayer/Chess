using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour //Just an easter egg that came from a joke in my family, if you spend too long thinking about your move, the Jepordy theme plays
{
    private float _oldTime = 0f;
    [SerializeField]
    private AudioSource _annoyance = null; //The component with the audio
    private bool _playing = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _oldTime = Time.time; //Sets the time the game started
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time - _oldTime >= 60 & !_playing) { //If a full minute has passed, play the song
            _annoyance.Play();
            _playing = true;
        }
    }

    public void StopSong() { //Update the old time and stop the song
        _oldTime = Time.time;
        _playing = false;
        _annoyance.Stop();
    }
}
