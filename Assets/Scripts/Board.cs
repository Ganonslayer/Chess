using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _pieces = {}; //Order is KQRNBPkqrnbp (FEN syntax)
    [SerializeField]
    private GameObject _tests = null;
    
    // Start is called before the first frame update
    void Start()
    {
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
        }
        _tests.SetActive(true);
    }
}
