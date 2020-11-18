using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Checkmate : MonoBehaviour
{
    private int _checkmateCounter = 0;
    private bool _check = false;
    [SerializeField]
    private GameObject _text = null;
    [SerializeField]
    private Text _txt = null;
    [SerializeField]
    private Global _global = null;

    public void EnterCheck() {
        _check = true;
    }

    public void ExitCheck() {
        _check = false;
        _checkmateCounter = 0;
    }

    public bool PassCheck() {
        return(_check);
    }

    public void TestIncrement(bool white) {
        if (_check) {
            _checkmateCounter += 1;
        }
        if (_checkmateCounter == 1) {
            EnterCheckmate(white);
        }
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
