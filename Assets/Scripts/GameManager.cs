using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexMapToolsExamples;

public class GameManager : MonoBehaviour {

    public int turnNum = 0;
    private Troop[] troopArray;
    private TurnDisplay turnDisplay;
    private HexControls hexControls;
	// Use this for initialization
	void Start () {
		troopArray = FindObjectsOfType<Troop>();
        turnDisplay = FindObjectOfType<TurnDisplay>();
        hexControls = FindObjectOfType<HexControls>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EndTurn() {
        if (turnNum >= 3) {
            turnNum = 0;
        } else {
            turnNum += 1;
        }

        if (turnNum == 0) {
            turnDisplay.SetTurnDisplay("");
            hexControls.ChangePlayer(CellColor.White);
            ActionTurn();
        }else if (turnNum == 1) {
            turnDisplay.SetTurnDisplay("Player One");
            hexControls.ChangePlayer(CellColor.Blue);
        } else if (turnNum == 2) {
            turnDisplay.SetTurnDisplay("Player Two");
            hexControls.ChangePlayer(CellColor.Red);
           
        } else if (turnNum == 3) {
            turnDisplay.SetTurnDisplay("Ready?");
            hexControls.ChangePlayer(CellColor.White);
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
            Cell thisCell = thisTroop.GetComponentInParent<Cell>();
            foreach (Troop thatTroop in troopArray) {
                Cell thatCell = thatTroop.GetComponentInParent<Cell>();
                if ((thisTroop != thatTroop && thisTroop.newPos == thatTroop.newPos) || (thisTroop.newPos == thatTroop.currentPos && thatTroop.newPos == thisTroop.currentPos)) {
                    
                    if (thisTroop.actionPower == thatTroop.actionPower) {
                        thisTroop.newPos = thisTroop.currentPos;
                        thatTroop.newPos = thatTroop.currentPos;
                    } else if (thisTroop.actionPower < thatTroop.actionPower) { //this troop loses to that cell
                        thatCell.Color = CellColor.White;
                        thatTroop.transform.SetParent(thisCell.transform);
                        Destroy(thisTroop.gameObject);
                    } else if (thisTroop.actionPower > thatTroop.actionPower) { //this troop beats that troop
                        thisCell.Color = CellColor.White;
                        thisTroop.transform.SetParent(thatCell.transform);
                        Destroy(thatTroop.gameObject);
                    }

                } 
            }
            //thisCell.Color = thisTroop.color;
            thisTroop.actionPower = thisTroop.basePower;
            hexControls.moveTroop(thisTroop);
            //thisTroop.transform.SetParent cells.newPos;
            thisTroop.currentPos = thisTroop.newPos;
        }
    }

}
