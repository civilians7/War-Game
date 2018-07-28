using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOneTroop : MonoBehaviour {

    SpriteRenderer sprite;
    private PlayerOneTroop[] troopArray;

    // Use this for initialization
    void Start () {
        troopArray = GameObject.FindObjectsOfType<PlayerOneTroop>();
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.cyan;
        GetComponent<PolygonCollider2D>().enabled = false;
    }
	

    void OnMouseDown() {
        troopArray = GameObject.FindObjectsOfType<PlayerOneTroop>();
        foreach (PlayerOneTroop thisTroop in troopArray) {
            thisTroop.sprite.color = Color.cyan;
        }
        sprite.color = Color.yellow;

    }

    public void EnemyTurn() {
        troopArray = GameObject.FindObjectsOfType<PlayerOneTroop>();
        foreach (PlayerOneTroop thisTroop in troopArray) {
            thisTroop.GetComponent<PolygonCollider2D>().enabled = false;
            thisTroop.sprite.color = Color.cyan;
            if (thisTroop.transform.childCount >= 1) {
                Destroy(thisTroop.transform.GetChild(0).gameObject);
            }
        }
    }

    public void MyTurn() {
        troopArray = GameObject.FindObjectsOfType<PlayerOneTroop>();
        foreach (PlayerOneTroop thisTroop in troopArray) {
            thisTroop.GetComponent<PolygonCollider2D>().enabled = true;
        }
    }
}
