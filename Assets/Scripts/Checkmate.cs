using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Checkmate : MonoBehaviour
{
    private bool _check = false;
    [SerializeField]
    private GameObject _text = null; //This and _txt are the UI components that are used to indicate checkmate and stalemate
    [SerializeField]
    private Text _txt = null;
    [SerializeField]
    private Global _global = null;

    public void EnterCheck() {
        _check = true;
    }

    public void ExitCheck() {
        _check = false;
    }

    public bool PassCheck() {
        return(_check);
    }

    public void EnterCheckmate(bool white) {
        if (white) {
            _txt.text = "Checkmate!\nWhite Wins!";
        }
        else {
            _txt.text = "Checkmate!\nBlack Wins!";
        }
        _text.SetActive(true);
        _global.Disable();
    }

    public void EnterStalemate(string reason) {
        _txt.text = "Stalemate!\nNo Winner!\n" + reason;
        _text.SetActive(true);
        _global.Disable();
    }
}
