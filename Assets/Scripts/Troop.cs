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

    private Troop[] troopArray;
    private GridSpace gridSpace;

    // Use this for initialization
    void Start() {
        troopArray = GameObject.FindObjectsOfType<Troop>();
        //gridSpace = FindObjectOfType<GridSpace>();
        movement = movement / 2;
        attackDistance = attackDistance / 2;
        currentPos = gameObject.transform.position;
        newPos = currentPos;

    }


    void OnMouseDown() {
        selectedTroop = gameObject;
      /*  if (transform.childCount != 0) {
            Destroy(transform.GetChild(0).gameObject);
        }
        var crosshair = Instantiate(target, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

        crosshair.transform.parent = gameObject.transform;*/
    }

  /*  void OnMouseDrag() {
        if (Troop.selectedTroop) {
            Troop.selectedTroop.transform.GetChild(0).transform.position = gridSpace.CalculateWorldPointOfMouseClick();
        }
    }

    void OnMouseUp() {
        gridSpace.SetTarget();
    }
    */
    public void EndTurn() {
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
