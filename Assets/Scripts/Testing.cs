using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Testing : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    private Player player;
    void Start()
    {
        playerPrefab = GameObject.FindGameObjectWithTag("Player");
        player = playerPrefab.GetComponent<Player>();
        ChordLibrary.GenerateChordLibrary();
    }
    void Update()
    {
        
    }
    public void Test()
    {
        Debug.Log("Testing");
    }
    public void AddABit()
    {
        Debug.Log("Added 25XP");
        player.LevelUp(25);
    }
    public void AddALot()
    {
        Debug.Log("Added 150XP");
        player.LevelUp(150);
    }
    public void AddATiny()
    {
        Debug.Log("Added 5XP");
        player.LevelUp(5);
    }
}
