using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public int turnNum = 0;
    private Troop[] troopArray;
    private TurnDisplay turnDisplay;
	// Use this for initialization
	void Start () {
		troopArray = GameObject.FindObjectsOfType<Troop>();
        turnDisplay = GameObject.FindObjectOfType<TurnDisplay>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EndTurn() {
        Troop.selectedTroop = null;
        troopArray[0].NextTurn();
        if (turnNum >= 3) {
            turnNum = 0;
        } else {
            turnNum += 1;
        }

        if (turnNum == 0) {
            turnDisplay.SetTurnDisplay("");
            ActionTurn();
        }else if (turnNum == 1) {
            turnDisplay.SetTurnDisplay("Player One");
            
        } else if (turnNum == 2) {
            turnDisplay.SetTurnDisplay("Player Two");
           
        } else if (turnNum == 3) {
            turnDisplay.SetTurnDisplay("Ready?");
        }

        foreach (Troop thisTroop in troopArray) {
            thisTroop.NextTurn();
            if (thisTroop.playerNum == turnNum) {
                thisTroop.GetComponent<PolygonCollider2D>().enabled = true;
            } else {
                thisTroop.GetComponent<PolygonCollider2D>().enabled = false;
            }
        }

        troopArray = GameObject.FindObjectsOfType<Troop>();
        foreach (Troop thisTroop in troopArray) {
            if (thisTroop.currentPos != thisTroop.transform.position) {
                thisTroop.newPos = thisTroop.transform.position;
                thisTroop.transform.position = thisTroop.currentPos;
            }
        }
    }


    public void ActionTurn() {
        foreach (Troop thisTroop in troopArray) {

            foreach (Troop thatTroop in troopArray) {

                if (thisTroop != thatTroop && thisTroop.newPos == thatTroop.newPos || thisTroop.newPos == thatTroop.currentPos) {

                    if (thisTroop.power == thatTroop.power) {
                        thisTroop.newPos = thisTroop.currentPos;
                        thatTroop.newPos = thatTroop.currentPos;
                    } else if (thisTroop.power < thatTroop.power) {
                        Destroy(thisTroop.gameObject);
                    } else if (thisTroop.power > thatTroop.power) {
                        Destroy(thatTroop.gameObject);
                    }

                } 
            }
            thisTroop.transform.position = thisTroop.newPos;
            thisTroop.currentPos = thisTroop.newPos;
        }
    }

}
