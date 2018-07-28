using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpace : MonoBehaviour {

    public Camera myCamera;
    public GameObject target;

    private void Start() {  
    }

    void OnMouseDown() {
        Vector2 rawPos = CalculateWorldPointOfMouseClick();
        Vector2 roundedPos = SnapToGrid(rawPos);

        if (Troop.selectedTroop) { 
            Vector2 currentPos = Troop.selectedTroop.GetComponent<Troop>().currentPos;
            if (Troop.selectedTroop.transform.childCount >= 1) {
                Destroy(Troop.selectedTroop.transform.GetChild(0).gameObject);
                Troop.selectedTroop.GetComponent<Troop>().targetPos = roundedPos;
            }
            if (Attack.isMoving) {
                Moving(roundedPos, currentPos);
            } else {
                Attacking(roundedPos, currentPos);
            }
        }
    }

   /* void OnMouseDrag() {
        if (Troop.selectedTroop) {
            Troop.selectedTroop.transform.GetChild(0).transform.position = CalculateWorldPointOfMouseClick();
        }
    }*/

  /*  public void SetTarget() {
        if (Troop.selectedTroop) {
            Vector2 rawPos = Troop.selectedTroop.transform.GetChild(0).transform.position;
            Vector2 roundedPos = SnapToGrid(rawPos);
            float attackDist = Troop.selectedTroop.GetComponent<Troop>().attackDistance;

            if (CanAttack(roundedPos, Troop.selectedTroop.transform.position, attackDist)) {
                Troop.selectedTroop.transform.GetChild(0).transform.position = roundedPos;
            } else {
                Destroy(Troop.selectedTroop.transform.GetChild(0).gameObject);
            }
        }
    }*/

    Vector2 SnapToGrid(Vector2 rawWorldPos) {
        float xPos;
        float yPos;

        if (Mathf.RoundToInt(rawWorldPos.x) - rawWorldPos.x > .25) {
            xPos = Mathf.RoundToInt(rawWorldPos.x) - .5f;
        } else if (Mathf.RoundToInt(rawWorldPos.x) - rawWorldPos.x < -.25) {
            xPos = Mathf.RoundToInt(rawWorldPos.x) + .5f;
        } else {
            xPos = Mathf.RoundToInt(rawWorldPos.x);
        }

        if (Mathf.RoundToInt(rawWorldPos.y) - rawWorldPos.y > .25) {
            yPos = Mathf.RoundToInt(rawWorldPos.y) - .5f;
            if (rawWorldPos.x < xPos) {
                xPos -= .25f;
            } else {
                xPos += .25f;
            }
        } else if (Mathf.RoundToInt(rawWorldPos.y) - rawWorldPos.y < -.25) {
            yPos = Mathf.RoundToInt(rawWorldPos.y) + .5f;
            if (rawWorldPos.x < xPos) {
                xPos -= .25f;
            } else {
                xPos += .25f;
            }
        } else {
            yPos = Mathf.RoundToInt(rawWorldPos.y);
        }

        return new Vector2(xPos, yPos);
    }

    public Vector2 CalculateWorldPointOfMouseClick() {
        float xPos = Input.mousePosition.x;
        float yPos = Input.mousePosition.y;
        float distanceFromCamera = 10f;

        Vector3 weirdTriplet = new Vector3(xPos, yPos, distanceFromCamera);
        Vector2 worldPos = myCamera.ScreenToWorldPoint(weirdTriplet);

        return worldPos;
    }

    public bool CanMove(Vector2 targetPos, Vector2 curPos, float movement) {
        return (targetPos.x - curPos.x <= movement && curPos.x - targetPos.x <= movement && targetPos.y - curPos.y <= movement && curPos.y - targetPos.y <= movement);

    }

    private bool CanAttack(Vector2 targetPos, Vector2 curPos, float attackDist) {
        return (targetPos.x - curPos.x <= attackDist && curPos.x - targetPos.x <= attackDist && targetPos.y - curPos.y <= attackDist && curPos.y - targetPos.y <= attackDist);

    }

    //sets the space the troop is targetting with an attack
    public void Attacking(Vector3 roundedPos, Vector2 currentPos) {

        float attackDist = Troop.selectedTroop.GetComponent<Troop>().attackDistance;
        if (CanAttack(roundedPos, Troop.selectedTroop.transform.position, attackDist)) {
            Troop.selectedTroop.GetComponent<Troop>().targetPos = roundedPos;
            GameObject crosshair = Instantiate(target, roundedPos, Quaternion.identity);
            crosshair.transform.parent = Troop.selectedTroop.transform;

        }
            
    }

    //Sets the position of the troop
    public void Moving(Vector2 roundedPos, Vector2 currentPos) {

        float movement = Troop.selectedTroop.GetComponent<Troop>().movement;
        if (CanMove(roundedPos, currentPos, movement)) {
            Troop.selectedTroop.transform.position = roundedPos;
        }

    }
}
