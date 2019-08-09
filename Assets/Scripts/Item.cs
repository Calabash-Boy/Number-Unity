﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{

    
    private int num;
    private Grid grid;
    private Index gridIndex;
    private TextMeshProUGUI numText;
    private Canvas canvas;
    private SpriteRenderer renderer;

    private bool isMovable;

    private IEnumerator moveCoroutine;

    [System.NonSerialized] public bool canMerge;
    [System.NonSerialized] public bool isMerging;
    public AnimationClip mergeAnimation;

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

    public int X
    {
        get { return gridIndex.X; }
        set { gridIndex.X = value; }
    }

    public int Y
    {
        get { return gridIndex.Y; }
        set { gridIndex.Y = value; }
    }

    private void Awake()
    {
        gridIndex = GetComponent<Index>();
        canvas = transform.Find("Canvas").GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        numText = canvas.transform.Find("NumText").GetComponent<TextMeshProUGUI>();
        numText.transform.position = transform.position;
        renderer = GetComponent<SpriteRenderer>();
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

        GridRef.SelectItem(this);
        isMovable = true;
        renderer.sortingOrder = 99;
        canvas.sortingOrder = 99;
    }

    private void OnMouseUp()
    {
        isMovable = false;
        renderer.sortingOrder = 1;
        canvas.sortingOrder = 1;
        GridRef.ReleaseItem(this);
    }

    private void OnMouseDrag()
    {
        if (isMovable && !isMerging)
        {
            GridRef.DragItem(this);
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

    

    private IEnumerator MoveCoroutine(int newX, int newY, float time, bool merge)
    {
        gridIndex.X = newX;
        gridIndex.Y = newY;
        gridIndex.ChangeName();

        Vector3 startPos = transform.localPosition;
        Vector3 endPos = GridRef.GetWorldPosition(newX, newY);
        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            transform.localPosition = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }
        transform.localPosition = endPos;
        if (merge)
        {
            Destroy(transform.gameObject);
        }
    }

    private IEnumerator MergeCoroutine()
    {
        isMerging = true;
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play(mergeAnimation.name);
            yield return new WaitForSeconds(mergeAnimation.length);
            isMerging = false;
        }
        isMerging = false;
    }
    
    public void Init(int _x, int _y, Grid _grid, int _num)
    {
        gridIndex.X = _x;
        gridIndex.Y = _y;
        grid = _grid;
        Num = _num;
        gridIndex.ChangeName();
    }


    public void Move(int newX, int newY, float time, bool merge)
    {
        if (merge)
        {
            isMerging = true;
            renderer.sortingOrder = 1;
            canvas.sortingOrder = 1;
        }
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MoveCoroutine(newX, newY, time, merge);
        StartCoroutine(moveCoroutine);
    }

    public void ChangeName()
    {
        gridIndex.ChangeName();
    }

    public void Merge()
    {
        StartCoroutine(MergeCoroutine());
    }
}
