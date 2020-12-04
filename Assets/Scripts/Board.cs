using System.Collections;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _pieces = {}; //Order is KQRNBPkqrnbp (FEN syntax)
    [SerializeField]
    private GameObject _tests = null;
    [SerializeField]
    private Global _global = null;
    [SerializeField]
    private FEN _fen = null;
    
    // Start is called before the first frame update
    void Start()
    {
        float[] positions = {0f,0f};
        List<GameObject> kings = new List<GameObject>();
        List<GameObject> rooks = new List<GameObject>();
        string textString = "";
        List<GameObject> objects = new List<GameObject>();
        Vector3 position = new Vector3(-31.5f, 31.5f, -1f);
        string boardPosition = PlayerPrefs.GetString("FEN", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        MatchCollection matches = Regex.Matches(Regex.Match(boardPosition, @"([KQRNBPkqrnbp12345678]{1,8}[/]){7}[KQRNBPkqrnbp12345678]{1,8}[ ]").Value, @"([KQRNBPkqrnbp12345678]{1,8})");
        foreach (Match match in matches) {
            textString = match.Value;
            foreach (char value in textString) {
                switch ((int)value) { //32
                case 75:
                    objects.Add(Instantiate(_pieces[0], position, Quaternion.identity));
                    position = position + new Vector3(9f, 0f, 0f);
                    break;
                case 81:
                    objects.Add(Instantiate(_pieces[1], position, Quaternion.identity));
                    position = position + new Vector3(9f, 0f, 0f);
                    break;
                case 82:
                    objects.Add(Instantiate(_pieces[2], position, Quaternion.identity));
                    position = position + new Vector3(9f, 0f, 0f);
                    break;
                case 78:
                    objects.Add(Instantiate(_pieces[3], position, Quaternion.identity));
                    position = position + new Vector3(9f, 0f, 0f);
                    break;
                case 66:
                    objects.Add(Instantiate(_pieces[4], position, Quaternion.identity));
                    position = position + new Vector3(9f, 0f, 0f);
                    break;
                case 80:
                    objects.Add(Instantiate(_pieces[5], position, Quaternion.identity));
                    position = position + new Vector3(9f, 0f, 0f);
                    break;
                case 107:
                    objects.Add(Instantiate(_pieces[6], position, Quaternion.identity));
                    position = position + new Vector3(9f, 0f, 0f);
                    break;
                case 113:
                    objects.Add(Instantiate(_pieces[7], position, Quaternion.identity));
                    position = position + new Vector3(9f, 0f, 0f);
                    break;
                case 114:
                    objects.Add(Instantiate(_pieces[8], position, Quaternion.identity));
                    position = position + new Vector3(9f, 0f, 0f);
                    break;
                case 110:
                    objects.Add(Instantiate(_pieces[9], position, Quaternion.identity));
                    position = position + new Vector3(9f, 0f, 0f);
                    break;
                case 98:
                    objects.Add(Instantiate(_pieces[10], position, Quaternion.identity));
                    position = position + new Vector3(9f, 0f, 0f);
                    break;
                case 112:
                    objects.Add(Instantiate(_pieces[11], position, Quaternion.identity));
                    position = position + new Vector3(9f, 0f, 0f);
                    break;
                default:
                    position = position + new Vector3(9f*((int)value-48), 0f, 0f);
                    break;
                }
            }
            position = position + new Vector3(-72f, -9f, 0f);
        }
        foreach (GameObject inactive in objects) {
            inactive.SetActive(true);
            if (inactive.GetComponent<Piece>().PassPiece() == "King") {
                kings.Add(inactive);
            }
            else if (inactive.GetComponent<Piece>().PassPiece() == "Rook") {
                rooks.Add(inactive);
            }
        }
        matches = Regex.Matches(boardPosition, @"[ ]\d+[ ]");
        foreach(Match match in matches) {
            _fen.Halfmove(Convert.ToInt16(match.Value));
        }
        matches = Regex.Matches(boardPosition, @"[ ]\d+$");
        foreach(Match match in matches) {
            _fen.Fullmove(Convert.ToInt16(match.Value));
        }
        matches = Regex.Matches(boardPosition, @"[ ][wb]{1}[ ]");
        foreach(Match match in matches) {
            _global.SetTurn(match.Value);
        }
        matches = Regex.Matches(boardPosition, @"([ ][KQkq]{1,4})|[-]");
        foreach (GameObject rook in rooks) {
            rook.GetComponent<Piece>().Moved();
            rook.GetComponent<Piece>().Start();
        }
        foreach(Match match in matches) {
            foreach (char value in match.Value) {
                if (value == '-') {
                    foreach (GameObject king in kings) {
                        king.GetComponent<Piece>().Moved();
                        _fen.KingMove(0);
                        _fen.KingMove(1);
                    }
                }
                else {
                    switch (value) {
                    case 'K':
                        foreach(GameObject rook in rooks) {
                            if(rook.GetComponent<Piece>().PassColor() & rook.GetComponent<Piece>().PassKingSide()) {
                                rook.GetComponent<Piece>().SetMoved(false);
                            }
                        }
                        break;
                    case 'k':
                        foreach(GameObject rook in rooks) {
                            if(!rook.GetComponent<Piece>().PassColor() & rook.GetComponent<Piece>().PassKingSide()) {
                                rook.GetComponent<Piece>().SetMoved(false);
                            }
                        }
                        break;
                    case 'Q':
                        foreach(GameObject rook in rooks) {
                            if(rook.GetComponent<Piece>().PassColor() & !rook.GetComponent<Piece>().PassKingSide()) {
                                rook.GetComponent<Piece>().SetMoved(false);
                            }
                        }
                        break;
                    case 'q':
                        foreach(GameObject rook in rooks) {
                            if(!rook.GetComponent<Piece>().PassColor() & !rook.GetComponent<Piece>().PassKingSide()) {
                                rook.GetComponent<Piece>().SetMoved(false);
                            }
                        }
                        break;
                    }
                }
            }
            foreach (GameObject rook in rooks) {
                if (rook.GetComponent<Piece>().PassMoved()) {
                    _fen.RookMove(Convert.ToInt16(!rook.GetComponent<Piece>().PassColor()), Convert.ToInt16(!rook.GetComponent<Piece>().PassKingSide()));
                }
            }
            break;
        }
        matches = Regex.Matches(boardPosition, @"[ ]([abcdefgh]{1}[12345678]{1})[ ]");
        foreach (Match match in matches) {
            _fen.EnPassent(match.Value.Trim());
            foreach(char value in match.Value) {
                if ((int)value >= 97 & (int)value <= 104) {
                    positions[0] = (((float)value-97f)*9f)-31.5f;
                }
                if ((int)value >= 49 & (int)value <= 56) {
                    positions[1] = (((float)value-49f)*9f)-31.5f;
                    if ((int)value == 51) {
                        positions[1] += 9f;
                    }
                    else {
                        positions[1] -= 9f;
                    }
                }
            }
            Collider2D[] collisions = Physics2D.OverlapCircleAll(new Vector2(positions[0], positions[1]), 9f);
            foreach (Collider2D collision in collisions) {
                if (collision.gameObject.CompareTag("Black") | collision.gameObject.CompareTag("White")) {
                    collision.gameObject.GetComponent<Piece>().SetEnPassentable(true);
                }
            }
        }
        _tests.SetActive(true);
    }
}
