using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour //The script used by each tile of the board
{
    private HashSet<GameObject> _threateningPieces = new HashSet<GameObject>();
    private bool _movingTo = false;
    [SerializeField]
    private Tests _tests = null;
    [SerializeField]
    private FEN _fen = null;
    [SerializeField]
    private Global _global = null;
    private bool _blockable = false; //If a piece starts occupying this tile, will it take the king out of check
    private bool _promotionTile = false;
    
    public void Start() { //Set some values at the start of the game
        if (transform.position.y == 31.5f | transform.position.y == -31.5f) {
            _promotionTile = true;
        }
    }

    // Update is called once per frame
    public void FixedUpdate() //Render the circles for a threatened tile
    {
        if (_movingTo) {
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        }
        else {
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void ChangeThreat(GameObject piece, bool add = true) { //Update who is threatening a tile
        if(!add) {
            _threateningPieces.Remove(piece);
        }
        else if (add) {
            _threateningPieces.Add(piece);
        }
    }

    public void ChangeCircleRender(bool newState = true) { //Change is the circle renders
        _movingTo = newState;
    }

    public void OnMouseDown() { //What happens when you click on a tile
        Movement(false);
    }

    public void Movement(bool killFlag = false) { //Move a piece to this tile, if a piece is currently trying to move
        Piece piece;
        int dummy = 0;
        List<GameObject> rooks = new List<GameObject>();
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Board");
        if (_movingTo) {
            _tests.DelayCheckmate();
            piece = (_global.PassMovingPiece()).GetComponent<Piece>();
            if (piece.PassPiece() == "Pawn") {
                if (Mathf.Abs(_global.PassMovingPiece().transform.position.y) - Mathf.Abs(transform.position.y) == 18f) {
                    _fen.EnPassent(new Vector2(transform.position.x, transform.position.y+(Mathf.Sign(transform.position.y)*9f)));
                }
                else {
                    _fen.EnPassent("-");
                }
            }
            if ((transform.position.y == _global.PassMovingPiece().transform.position.y) & piece.PassPiece() == "Pawn") {
                piece.EnPassented();
            }
            if (Mathf.Abs(transform.position.y) == 31.5f & (_global.PassMovingPiece()).GetComponent<Piece>().PassPiece() == "Pawn") {
                _global.Promote();
            }
            if (((Mathf.Abs(transform.position.x) - Mathf.Abs(_global.PassMovingPiece().transform.position.x) == 18f) | (Mathf.Abs(transform.position.x) - Mathf.Abs(_global.PassMovingPiece().transform.position.x) == 9f)) & piece.PassPiece() == "King") {
                rooks = _global.PassMovingPiece().GetComponent<Piece>().PassRooks();
                foreach (GameObject rook in rooks) {
                    if (rook.transform.position.x*transform.position.x > 0) {
                        if (rook.transform.position.x > 0) {
                            rook.transform.position = new Vector3((_global.PassMovingPiece()).transform.position.x+9f, (_global.PassMovingPiece()).transform.position.y, -1f);
                        }
                        else {
                            rook.transform.position = new Vector3((_global.PassMovingPiece()).transform.position.x-9f, (_global.PassMovingPiece()).transform.position.y, -1f);
                        }
                    }
                }
            }
            (_global.PassMovingPiece()).transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
            dummy = _global.UnThreaten(piece.gameObject);
            dummy = _global.ChangeTurn();
            _global.UnCircle();
            piece.Moved();
        }
    }

    public bool PassThreat(GameObject piece) { //Pass if a given piece is currently threatening this tile
        return(_threateningPieces.Contains(piece));
    }

    public bool PassPromotion() { //Pass if a pawn can be promoted on this tile
        return(_promotionTile);
    }

    public HashSet<GameObject> PassThreats() {
        return(_threateningPieces);
    }

    public void CanBlock(bool newState) {
        _blockable = newState;
    }

    public bool Blockable() {
        return(_blockable);
    }
}
