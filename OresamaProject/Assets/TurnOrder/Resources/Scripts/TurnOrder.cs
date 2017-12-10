using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TurnOrder : MonoBehaviour {

    public GameObject _iconPrefab;

    public int _iconCount;
    public Vector2 _position;
    public int _padding;

    public Canvas _canvas;
    public List<GameObject> _icons;

    public Queue<GameObject> _chars;

    public void Init(Queue<GameObject> chars)
    {
        //Debug.Log(chars.Count);
        foreach (GameObject go in _icons)
        {
            Destroy(go);
        }

		_icons = new List<GameObject>();

        _chars = chars;
        int i = 0;
        foreach (GameObject c in _chars.ToArray()) 
        {
            GameObject icon = Instantiate(_iconPrefab);
            icon.transform.position = new Vector2(_position.x, _position.y - _padding * i);
            icon.transform.SetParent(_canvas.transform, false);

			icon.GetComponent<Portrait>().face.sprite = c.GetComponent<Character>().faceSprite;
            if (c.GetComponent<Character>().GetHp() <= 0)
                icon.GetComponent<Portrait>().batsu.SetActive(true);

            _icons.Add(icon);
            i++;
        }

        //一番上（現在行動中）のキャラ画像だけ大きくする
        if (_icons.Count > 0)
        {
            _icons[0].transform.localScale = new Vector3(0.9f, 0.9f, 1f);
            RectTransform rect = _icons[0].GetComponent<RectTransform>();
            Vector3 pos = rect.localPosition;
            rect.localPosition = new Vector3(pos.x + 10.0f, pos.y + 12f, pos.z);
            //Debug.Log("nextChara");
            //Debug.Log(rect.position);
        }


    }

}
