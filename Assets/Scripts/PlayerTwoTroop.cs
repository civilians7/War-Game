using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTwoTroop : MonoBehaviour {

    SpriteRenderer sprite;
    private PlayerTwoTroop[] troopArray;

    // Use this for initialization
    void Start () {
        troopArray = GameObject.FindObjectsOfType<PlayerTwoTroop>();
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.red;
        GetComponent<PolygonCollider2D>().enabled = false;
    }
	


    void OnMouseDown() {
        troopArray = GameObject.FindObjectsOfType<PlayerTwoTroop>();

        foreach (PlayerTwoTroop thisTroop in troopArray) {
            thisTroop.sprite.color = Color.red;
        }
        sprite.color = Color.yellow;
    }

    public void EnemyTurn() {
        troopArray = GameObject.FindObjectsOfType<PlayerTwoTroop>();
        foreach (PlayerTwoTroop thisTroop in troopArray) {
            thisTroop.GetComponent<PolygonCollider2D>().enabled = false;
            thisTroop.sprite.color = Color.red;
            
        }
    }

    public void MyTurn() {
        troopArray = GameObject.FindObjectsOfType<PlayerTwoTroop>();
        foreach (PlayerTwoTroop thisTroop in troopArray) {
            thisTroop.GetComponent<PolygonCollider2D>().enabled = true;
        }
    }

}
