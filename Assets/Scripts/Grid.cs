﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //布局参数
    float cameraHeight;
    float cameraWidth;
    float aspectRatio;
    private float margin;
    private float itemSpace;
    private float itemWidth;

    private float width;
    private float height;

    // 外部设定参数
    public int column;
    public int row;
    public float moveUpTime;

    public GameObject backgroundPrefab;
    public GameObject itemPrefab;

    //私有变量
    private Item[,] items;          //item array

    private GameObject mouseOnBack;             //记录最后位置
    private GameController gameController;
    private bool isDragingItem;
    private bool isMovingItem;
    private Item mergingItem;

    public float Width
    {
        get { return width; }
    }

    public float Height
    {
        get { return height; }
    }
    public float ItemWidth
    {
        get { return itemWidth; }
    }

    public bool IsMovingItem
    {
        get { return (isDragingItem || isMovingItem || isMergingItem); }
    }

    private bool isMergingItem
    {
        get
        {
            if (mergingItem != null)
            {
                return mergingItem.isMerging;
            }
            return false;
        }
    }
    // Start is called before the first frame update

    private void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        
        aspectRatio = Screen.width * 1.0f / Screen.height;
        cameraHeight = Camera.main.orthographicSize * 2;
        cameraWidth = cameraHeight * aspectRatio;


        margin = 0.2f;
        itemSpace = 0.05f;
        itemWidth = (cameraWidth - itemSpace * 7 - margin * 2) / 6; //(cameraWidth - (margin * 2) - itemSpace * 7) / 6;

        Vector2 rect = GetWorldPosition(column - 1, 0);
        width = rect.x * 2 + itemWidth;
        height = rect.y * 2 + itemWidth;
        CreateBackground();
    }

    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    void CreateBackground()
    {
        for (int col = 0; col < column; col++)
        {
            for (int r = 0; r < row; r++)
            {
                Vector2 pos = GetWorldPosition(col, r);
                GameObject back = Instantiate(backgroundPrefab, pos, Quaternion.identity);
                back.GetComponent<SpriteRenderer>().size = new Vector2(itemWidth, itemWidth);
                back.GetComponent<Index>().X = col;
                back.GetComponent<Index>().Y = r;
                back.transform.parent = transform;
                back.transform.localPosition = pos;
            }
        }
    }

    public void Refresh()
    {
        if (items != null)
        {
            foreach (Item item in items)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
        }
        
        items = new Item[column, row];
        //GenerateNewLine();
        //GenerateNewLine();
    }

    public Vector2 GetWorldPosition(int _x, int _y)
    {
        float x = -cameraWidth / 2.0f + itemSpace * (_x + 1) + itemWidth * _x + itemWidth / 2 + margin;
        float y = ((itemSpace * (row / 2.0f - _y + 1)) + (itemWidth * (row / 2 - _y))) - itemWidth / 2;
        return new Vector2(x, y);
    }

    public void GenerateNewLine()
    {
        Item[] newItems = new Item[column];
        for (int i = 0; i < column; i ++)
        {
            int randomNum = Random.Range(1, 5);
            while (items != null && items[i,row - 1] != null && randomNum == items[i, row - 1].Num) { randomNum = Random.Range(1, 5); }
            Vector2 pos = GetWorldPosition(i, row);
            Item item = Instantiate(itemPrefab, pos, Quaternion.identity).GetComponent<Item>();
            item.GetComponent<SpriteRenderer>().size = new Vector2(itemWidth, itemWidth);
            item.transform.parent = transform;
            item.transform.localPosition = pos;
            item.Init(i, row, this, randomNum);
            newItems[i] = item;
        }
        MoveNewLineUp(newItems);
    }

    private void MoveNewLineUp(Item[] newItems)
    {
        for(int x = 0; x < column; x ++)
        {
            for (int y = 0; y < row; y ++)
            {
                Item item = items[x, y];
                if (item != null)
                {
                    if (y == 0)
                    {
                        gameController.GameOver();
                        return;
                    }
                    item.Move(x, y - 1, moveUpTime, false);
                    items[x, y - 1] = item;
                }
                if (y == row - 1)
                {
                    newItems[x].Move(x, y, moveUpTime, false);
                    items[x, y] = newItems[x];
                }
            }
        }
    }

    private void Fall(int preX, int preY, int newX)
    {
        StartCoroutine(MoveDown(newX));
        if (preX != newX)
        {
            if (preY > 0 && items[preX, preY - 1] != null)
            {
                items[preX, preY - 1].canMerge = true;
            }
            StartCoroutine(MoveDown(preX));
        }
    }

    private bool MoveDownStep(int onX)
    {
        bool isMoved = false;
        for (int y = 0; y < row - 1; y++)
        {
            Item item = items[onX, y];
            Item itemBelow = items[onX, y + 1];
            if (item != null && !item.isMerging)
            {
                if (itemBelow == null)
                {
                    item.Move(onX, y + 1, moveUpTime, false);
                    items[onX, y + 1] = item;
                    items[onX, y] = null;
                    isMoved = true;
                }
                else if(itemBelow.isMerging)
                {
                    
                }else if (item.canMerge)
                {
                    if(item.Num == itemBelow.Num)
                    {
                        Merge(item, itemBelow);
                        itemBelow.canMerge = true;
                    }
                    else
                    {
                        item.canMerge = false;
                    }
                    
                }
            }
        }
        return isMoved;
    }

    //private bool MergeStep(int onX)
    //{
    //    //bool isMerged = false;
    //    for (int y = 0; y < row - 1; y++)
    //    {
    //        Item item = items[onX, y];
    //        Item itemBelow = items[onX, y + 1];
    //        if (item != null && item.canMerge)
    //        {
    //            if (itemBelow != null && item.Num == itemBelow.Num)
    //            {
    //                Merge(item, itemBelow);
    //                itemBelow.canMerge = true;
    //                //isMerged = true;
    //                return true;
    //            }
    //            else
    //            {
    //                item.canMerge = false;
    //            }
    //        }
    //    }
    //    return false;
    //}

    private void Merge(Item item, Item toItem)
    {
        gameController.Score++;
        toItem.Merge();
        mergingItem = toItem;
        toItem.Num++;
        items[item.X, item.Y] = null;
        item.Move(toItem.X, toItem.Y, moveUpTime, true);
    }

    public void SelectItem(Item item)
    {
        isDragingItem = true;
        item.canMerge = true;
    }

    public void ReleaseItem(Item item)
    {
        item.transform.position = mouseOnBack.transform.position;
        Index backIndex = mouseOnBack.GetComponent<Index>();
        if (item.X != backIndex.X || item.Y != backIndex.Y)
        {
            items[backIndex.X, backIndex.Y] = item;
            items[item.X, item.Y] = null;

            int x = item.X;
            int y = item.Y;
            item.X = backIndex.X;
            item.Y = backIndex.Y;
            item.ChangeName();
            Fall(x, y, backIndex.X);
        }
        isDragingItem = false;
    }

    public void DragItem(Item item)
    {
         
        Collider2D[] colliders = Physics2D.OverlapPointAll(item.transform.position);
        if (colliders.Length > 2)
        {
            foreach (Collider2D c in colliders)
            {
                if (c.gameObject.tag.Equals("Item") && c.gameObject.name != item.name)
                {
                    Item onItem = c.gameObject.GetComponent<Item>();
                    if (item.Num == onItem.Num)
                    {
                        int x = item.X;
                        int y = item.Y;
                        
                        Merge(item, onItem);
                        onItem.canMerge = true;
                        Fall(x, y, onItem.X);
                        isDragingItem = false;
                    }
                    else
                    {
                        item.transform.position = mouseOnBack.transform.position;
                    }
                }

            }
        }
        else if(colliders.Length == 2)
        {
            foreach (Collider2D c in colliders)
            {
                if (c.gameObject.tag.Equals("Back"))
                {
                    mouseOnBack = c.gameObject;
                }
            }
                
        }
    }

    //从该位置向下落 以上item不管n
    public IEnumerator MoveDown(int onX)
    {
        //bool waitMerge = true;
        //while (waitMerge)
        //{
        //    isMovingItem = true;
        //    yield return new WaitForSeconds(moveUpTime);
        //    while (MoveDownStep(onX))
        //    {
        //        yield return new WaitForSeconds(moveUpTime);
        //    }
        //    waitMerge = MergeStep(onX);
        //}
        //isMovingItem = false;
        isMovingItem = true;
        while (MoveDownStep(onX))
        {
            yield return new WaitForSeconds(moveUpTime);
        }
        isMovingItem = false;
    }

}
