﻿using System;
using UnityEngine;
using System.Collections;

public class MouseControlledBox : MonoBehaviour
{

    private int _mouseId = 0;

    private ManyMouse _mouse; //This is what you'll grab input from.

	public void Init (int mouseID)
	{
	    _mouseId = mouseID;
        if (ManyMouseWrapper.MouseCount > _mouseId)
        {
            _mouse = ManyMouseWrapper.GetMouseByID( _mouseId );
            Debug.Log(gameObject.name + " connected to mouse: " + _mouse.DeviceName);
          
            //List to mouse button events
            _mouse.EventButtonDown += EventButtonDown;
        }
        else
        {
            Debug.Log("Mouse ID " + _mouseId + " not found. Plug in an extra mouse?");
            Destroy(gameObject);
        }
	}

    private void EventButtonDown(ManyMouse mouse, int buttonId)
    {
        //Manual "leave game"
        if(buttonId == 1)
        {
            _mouse.EventButtonDown -= EventButtonDown;
            //destroy self
            Destroy(gameObject);
        }
    }

	void Update ()
	{
	    Vector3 delta = _mouse.Delta;
        transform.position += delta;
        Debug.Log(_mouse.Pos);
	}
}
