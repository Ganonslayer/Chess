using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    private bool _turnW = true;
    [SerializeField]
    private Audio _audio = null;
    [SerializeField]
    private GameObject _movingPiece = null;
    [SerializeField]
    private Promotion _promotion = null;
    public bool _promoting = false;
    [SerializeField]
    private GameObject[] _promotionUI = {};
    [SerializeField]
    private FEN _fen = null;
    [SerializeField]
    private GameObject[] _checkText = {};
    [SerializeField]
    private Checkmate _checkmate = null;

    public void FixedUpdate() { //Test if a piece capture has occured, check for promotion occuring and update the check UI
        if (_movingPiece != null) {
            _movingPiece.GetComponent<Piece>().TestKill();
        }
        foreach (GameObject canvas in _promotionUI) {
            canvas.SetActive(_promoting);
        }
        if (_checkmate.PassCheck()) {
            if (_turnW) {
                _checkText[0].SetActive(true);
                _checkText[1].SetActive(false);
            }
            else {
                _checkText[0].SetActive(false);
                _checkText[1].SetActive(true);
            }
        }
        else {
            _checkText[0].SetActive(false);
            _checkText[1].SetActive(false);
        }
    }

    public int ChangeTurn() { //Change who's turn it is and preform various checks that occur when the turn changes, mostly involving FEN
        if (_turnW) {
            _turnW = false;
            GameObject[] pieces = GameObject.FindGameObjectsWithTag("White");
            foreach (GameObject pieceOb in pieces) {
                if (pieceOb != _movingPiece) {
                    UnThreaten(pieceOb);
                }
            }
            foreach (GameObject pieceOb in pieces) {
                pieceOb.GetComponent<Piece>().NewTurn();
            }
            if (_movingPiece.GetComponent<Piece>().PassPiece() == "King") {
                _fen.KingMove(0);
            }
            else if (_movingPiece.GetComponent<Piece>().PassPiece() == "Rook") {
                if (_movingPiece.GetComponent<Piece>().PassKingSide()) {
                    _fen.RookMove(0, 0);
                }
                else {
                    _fen.RookMove(0, 1);
                }
            }
        }
        else {
            _turnW = true;
            GameObject[] pieces = GameObject.FindGameObjectsWithTag("Black");
            foreach (GameObject pieceOb in pieces) {
                if (pieceOb != _movingPiece) {
                    UnThreaten(pieceOb);
                }
            }
            foreach (GameObject pieceOb in pieces) {
                pieceOb.GetComponent<Piece>().NewTurn();
            }
            if (_movingPiece.GetComponent<Piece>().PassPiece() == "King") {
                _fen.KingMove(1);
            }
            else if (_movingPiece.GetComponent<Piece>().PassPiece() == "Rook") {
                if (_movingPiece.GetComponent<Piece>().PassKingSide()) {
                    _fen.RookMove(1, 0);
                }
                else {
                    _fen.RookMove(1, 1);
                }
            }
        }
        if (_movingPiece.GetComponent<Piece>().PassPiece() == "Pawn") {
            _fen.ResetHalfmove();
        }
        else {
            _fen.Halfmove();
        }
        _audio.StopSong();
        _fen.Record();
        return 1;
    }

    public void UnCircle() { //Remove the circles from all tiles.
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Board");
        foreach (GameObject tileOb in tiles) {
            tileOb.GetComponent<Tile>().ChangeCircleRender(false);
        }
    }

    public int UnThreaten(GameObject piece) { //Dethreaten the tiles a piece was previously threatening after it moves
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Board");
        foreach (GameObject tileOb in tiles) {
            tileOb.GetComponent<Tile>().ChangeThreat(piece, false);
        }
        return 1;
    }

    public void AssignMovingPiece(GameObject piece) { //Assinging which piece is trying to move
        _movingPiece = piece;
    }

    public GameObject PassMovingPiece() { //Pass the piece trying to move
        return(_movingPiece);
    }

    public bool PassTurn() { //Pass who's turn it is back
        return(_turnW);
    }

    public void Promote(bool start = true) {
        _promotion.SetPawn(_movingPiece);
        _promoting = start;
    }

    public void Disable() { //Disable the board, once the game ends
        GameObject[] things = GameObject.FindGameObjectsWithTag("Board");
        foreach (GameObject tile in things) {
            tile.GetComponent<Tile>().enabled = false;
        }
        things = GameObject.FindGameObjectsWithTag("Black");
        foreach (GameObject piece in things) {
            piece.GetComponent<Piece>().enabled = false;
        }
        things = GameObject.FindGameObjectsWithTag("White");
        foreach (GameObject piece in things) {
            piece.GetComponent<Piece>().enabled = false;
        }
    }
}
