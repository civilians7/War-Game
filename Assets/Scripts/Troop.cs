using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Troop : MonoBehaviour {


    public static GameObject selectedTroop;
    public float movement;
    public int power;
    public float attackDistance;
    public Vector3 currentPos;
    public Vector3 newPos;
    public Vector3 targetPos;
    public int playerNum;

    private Troop[] troopArray;
    private SpriteRenderer sprite;
    private Color spriteColor;
    // Use this for initialization
    void Start() {
        currentPos = gameObject.transform.position;
        newPos = currentPos;
        sprite = GetComponent<SpriteRenderer>();
        spriteColor = sprite.color;
        GetComponent<PolygonCollider2D>().enabled = false;
        troopArray = GameObject.FindObjectsOfType<Troop>();
    }


    void OnMouseDown() {
        selectedTroop = gameObject;

        troopArray = GameObject.FindObjectsOfType<Troop>();
        foreach (Troop thisTroop in troopArray) {
            if (thisTroop.playerNum == playerNum) {
                thisTroop.sprite.color = spriteColor;
            }
        }
        sprite.color = Color.yellow;
    }

    public void NextTurn() {
    sprite.color = spriteColor;
    }


}


