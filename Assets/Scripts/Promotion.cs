using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Promotion : MonoBehaviour
{
    private GameObject _pawn = null;
    [SerializeField]
    private Dropdown _dropdown = null;
    [SerializeField]
    private GameObject[] _prefabs = {};
    [SerializeField]
    private Global _global = null;
    private GameObject _newPiece = null;

    public void SetPawn(GameObject pawn) {
        _pawn = pawn;
    }

    public void OnButtonPress() {
        int i = 0;
        if (_global.PassTurn()) {
            i += 4;
        }
        switch(_dropdown.value + i) {
            case 0:
                break;
            case 1:
                _newPiece = Instantiate(_prefabs[0], _pawn.transform.position, Quaternion.identity);
                Destroy(_pawn);
                break;
            case 2:
                _newPiece = Instantiate(_prefabs[1], _pawn.transform.position, Quaternion.identity);
                Destroy(_pawn);
                break;
            case 3:
                _newPiece = Instantiate(_prefabs[2], _pawn.transform.position, Quaternion.identity);
                Destroy(_pawn);
                break;
            case 4:
                if (_dropdown.value != 0) {
                    _newPiece = Instantiate(_prefabs[3], _pawn.transform.position, Quaternion.identity);
                    Destroy(_pawn);
                    break;
                }
                break;
            case 5:
                _newPiece = Instantiate(_prefabs[4], _pawn.transform.position, Quaternion.identity);
                Destroy(_pawn);
                break;
            case 6:
                _newPiece = Instantiate(_prefabs[5], _pawn.transform.position, Quaternion.identity);
                Destroy(_pawn);
                break;
            case 7:
                _newPiece = Instantiate(_prefabs[6], _pawn.transform.position, Quaternion.identity);
                Destroy(_pawn);
                break;
            case 8:
                _newPiece = Instantiate(_prefabs[7], _pawn.transform.position, Quaternion.identity);
                Destroy(_pawn);
                break;
        }
        _global.AssignMovingPiece(_newPiece);
        _global.Promote(false);
    }
}
