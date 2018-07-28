using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour {

    public int turnNum = 0;
    private TurnDisplay turnDisplay;
    private PlayerOneTroop oneTroop;
    private PlayerTwoTroop twoTroop;
    private Troop troop;

    void Start() {
        oneTroop = GameObject.FindObjectOfType<PlayerOneTroop>();
        twoTroop = GameObject.FindObjectOfType<PlayerTwoTroop>();
        troop = GameObject.FindObjectOfType<Troop>();
        turnDisplay = GameObject.FindObjectOfType<TurnDisplay>();
    }

    public void EndTurn() {
        Troop.selectedTroop = null;
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
            PlayerOneTurn();
            
        } else if (turnNum == 2) {
            turnDisplay.SetTurnDisplay("Player Two");
            PlayerTwoTurn();
           
        } else if (turnNum == 3) {
            turnDisplay.SetTurnDisplay("Ready?");
            troop.EndTurn();
            twoTroop.EnemyTurn();
            oneTroop.EnemyTurn();
        }

    }

    public void PlayerOneTurn() {
        troop.EndTurn();
        twoTroop.EnemyTurn();
        oneTroop.MyTurn();
    }

    public void PlayerTwoTurn() {
        troop.EndTurn();
        oneTroop.EnemyTurn();
        twoTroop.MyTurn();
    }

    public void ActionTurn() {
        twoTroop.EnemyTurn();
        oneTroop.EnemyTurn();
        troop.ActionTurn();
    }
}
