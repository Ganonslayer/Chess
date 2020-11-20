using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour //All tests related to piece movement
{
    [SerializeField]
    private GameObject[] _dummyKings = {};
    [SerializeField]
    private Global _global = null;
    [SerializeField]
    private Checkmate _checkmate = null;
    [SerializeField]
    private Tests _tests = null;
    private GameObject[] _tiles = {};

    private void Start() { //Start by setting one of the variable used for convienence
        _tiles = GameObject.FindGameObjectsWithTag("Board");
    }

    public int KingCannotMove(GameObject target) { //Return the number of tiles the king cannot move to without entering check, and mark them as not available moves
        List<GameObject> dummyKing = new List<GameObject>();
        GameObject dummyKingPrefab = null;
        HashSet<GameObject> noMove = new HashSet<GameObject>();
        if (_global.PassTurn()) {
            dummyKingPrefab = _dummyKings[0];
        }
        else {
            dummyKingPrefab = _dummyKings[1];
        }
        foreach (GameObject tile in _tiles) {
            if (tile.GetComponent<Tile>().PassThreat(target)) {
                dummyKing.Add(Instantiate(dummyKingPrefab, new Vector3(tile.transform.position.x, tile.transform.position.y, -1), Quaternion.identity));
            }
        }
        noMove = _tests.TestCheck(true);
        foreach (GameObject tile in noMove) {
            tile.GetComponent<Tile>().ChangeCircleRender(false);
            tile.GetComponent<Tile>().ChangeThreat(target, false);
        }
        foreach (GameObject dead in dummyKing) {
            Destroy (dead);
        }
        return (noMove.Count);
    }

    private bool MoveTest(GameObject targetOb, Vector2 target, Vector2 direction, bool move, float distance = Mathf.Infinity, bool skip = false, bool king = false, bool pawn = false) { //Mark and threaten the tiles for a piece in one direction
        RaycastHit2D[] hitArray = Physics2D.RaycastAll(target, direction, distance);
        ContactFilter2D filters = new ContactFilter2D();
        List<Collider2D> tiles = new List<Collider2D>();
        bool legalMove = false;
        filters.NoFilter();
        filters.useTriggers = true;
        foreach(RaycastHit2D hit in hitArray) {
            if (hit.transform.gameObject == targetOb) {
                continue;
            }
            if (!skip) {
                skip = true;
                continue;
            }
            if (((hit.transform.CompareTag("White") == targetOb.CompareTag("White")) | pawn) & !hit.transform.CompareTag("Board")) {
                hit.transform.gameObject.GetComponent<BoxCollider2D>().OverlapCollider(filters, tiles);
                foreach (Collider2D tile in tiles) {
                    if (tile != null) {
                        if (tile.transform.CompareTag("Board")) {
                            if (!king) {
                                tile.transform.gameObject.GetComponent<Tile>().ChangeThreat(targetOb, true);
                            }
                            else {
                                tile.transform.gameObject.GetComponent<Tile>().ChangeThreat(targetOb, false);
                                legalMove = false;
                            }
                            if (move) {
                                tile.transform.gameObject.GetComponent<Tile>().ChangeCircleRender(false);
                            }
                            break;
                        }
                    }
                }
                break;
            }
            else if (hit.transform.CompareTag("Board")) {
                if (!_checkmate.PassCheck() | hit.transform.gameObject.GetComponent<Tile>().Blockable() | king) {
                    hit.transform.gameObject.GetComponent<Tile>().ChangeThreat(targetOb, true);
                    if (move) {
                        hit.transform.gameObject.GetComponent<Tile>().ChangeCircleRender(true);
                    }
                    legalMove = true;
                }
            }
            else if (hit.transform.CompareTag("White") | hit.transform.CompareTag("Black")) {
                hit.transform.gameObject.GetComponent<BoxCollider2D>().OverlapCollider(filters, tiles);
                foreach (Collider2D tile in tiles) {
                    if (tile != null) {
                        if (tile.transform.CompareTag("Board")) {
                            if (!pawn & (!_checkmate.PassCheck() | tile.transform.gameObject.GetComponent<Tile>().Blockable() | king)) {
                                tile.transform.gameObject.GetComponent<Tile>().ChangeThreat(targetOb, true);
                                if (move) {
                                    tile.transform.gameObject.GetComponent<Tile>().ChangeCircleRender(true);
                                }
                                legalMove = true;
                                break;
                            }
                        }
                    }
                }
                if (hit.transform.gameObject.GetComponent<Piece>().PassPiece() != "King") {
                    break;
                }
            }
        }
        return legalMove;
    }

    public bool RookMoveTest(GameObject target, bool move) { //Threaten the tiles for a rook
        bool legal1 = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), Vector2.up, move);
        bool legal2 = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), -Vector2.up, move);
        bool legal3 = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), Vector2.right, move);
        bool legal4 = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), -Vector2.right, move);
        return (legal1 | legal2 | legal3 | legal4);
    }

    public bool BishopMoveTest(GameObject target, bool move) { //Threaten the tiles for a bishop
        bool legal1 = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), (Vector2.right + Vector2.up), move);
        bool legal2 = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), (Vector2.right - Vector2.up), move);
        bool legal3 = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), (-Vector2.right + Vector2.up), move);
        bool legal4 = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), (-Vector2.right - Vector2.up), move);
        return (legal1 | legal2 | legal3 | legal4);
    }

    public bool QueenMoveTest(GameObject target, bool move) { //Threaten the tiles for a queen
        bool legal1 = BishopMoveTest(target, move);
        bool legal2 = RookMoveTest(target, move);
        return (legal1 | legal2);
    }

    public bool KnightMoveTest(GameObject target, bool move) { //Threaten the tiles for a knight
        bool legal1 = MoveTest(target, new Vector2(target.transform.position.x + 18f, target.transform.position.y + 9f), Vector2.up, move, 1f, true);
        bool legal2 = MoveTest(target, new Vector2(target.transform.position.x - 18f, target.transform.position.y + 9f), Vector2.up, move, 1f, true);
        bool legal3 = MoveTest(target, new Vector2(target.transform.position.x + 18f, target.transform.position.y - 9f), Vector2.up, move, 1f, true);
        bool legal4 = MoveTest(target, new Vector2(target.transform.position.x - 18f, target.transform.position.y - 9f), Vector2.up, move, 1f, true);
        bool legal5 = MoveTest(target, new Vector2(target.transform.position.x + 9f, target.transform.position.y + 18f), Vector2.up, move, 1f, true);
        bool legal6 = MoveTest(target, new Vector2(target.transform.position.x - 9f, target.transform.position.y + 18f), Vector2.up, move, 1f, true);
        bool legal7 = MoveTest(target, new Vector2(target.transform.position.x + 9f, target.transform.position.y - 18f), Vector2.up, move, 1f, true);
        bool legal8 = MoveTest(target, new Vector2(target.transform.position.x - 9f, target.transform.position.y - 18f), Vector2.up, move, 1f, true);
        return (legal1 | legal2 | legal3 | legal4 | legal5 | legal6 | legal7 | legal8);
    }

    public bool KingMoveTest(GameObject target, bool move) { //Threaten the tiles for a king
        bool[] legal = {false, false, false, false, false, false, false, false};
        legal[0] = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), Vector2.up, move, 9f, false, true);
        legal[1] = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), -Vector2.up, move, 9f, false, true);
        legal[2] = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), Vector2.right, move, 9f, false, true);
        legal[3] = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), -Vector2.right, move, 9f, false, true);
        legal[4] = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), (Vector2.right + Vector2.up), move, 9f, false, true);//culprit
        legal[5] = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), (Vector2.right - Vector2.up), move, 9f, false, true);
        legal[6] = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), (-Vector2.right + Vector2.up), move, 9f, false, true);
        legal[7] = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), (-Vector2.right - Vector2.up), move, 9f, false, true);
        if (!_checkmate.PassCheck()) {
            CastleTest(target, move); //Check for castling if it is available
        }
        int z = KingCannotMove(target);
        z--;
        foreach (bool legalMove in legal) {
            if (legalMove & z>0) {
                z--;
            }
            else if (legalMove) {
                return true;
            }
        }
        return false;
    }

    public bool PawnMoveTest(GameObject target, bool moved, bool move) { //Threaten and mark potential moves for a pawn
        float distance = 9f;
        List<Collider2D> collisions = new List<Collider2D>();
        ContactFilter2D filters = new ContactFilter2D();
        filters.NoFilter();
        filters.useTriggers = true;
        if (!moved) {
            distance += 9f;
        }
        if (!target.GetComponent<Piece>().PassColor()) {
            distance *= -1;
        }
        bool legal = MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), Vector2.up, move, distance, false, false, true); //Normal Forward Move
        RaycastHit2D[] hitArray = Physics2D.RaycastAll(new Vector2(target.transform.position.x, target.transform.position.y), (Vector2.up+Vector2.right), Mathf.Min(9f, Mathf.Max(-9f, distance)));
        foreach(RaycastHit2D hit in hitArray) { //Capture Right
            if ((hit.transform.CompareTag("Black") & _global.PassTurn()) | (hit.transform.CompareTag("White") & !_global.PassTurn())) {
                Physics2D.OverlapCollider(hit.transform.gameObject.GetComponent<BoxCollider2D>(), filters, collisions);
                    foreach(Collider2D test in collisions) {
                        if (test.gameObject.CompareTag("Board") & (!_checkmate.PassCheck() | test.transform.gameObject.GetComponent<Tile>().Blockable())) {
                            test.transform.GetComponent<Tile>().ChangeThreat(target, true);
                            if (move) {
                                test.transform.GetComponent<Tile>().ChangeCircleRender(true);
                            }
                            legal = true;
                            break;
                        }
                    }
                    break;
            }
        }
        hitArray = Physics2D.RaycastAll(new Vector2(target.transform.position.x, target.transform.position.y), (Vector2.up-Vector2.right), Mathf.Min(9f, Mathf.Max(-9f, distance)));
        foreach(RaycastHit2D hit in hitArray) { //Capture Left
            if ((hit.transform.CompareTag("Black") & _global.PassTurn()) | (hit.transform.CompareTag("White") & !_global.PassTurn())) {
                Physics2D.OverlapCollider(hit.transform.gameObject.GetComponent<BoxCollider2D>(), filters, collisions);
                    foreach(Collider2D test in collisions) {
                        if (test.gameObject.CompareTag("Board") & (!_checkmate.PassCheck() | test.transform.gameObject.GetComponent<Tile>().Blockable())) {
                            test.transform.GetComponent<Tile>().ChangeThreat(target, true);
                            if (move) {
                                test.transform.GetComponent<Tile>().ChangeCircleRender(true);
                            }
                            legal = true;
                            break;
                        }
                    }
                    break;
            }
        }
        hitArray = Physics2D.RaycastAll(new Vector2(target.transform.position.x, target.transform.position.y), Vector2.right, 9f);
        foreach(RaycastHit2D hit in hitArray) { //En Passent Right
            if (((hit.transform.CompareTag("Black") & _global.PassTurn()) | (hit.transform.CompareTag("White") & !_global.PassTurn()))) {
                if ((hit.transform.GetComponent<Piece>().PassPiece() == "Pawn") & (hit.transform.position.y==4.5f | hit.transform.position.y==-4.5f) & hit.transform.GetComponent<Piece>().EnPassent()) {
                    Physics2D.OverlapCollider(hit.transform.gameObject.GetComponent<BoxCollider2D>(), filters, collisions);
                        foreach(Collider2D test in collisions) {
                            if (test.gameObject.CompareTag("Board") & (!_checkmate.PassCheck() | test.transform.gameObject.GetComponent<Tile>().Blockable())) {
                                test.transform.GetComponent<Tile>().ChangeThreat(target, true);
                                if (move) {
                                    test.transform.GetComponent<Tile>().ChangeCircleRender(true);
                                }
                                target.transform.GetComponent<Piece>().SetEnPassent(hit.transform.gameObject);
                                legal = true;
                                break;
                            }
                        }
                    break;
                }
            }
        }
        hitArray = Physics2D.RaycastAll(new Vector2(target.transform.position.x, target.transform.position.y), -Vector2.right, 9f);
        foreach(RaycastHit2D hit in hitArray) { //En Passent Left
            if (((hit.transform.CompareTag("Black") & _global.PassTurn()) | (hit.transform.CompareTag("White") & !_global.PassTurn()))) {
                if ((hit.transform.GetComponent<Piece>().PassPiece() == "Pawn") & (hit.transform.position.y==4.5f | hit.transform.position.y==-4.5f) & hit.transform.GetComponent<Piece>().EnPassent()) {
                    Physics2D.OverlapCollider(hit.transform.gameObject.GetComponent<BoxCollider2D>(), filters, collisions);
                        foreach(Collider2D test in collisions) {
                            if (test.gameObject.CompareTag("Board") & (!_checkmate.PassCheck() | test.transform.gameObject.GetComponent<Tile>().Blockable())) {
                                test.transform.GetComponent<Tile>().ChangeThreat(target, true);
                                if (move) {
                                    test.transform.GetComponent<Tile>().ChangeCircleRender(true);
                                }
                                target.transform.GetComponent<Piece>().SetEnPassent(hit.transform.gameObject);
                                legal = true;
                                break;
                            }
                        }
                    break;
                }
            }
        }
        return legal;
    }

    public void CastleTest(GameObject target, bool move) { //Check for castling being available
        target.GetComponent<Piece>().ChangeRook(null, false, true);
        if (!target.GetComponent<Piece>().PassMoved()) {
            RaycastHit2D[] hitArray = Physics2D.RaycastAll(new Vector2(target.transform.position.x, target.transform.position.y), -Vector2.right);
            foreach (RaycastHit2D hit in hitArray) {
                if (hit.transform.gameObject.CompareTag("Black") | hit.transform.gameObject.CompareTag("White")) {
                    if (hit.transform.gameObject.GetComponent<Piece>().PassPiece() == "Rook" & !hit.transform.gameObject.GetComponent<Piece>().PassMoved() & hit.transform.gameObject != target) {
                        MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), -Vector2.right, move, 18f);
                        target.GetComponent<Piece>().ChangeRook(hit.transform.gameObject, true);
                        break;
                    }
                    else if (hit.transform.gameObject != target) {
                        break;
                    }
                }
            }
            hitArray = Physics2D.RaycastAll(new Vector2(target.transform.position.x, target.transform.position.y), Vector2.right);
            foreach (RaycastHit2D hit in hitArray) {
                if (hit.transform.gameObject.CompareTag("Black") | hit.transform.gameObject.CompareTag("White")) {
                    if (hit.transform.gameObject.GetComponent<Piece>().PassPiece() == "Rook" & !hit.transform.gameObject.GetComponent<Piece>().PassMoved() & hit.transform.gameObject != target) {
                        MoveTest(target, new Vector2(target.transform.position.x, target.transform.position.y), Vector2.right, move, 18f);
                        target.GetComponent<Piece>().ChangeRook(hit.transform.gameObject, true);
                        break;
                    }
                    else if (hit.transform.gameObject != target) {
                        break;
                    }
                }
            }
        }
    }
}
