using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    [SerializeField]
    private FEN _fen = null;
    [SerializeField]
    private InputField _txt = null;

    public void Start() {
        if (_txt) {
            _txt.readOnly = true;
        }
    }

    public void OnButtonPress() {
        _txt.text = _fen.RecordPosition(true);
    }

    public void OnMenuButtonPress() {
        SceneManager.LoadScene("GameMenu");
    }
}
