using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private InputField _txt = null;
    [SerializeField]
    private Text _failText = null;
    private Regex rx = new Regex(@"([KQRNBPkqrnbp12345678]{1,8}[/]){7}[KQRNBPkqrnbp12345678]{1,8}[ ](w|b)[ ]((K?Q?k?q?)|-)[ ](([abcdefgh][12345678])|-)[ ]\d+[ ]\d+");


    public void ButtonStart() {
        string textString = "";
        bool failFlag = false;
        int counter = 0;
        MatchCollection matches = rx.Matches(_txt.text);
        if (_txt.text != "") {
            if (matches.Count == 1) {
                matches = Regex.Matches(Regex.Match(_txt.text, @"([KQRNBPkqrnbp12345678]{1,8}[/]){7}[KQRNBPkqrnbp12345678]{1,8}[ ]").Value, @"[Kk]");
                if (matches.Count > 2) {
                    Fail("Invalid FEN");
                    failFlag = true;
                }
                matches = Regex.Matches(Regex.Match(_txt.text, @"([KQRNBPkqrnbp12345678]{1,8}[/]){7}[KQRNBPkqrnbp12345678]{1,8}[ ]").Value, @"([KQRNBPkqrnbp12345678]{1,8})");
                foreach (Match match in matches) {
                    counter = 0;
                    textString = match.Value;
                    foreach (char value in textString) {
                        if ((int)value >= 49 & (int)value <= 56) {
                            counter += (int)value-48;
                        }
                        else {
                            counter += 1;
                        }
                    }
                    if (counter != 8) {
                        Fail("Invalid FEN");
                        failFlag = true;
                        break;
                    }
                }
                if (!failFlag) {
                    PlayerPrefs.SetString("FEN", _txt.text);
                    SceneManager.LoadScene("GameBoard");
                }
                Fail("Invalid FEN");
            }
            Fail("Invalid FEN");
            _txt.gameObject.SetActive(true);
        }
        else {
            PlayerPrefs.SetString("FEN", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            SceneManager.LoadScene("GameBoard");
        }
    }

    private void Fail(string reason) {
        _failText.text = reason;
    }
}