using UnityEngine;
using System.Collections;

class EnemyChar
{
    public EnemyChar(string prefab)
    {
        enemyPrefab = prefab;
    }

    private string enemyPrefab;

    public string getEnemyPrefab()
    {
        return enemyPrefab;
    }
}


public class EnemyCharacter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
