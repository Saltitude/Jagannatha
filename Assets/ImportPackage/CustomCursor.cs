using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    Vector2 targetPos;


    public Texture2D cursorTexture;
    public Texture2D cursorTextureClic;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;


    // Start is called before the first frame update
    void Start()
    {
        //Cursor.visible = false;
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    // Update is called once per frame
    void Update()
    {
        //targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //transform.position = targetPos;  


        if (Input.GetMouseButtonDown(0))
            Cursor.SetCursor(cursorTextureClic, hotSpot, cursorMode);

        if (Input.GetMouseButtonUp(0))
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);




    }



}
