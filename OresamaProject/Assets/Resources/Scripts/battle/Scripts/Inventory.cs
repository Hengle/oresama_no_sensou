using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour 
{
    public static Inventory inv;

    [SerializeField]
    private List<GameObject> playerItems = new List<GameObject>();

    [SerializeField]
    private List<GameObject> enemyItems = new List<GameObject>();

    public List<GameObject> GetPlayerItems() { return playerItems; }
    public List<GameObject> GetEnemyItems() { return enemyItems; }

    void Awake()
    {
        inv = this;
    }

    public void UseItem(GameObject user, GameObject item) 
    {
        List<GameObject> list = user.tag == "Player" ? playerItems : enemyItems;
        list.Remove(item);
    }

    public void AddItem(GameObject user, GameObject item)
    {
        List<GameObject> list = user.tag == "Player" ? playerItems : enemyItems;
        list.Add(item);
    }
}
