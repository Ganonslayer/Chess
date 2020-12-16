using System;
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
    private Regex rx = new Regex(@"^([KQRNBPkqrnbp12345678]{1,8}[/]){7}[KQRNBPkqrnbp12345678]{1,8}[ ](w|b)[ ]((K?Q?k?q?)|-)[ ](([abcdefgh][12345678])|-)[ ]\d+[ ]\d+$");

    public void ButtonStart() { //The code that validates the FEN given using RegEx and then saves that and loads the game board when you click the button
        string textString = "";
        bool failFlag = false;
        int counter = 0;
        MatchCollection matches = rx.Matches(_txt.text);
        if (_txt.text != "") {
            if (matches.Count == 1) {
                matches = Regex.Matches(Regex.Match(_txt.text, @"([KQRNBPkqrnbp12345678]{1,8}[/]){7}[KQRNBPkqrnbp12345678]{1,8}[ ]").Value, @"[K]");
                if (matches.Count != 1) {
                    Fail("Invalid FEN\nThere is an incorrect number of white kings");
                    failFlag = true;
                }
                matches = Regex.Matches(Regex.Match(_txt.text, @"([KQRNBPkqrnbp12345678]{1,8}[/]){7}[KQRNBPkqrnbp12345678]{1,8}[ ]").Value, @"[k]");
                if (matches.Count != 1) {
                    Fail("Invalid FEN\nThere is an incorrect number of black kings");
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
                        Fail("Invalid FEN\nOne row has too many position taken up");
                        failFlag = true;
                    }
                }
                matches = Regex.Matches(Regex.Match(_txt.text, @"[ ]\d+[ ]\d+").Value, @"[ ]\d+[ ]");
                foreach (Match match in matches) {
                    if (Int16.Parse(match.Value) >= 50) {
                        Fail("Invalid FEN\nMore than 50 halfturns have passed without an irreversable change, a stalemate has occured.");
                        failFlag = true;
                    }
                }
                if (!failFlag) {
                    PlayerPrefs.SetString("FEN", _txt.text);
                    SceneManager.LoadScene("GameBoard");
                    return;
                }
            }
            else {
                Fail("Invalid FEN\nYou have given a FEN in the wrong format");
            }
        }
        else {
            PlayerPrefs.SetString("FEN", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            SceneManager.LoadScene("GameBoard");
        }
    }

    private void Fail(string reason) {
        _failText.text = reason;
        _failText.gameObject.SetActive(true);
    }
}