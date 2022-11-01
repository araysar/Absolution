using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGames : MonoBehaviour
{
    public string[] games;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            print(games[Random.Range(0, games.Length)]);
        }
    }
}
