using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{

    private int x;
    private int y;
    private int num;
    private Grid grid;
    private TextMeshProUGUI numText;


    private Vector3 touchBeginPos;
    private Vector3 touchLastPos;
    private bool isMovable;

    public int X
    {
        get { return x;  }
        set
        {
            x = value;
            ChangeName();
        }
    }

    public int Y
    {
        get { return y; }
        set
        {
            y = value;
            ChangeName();
        }
    }

    //元素数字
    public int Num
    {
       get { return num;  }
       set
        {
            num = value;
            this.numText.text = num.ToString();
            SetColor();
        }
    }

    public Grid GridRef
    {
        get { return grid; }
    }

    private void Awake()
    { 
        Canvas canvas = transform.Find("Canvas").GetComponent<Canvas>();
        canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        numText = canvas.transform.Find("NumText").GetComponent<TextMeshProUGUI>();
        numText.transform.position = transform.position;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        isMovable = false;

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnMouseDown()
    {
        
        grid.selectItem(this);
        isMovable = true;
        touchLastPos = Input.mousePosition;
        GetComponent<SpriteRenderer>().sortingOrder = 99;
        transform.Find("Canvas").GetComponent<Canvas>().sortingOrder = 99;
        //transform.position = Camera.main.ScreenToWorldPoint(touchLastPos);
    }

    private void OnMouseUp()
    {
        isMovable = false;
        GetComponent<SpriteRenderer>().sortingOrder = 1;
        transform.Find("Canvas").GetComponent<Canvas>().sortingOrder = 1;
        grid.releaseItem(this);
    }

    private void OnMouseDrag()
    {
        if (isMovable)
        {
            grid.moveItem(this);
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPos = new Vector2(Mathf.Clamp(newPos.x, (-grid.Width + grid.ItemWidth) / 2.0f, (grid.Width - grid.ItemWidth) / 2.0f),
                                 Mathf.Clamp(newPos.y, (-grid.Height + grid.ItemWidth) / 2.0f, (grid.Height - grid.ItemWidth) / 2.0f));
            transform.position = newPos;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("OnCollisionEnter2D`" + name);
    }

    private void SetColor()
    {
        float r = num % 2;
        float g = (0.1f * (num / 10) + 0.1f * (num % 10)) / 2.0f;
        float b = g * 4;
        GetComponent<SpriteRenderer>().color = new Color(r, g, b);
    }

    private void ChangeName()
    {
        name = "Item(" + y + "," + x + ")";
    }
    
    public void Init(int _x, int _y, Grid _grid, int _num)
    {
        x = _x;
        y = _y;
        grid = _grid;
        Num = _num;
        ChangeName();
    }

}
