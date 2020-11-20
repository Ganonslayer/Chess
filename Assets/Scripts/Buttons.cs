using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    [SerializeField]
    private FEN _fen = null;
    [SerializeField]
    private InputField _txt = null;

    public void Start() {
        _txt.readOnly = true;
    }
    public void OnButtonPress() {
        _txt.text = _fen.RecordPosition(true);
    }
}
