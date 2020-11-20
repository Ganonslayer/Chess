using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FEN : MonoBehaviour //Stores the board positions as a FEN (Forsyth-Edwards Notation), for use in testing the threefold repition rule
{
    private List<string> _savedPositions = new List<string>(); //A list of all previous board positions
    [SerializeField]
    private Global _global = null;
    [SerializeField]
    private Checkmate _checkmate = null;
    private bool _record = false; //A flag so that the tests can be delayed under certain conditions
    private int _counter = 0;
    private int _halfmove = 0; //Half moves for the fifty turn rule
    private int _fullmove = 1;
    private string _enPassent = "-";
    private bool[] _kingMove = {false, false}; //White King, Black King
    private bool[] _rookMove = {false, false, false, false}; //Order is White King, White Queen, Black King, Black Queen

    public void Start() {
        _savedPositions.Add("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq"); //Initial board position
    }

    public void Fullmove() {
        _fullmove += 1;
    }

    public void EnPassent(string newValue) {
        _enPassent = newValue;
    }

    public void EnPassent(GameObject tile) {
        string newValue = "";
        char tempValue = '-';
        float position = tile.transform.position.y + 31.5f;
        position = position/9f;
        tempValue = (char)(97+(int)position);
        newValue += tempValue;
        position = tile.transform.position.x + 31.5f;
        position = position/9f;
        tempValue = (char)(49+(int)position);
    }

    public void FixedUpdate() { //Test if the flag for board position should be recorded
        if (_record | _counter == 1) {
            _counter++;
            _record = false;
        }
        if (_counter == 2) {
            _counter = 0;
            RecordPosition();
        }
    }

    public void Record() { //Set the recording falg
        _record = true;
    }

    public string RecordPosition(bool fullFEN = false) { //The function that records the position and tests if a stalemate has occured
        string fenPosition = "";
        RaycastHit2D[] hitArray = {};
        Vector2 lastTest = new Vector3(0f,0f);
        GameObject lastTestPiece = null;
        int emptyTiles = 0;
        Array.Resize(ref hitArray, 16);
        for (float i = 31.5f; i >= -31.5f; i -= 9f) { //FEN field 1
            hitArray = Physics2D.RaycastAll(new Vector3(-31.5f, i, 0f), Vector2.right, Mathf.Infinity);
            foreach (RaycastHit2D hit in hitArray) {
                if (hit) {
                    if (lastTest == new Vector2(hit.transform.position.x, hit.transform.position.y)) {
                        emptyTiles -= 1;
                        if (!hit.transform.gameObject.CompareTag("Board")) {
                            if (emptyTiles > 0) {
                                fenPosition += emptyTiles.ToString();
                            }
                            fenPosition += GetPieceString(hit.transform.gameObject);
                        }
                        else {
                            if (emptyTiles > 0) {
                                fenPosition += emptyTiles.ToString();
                            }
                        }
                        emptyTiles = 0;
                    }
                    else {
                        lastTest = new Vector2(hit.transform.position.x, hit.transform.position.y);
                        if (hit.transform.gameObject.CompareTag("Board")) {
                            emptyTiles += 1;
                        }
                        else {
                            lastTestPiece = hit.transform.gameObject;
                            if (emptyTiles > 0) {
                                fenPosition += emptyTiles.ToString();
                            }
                            emptyTiles = 0;
                            fenPosition += GetPieceString(hit.transform.gameObject);
                        }
                    }
                }
            }
            if (emptyTiles > 0) {
                fenPosition += emptyTiles.ToString();
            }
            emptyTiles = 0;
            if (i != -31.5f) {
                fenPosition += "/";
            }
        }
        if (_global.PassTurn()) { //FEN field 2
            fenPosition += " w";
        }
        else {
            fenPosition += " b";
        }
        fenPosition += " " + ConstructCastle(); //FEN field 3
        Compare(fenPosition); //Check for stalemate
        if (!fullFEN) {
            _savedPositions.Add(fenPosition); //Add the current FEN to the list
        }
        else {
            fenPosition += " " + _enPassent + " " + _halfmove + " " + _fullmove;
        }
        return (fenPosition);
    }

    private void Compare(string position) { //Check if stalemate has occured
        int count = 0;
        foreach (string test in _savedPositions) {
            if (test == position) {
                count += 1;
            }
        }
        if (count >= 2) {
            _checkmate.EnterStalemate("Threefold Repition Rule!");
        }
    }

    public void RookMove(int color, int kingSide) { //Update the rooks for field 3 of FEN
        if (color == 1) {color = 2;}
        _rookMove[color+kingSide] = true;
    }

    public void KingMove(int color) { //Update the kings for field 3 of FEN
        _kingMove[color] = true;
    }

    public void Halfmove() { //Update the halfmove
        _halfmove += 1;
        if (_halfmove >= 50) {
            _checkmate.EnterStalemate("Fifty Turn Rule!");
        }
    }

    public void ResetHalfmove() { //Reset the halfmove is a pawnmove or piece capture occurs
        _halfmove = 0;
        _savedPositions.Clear();
    }
    
    private string ConstructCastle() { //Construct FEN field 3
        string returnValue = "";
        if (!_kingMove[0] & !_kingMove[1]) {
            for (int i = 0; i < 4; i++) {
                if (!_rookMove[i]) {
                    switch (i) {
                    case 0:
                        returnValue += "K";
                        break;
                    case 1:
                        returnValue += "Q";
                        break;
                    case 2:
                        returnValue += "k";
                        break;
                    case 3:
                        returnValue += "q";
                        break;
                    }
                }
            }
        } 
        else if (!_kingMove[0]) {
            for (int i = 0; i < 2; i++) {
                if (!_rookMove[i]) {
                    switch (i) {
                    case 0:
                        returnValue += "K";
                        break;
                    case 1:
                        returnValue += "Q";
                        break;
                    }
                }
            }
        }
        else if (!_kingMove[1]) {
            for (int i = 2; i < 4; i++) {
                if (!_rookMove[i]) {
                    switch (i) {
                    case 2:
                        returnValue += "k";
                        break;
                    case 3:
                        returnValue += "q";
                        break;
                    }
                }
            }
        }
        else {
            returnValue = "-";
        }
        return returnValue;
    }
    
    private string GetPieceString(GameObject piece) { //Convert a piece into the appropriate character for the FEN
        switch(piece.GetComponent<Piece>().PassPiece()) {
            case "Pawn":
                if (piece.GetComponent<Piece>().PassColor()) {
                    return "P";
                }
                else {
                    return "p";
                }
            case "Rook":
                if (piece.GetComponent<Piece>().PassColor()) {
                    return "R";
                }
                else {
                    return "r";
                }
            case "Knight":
                if (piece.GetComponent<Piece>().PassColor()) {
                    return "N";
                }
                else {
                    return "n";
                }
            case "Bishop":
                if (piece.GetComponent<Piece>().PassColor()) {
                    return "B";
                }
                else {
                    return "b";
                }
            case "Queen":
                if (piece.GetComponent<Piece>().PassColor()) {
                    return "Q";
                }
                else {
                    return "q";
                }
            case "King":
                if (piece.GetComponent<Piece>().PassColor()) {
                    return "K";
                }
                else {
                    return "k";
                }
        }
        return "";
    }
}
