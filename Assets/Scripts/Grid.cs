using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    float cameraHeight;
    float cameraWidth;
    float aspectRatio;
    private float margin;
    private float itemSpace;
    private float itemWidth;

    private float width;
    private float height;

    public int column;
    public int row;
    public GameObject backgroundPrefab;
    public GameObject itemPrefab;

    private Item[,] items;

    private GameObject mouseOnBack;
    private bool isMoving;

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
    // Start is called before the first frame update

    private void Awake()
    {
        items = new Item[column, row];
        aspectRatio = Screen.width * 1.0f / Screen.height;
        cameraHeight = Camera.main.orthographicSize * 2;
        cameraWidth = cameraHeight * aspectRatio;


        margin = 0.2f;
        itemSpace = 0.05f;
        itemWidth = (cameraWidth - itemSpace * 7 - margin * 2) / 6; //(cameraWidth - (margin * 2) - itemSpace * 7) / 6;

        Vector2 rect = GetWorldPosition(column - 1, 0);
        width = rect.x * 2 + itemWidth;
        height = rect.y * 2 + itemWidth;
    }

    void Start()
    {
        
        CreateBackground();
        GenerateNewLine();
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    void CreateBackground()
    {
        
        EdgeCollider2D edge = GetComponent<EdgeCollider2D>();
        Vector2[] points = edge.points;
        for (int col = 0; col < column; col++)
        {
            for (int r = 0; r < row; r++)
            {
                Vector2 pos = GetWorldPosition(col, r);
                GameObject back = Instantiate(backgroundPrefab, pos, Quaternion.identity);
                back.GetComponent<SpriteRenderer>().size = new Vector2(itemWidth, itemWidth);
                back.transform.parent = transform;

                if (col == 0 && r == 0)  //左上
                {
                    pos.x -= itemWidth / 2;
                    pos.y += itemWidth / 2;
                    points[0] = pos;
                    points[4] = pos;
                }
                else if (col == column - 1 && r == 0)      //右上
                {
                    pos.x += itemWidth / 2;
                    pos.y += itemWidth / 2;
                    points[3] = pos;
                }
                else if (col == 0 && r == row - 1)      //左下
                {
                    pos.x -= itemWidth / 2;
                    pos.y -= itemWidth / 2;
                    points[1] = pos;
                }
                else if (col == column - 1 && r == row - 1)     //右下
                {
                    pos.x += itemWidth / 2;
                    pos.y -= itemWidth / 2;
                    points[2] = pos;
                }
            }
        }
        edge.points = points;
       
    }

    Vector2 GetWorldPosition(int _x, int _y)
    {
        float x = -cameraWidth / 2.0f + itemSpace * (_x + 1) + itemWidth * _x + itemWidth / 2 + margin;
        float y = ((itemSpace * (row / 2.0f - _y + 1)) + (itemWidth * (row / 2 - _y))) - itemWidth / 2;
        return new Vector2(x, y);
    }

    void GenerateNewLine()
    {
        for (int i = 0; i < column; i ++)
        {
            Item item = Instantiate(itemPrefab, GetWorldPosition(i, row - 1), Quaternion.identity).GetComponent<Item>();
            item.GetComponent<SpriteRenderer>().size = new Vector2(itemWidth, itemWidth);
            item.transform.parent = transform;
            item.Init(i, 0, this, Random.Range(1, 4));
            items[i, 0] = item;
        }
    }

    public void selectItem(Item item)
    {
        isMoving = true;
    }

    public void releaseItem(Item item)
    {
        item.transform.position = mouseOnBack.transform.position;
        isMoving = false;
    }

    public void moveItem(Item item)
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
                        onItem.Num *= 2;
                        
                        Destroy(item.gameObject);
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

}
