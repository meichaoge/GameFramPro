using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class sdsda : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public BoxCollider2D box1;
    public BoxCollider2D box2;
    public BoxCollider2D box3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        //    if (hit.collider != null)
        //    {
        //        Debug.Log("clicked object name is ---->" + hit.collider.gameObject);
        //    }
        //    //JewelTouchChecker(Input.mousePosition);
        //    //RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        //    //if (hit.collider != null)
        //    //{
        //    //    Debug.Log(hit.collider.name);
        //    //}
        //}
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    box1.size = new Vector2(100, 100);
        //    box2.size = new Vector2(200, 200);
        //    box3.size = new Vector2(200, 200);
        //}
    }


    GameObject JewelTouchChecker(Vector3 mouseposition)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(mouseposition);
        Vector3 touchPos = new Vector3(wp.x, wp.y);
        Collider2D[] collider2Ds = Physics2D.OverlapPointAll(touchPos);
        for (int i = 0; i < collider2Ds.Length; i++)
        {
            Debug.Log(collider2Ds[i].name);
        }
        //if (Physics2D.OverlapPoint(touchPos))
        //{
        //    return Physics2D.OverlapPoint(touchPos).gameObject;
        //}
        return null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"OnBeginDrag {eventData.hovered[0]} ");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log($"OnDrag {eventData.hovered[0]} ");

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"OnDrag {eventData.hovered[0]} ");

    }
}
