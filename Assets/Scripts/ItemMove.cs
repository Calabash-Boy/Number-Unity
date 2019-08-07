using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMove : MonoBehaviour
{
    // Start is called before the first frame update
    Item item;
    private void Awake()
    {
        item = GetComponent<Item>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(int newX, int newY)
    {
        item.X = newX;
        item.Y = newY;
        //item.transform.localPosition = item.
            
    }

}
