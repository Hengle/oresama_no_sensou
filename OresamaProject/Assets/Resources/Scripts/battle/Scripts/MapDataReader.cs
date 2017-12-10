using UnityEngine;
using System.Collections;

public class MapDataReader : MonoBehaviour {

    [SerializeField]
    private TextAsset mapData;

	// Use this for initialization
	void Start ()
    {
        // 0 Name
        // 1 Player/Enemy
        // 2 icon
        // 3 mass
        // 4 job
        // 5 level

        string[] charData = CSVReader.ReadLine(mapData, 1);
        string prefabPath = "";
        GameObject chara = (GameObject)Instantiate(Resources.Load(prefabPath + charData[0]), Vector3.zero, this.transform.rotation);
		/*
        chara.GetComponent<playerMove>().NowPos = int.Parse(charData[3]);
        */
        chara.GetComponent<Character>().SetLevel(int.Parse(charData[5]));




        // read list from MapData.csv
        // get players
        // instanciate each character
        // enter character data






	}
	
}
