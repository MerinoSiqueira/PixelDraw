﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawRealtimeTest : MonoBehaviour
{

    public int sizeX = 32, sizeY = 32;
    public GameObject plane;
    public Camera cam;

    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        sizeY = sizeX;
        CreatePixelGrid();
    }

    void CreatePixelGrid()
    {
        Texture2D pixelTexture = new Texture2D(sizeX, sizeY, TextureFormat.ARGB32, false);
        pixelTexture.filterMode = FilterMode.Point;
        Paint(pixelTexture);
    }

    void Paint(Texture2D pixelTexture)
    {
        Color32 colorGreen = Color.green;
        Color32 colorBlack = Color.black;
        int count = 0;
        int center = CalculateCenter(sizeX, sizeY);
        Debug.Log("Center = " + center);
        for (int i = 0; i < sizeY; i++)
        {
            for (int j = 0; j < sizeX; j++)
            {
                count++;
                if (count == Input.mousePosition.x)
                {
                    pixelTexture.SetPixel(i, j, colorBlack);
                }
                else
                {
                    pixelTexture.SetPixel(i, j, colorGreen);
                }
            }
        }
        pixelTexture.Apply();
        plane.GetComponent<RawImage>().texture = pixelTexture;
    }

    int CalculateCenter(int sizeX, int sizeY)
    {
        if ((sizeX % 2) == 0 && (sizeY % 2) == 0)
        {
            return ((sizeX * sizeY) / 2) - (sizeX / 2);
        }
        if ((sizeX % 2) == 1 || (sizeY % 2) == 1)
        {
            if ((sizeX % 2) == 0 || (sizeY % 2) == 0)
            {
                if (sizeY > sizeX)
                {
                    return (sizeX * sizeY) / 2;
                }
                else
                {
                    return ((sizeX * sizeY) / 2) - (sizeY / 2);
                }
            }
        }
        if ((sizeX % 2) == 1 && (sizeY % 2) == 1)
        {
            return ((sizeX * sizeY) / 2) + 1;
        }
        return 0;
    }
}

