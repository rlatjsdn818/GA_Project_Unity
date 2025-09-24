using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //선형 탐색
    public List<Item> items = new List<Item>();

    // Start is called before the first frame update
    public void Start()
    {
        items.Add(new Item("Sword"));
        items.Add(new Item("Shield"));
        items.Add(new Item("Potion"));

        Item found = FindItem("Potion");

        if (found != null)
            Debug.Log("찾은 아이템: " + found.itemName);
        else
            Debug.Log("아이템을 찾을 수 없습니다.");
    }

    public Item FindItem(string _itemName)
    {
        foreach(var item in items)
        {
            if (item.itemName == _itemName)
                return item;
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
