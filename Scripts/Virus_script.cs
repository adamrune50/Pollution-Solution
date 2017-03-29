﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Virus_script : MonoBehaviour
{
    public Transform target;

    private bool change = true;
    private int updateTicks = 0;
    private float releaseTime = 1.0f;
    private float y = 1;
    private int width  = 1920;
    private int height = 1080;
    private Vector3 dragStart;
    private Vector3 dragEnd;
    private Vector3 direction;
    private Vector3 spriteMouseSeperation;
    private Vector3 mousePos;
    private float mouseDirection;
    private bool isThrown = false;

    private float speed = 100;
    private float ySpeed = 25;

    private int id = 1;

    private Vector3 movementDirection;
    private Vector3 normal;

    void Spawn()
    {
        System.Random rnd = new System.Random(DateTime.Now.Second * id);
        int halfWidth = width / 2;
        int halfHeight = height / 2;

        int side = rnd.Next(0, 4);

        int startX = 0;
        int startY = 0;

        switch (side)
        {
            case 0:
                startX = rnd.Next(0, width) - halfWidth;
                startY = halfHeight;
                break;
            case 1:
                startX = halfWidth;
                startY = rnd.Next(0, height) - halfHeight;
                break;
            case 2:
                startX = rnd.Next(0, width) - halfWidth;
                startY = -halfHeight;
                break;
            case 3:
                startX = -halfWidth;
                startY = rnd.Next(0, height) - halfHeight;
                break;
        }

        transform.position = new Vector2(startX, startY);
    }

    void Start()
    {
        Spawn();
    }

    void Update()
    {
        if (isThrown)
        {
            transform.Translate(direction * mouseDirection * 10 * Time.deltaTime); // Changed from force to translate
            // Using distance (mouse travel) as speed multiplier
        }
    }

    void OnMouseDown()
    {
        mousePos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y); // Position of mouse on screen (no z as messes up movement)
        spriteMouseSeperation = mousePos - transform.position;
        transform.position = mousePos - spriteMouseSeperation;
        dragStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        StartCoroutine(Release());
    }

    void OnMouseUp()
    {
        if (!isThrown)
        {
            isThrown = true;
            dragEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = dragEnd - dragStart;
            mouseDirection = direction.magnitude;
            direction.Normalize();
        }
    }

    IEnumerator Release()
    {
        yield return new WaitForSeconds(releaseTime);
        OnMouseUp();
    }

    Vector3 GetDirection()
    {
        Vector3 targetPos = target.position;
        Vector3 objPos = transform.position;

        Vector3 direction = targetPos - objPos;

        return direction.normalized;
    }

    Vector3 GetNormal(Vector3 v)
    {
        return new Vector3(-v.y / Mathf.Sqrt(Mathf.Pow(v.x, 2) + Mathf.Pow(v.y, 2)), v.x) / Mathf.Sqrt(Mathf.Pow(v.x, 2) + Mathf.Pow(v.y, 2));
    }

    void FixedUpdate()
    {
        updateTicks++;
        transform.Translate((movementDirection * speed) * Time.deltaTime);

        movementDirection = GetDirection();
        normal = GetNormal(movementDirection);

        y = normal.y * Mathf.Sin(updateTicks * (Mathf.PI / 180));
        transform.Translate(normal.x, y * ySpeed * Time.deltaTime, 0);

        if (change)
        {
            StartCoroutine(ChangeDirection());
            change = false;
        }
    }

    IEnumerator ChangeDirection()
    {
        yield return new WaitForSeconds(releaseTime);
        y *= -1;
        change = true;
    }
}
