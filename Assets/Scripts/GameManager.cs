using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexMapToolsExamples;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public int turnNum = 0;
    public Text seasonDisplay;
    public int turnsPerSeason = 1;
    public int startYear = 2010;
    private Troop[] troopArray;
    private TurnDisplay turnDisplay;
    private HexControls hexControls;
    private int turnInSeason = 1;
    private int currentYear;
    private int currentSeasonNum = 0;
    private string currentSeason = "Winter";

	// Use this for initialization
	void Start () {
		troopArray = FindObjectsOfType<Troop>();
        turnDisplay = FindObjectOfType<TurnDisplay>();
        hexControls = FindObjectOfType<HexControls>();
        currentYear = startYear;
        seasonDisplay.text = currentSeason + " " + currentYear;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EndTurn() {
        troopArray = GameObject.FindObjectsOfType<Troop>();

        if (turnNum >= 3) {
            turnNum = 0;
        } else {
            turnNum += 1;
        }

        if (turnNum == 0) {
            turnDisplay.SetTurnDisplay("");
            hexControls.ChangePlayer(CellColor.White);
            ActionTurn();
            SeasonCounter();
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

        foreach (Troop thisTroop in troopArray) {
            thisTroop.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, 0));
            thisTroop.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));
            thisTroop.firstPass = true;
            if (thisTroop.currentPos != thisTroop.transform.position) {
                thisTroop.newPos = thisTroop.transform.position;
                thisTroop.transform.position = thisTroop.currentPos;

                thisTroop.animationPath = hexControls.FindPath(thisTroop.currentPos, thisTroop.newPos);

                if (turnNum == 0) {
                    thisTroop.newPos = thisTroop.currentPos;
                }
            }
        }
    }


    public void ActionTurn() {
        foreach (Troop thisTroop in troopArray) {
            Cell thisCell = thisTroop.GetComponentInParent<Cell>();
            foreach (Troop thatTroop in troopArray) {
                Cell thatCell = thatTroop.GetComponentInParent<Cell>();
                if (thisTroop != thatTroop && (thisTroop.newPos == thatTroop.newPos || (thisTroop.newPos == thatTroop.currentPos && thatTroop.newPos == thisTroop.currentPos))) {
                    if (thisTroop.actionPower == thatTroop.actionPower) {
                        thisTroop.newPos = thisTroop.currentPos;
                        thatTroop.newPos = thatTroop.currentPos;
                        foreach (Troop movedTroop in troopArray) {
                            if (movedTroop.newPos == thisTroop.currentPos) {
                                movedTroop.newPos = movedTroop.currentPos;
                            }
                        }
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
            thisTroop.actionPower = thisTroop.basePower;
            thisTroop.assistingTroop = null;
            thisTroop.assistedTroops.Clear();
            hexControls.MoveTroop(thisTroop);
            thisTroop.currentPos = thisTroop.newPos;
        }
    }

    void SeasonCounter() {
        if (currentSeasonNum == 3) {
            currentSeasonNum = 0;
        } else {
            currentSeasonNum++;
        }

        if (currentSeasonNum == 0) {
            currentSeason = "Winter";
            currentYear++;
        } else if (currentSeasonNum == 1) {
            currentSeason = "Spring";
        } else if (currentSeasonNum == 2) {
            currentSeason = "Summer";
        } else if (currentSeasonNum == 3) {
            currentSeason = "Autumn";
        }  

        seasonDisplay.text = currentSeason + " " + currentYear;
    }

}
