using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexMapToolsExamples;
using HexMapTools;

public class Troop : MonoBehaviour {

    public int movement;
    public int basePower;
    public int actionPower;
    public float attackDistance;
    public Vector3 currentPos;
    public Vector3 newPos;
    public CellColor color;
    public Vector3 direction;

    public List<HexCoordinates> animationPath;

    public List<Troop> assistedTroops = new List<Troop>();
    public Troop assistingTroop;
    
    private HexControls hexControls;
    private GameManager gameManager;
    public bool firstPass = true;
    private bool moving = true;
    // Use this for initialization
    void Start() {
        currentPos = transform.position;
        newPos = currentPos;
        assistedTroops.Add(this);
        color = GetComponentInParent<Cell>().Color;
        hexControls = FindObjectOfType<HexControls>();
        gameManager = FindObjectOfType<GameManager>();

    }
    void Update() {
        //if (gameManager.turnNum == 0) {
        //    float newPosPoint = (newPos.x * direction.x);
        //    float transformPoint = (transform.position.x * direction.x);
            
        //    if (firstPass) {
        //        moving = newPosPoint > transformPoint;
        //        firstPass = false;
        //    }
            
        //    if (direction != new Vector3(0,0,0) && moving == (newPosPoint > transformPoint)) {
        //        transform.Translate(direction);
        //    } else {
        //        hexControls.SetTroopParent(this);
        //    }
        //}
    }

    public void Engage(Troop troop) {
        if (troop.GetComponentInParent<Cell>().Color == this.GetComponentInParent<Cell>().Color) {
            AssistedBy(troop);
        } 
    }

    void AssistedBy(Troop ally) {

        foreach (Troop troop in assistedTroops) {
            if (ally == troop || ally.assistingTroop != null) {
                return;
            }
        }
        foreach (Troop troop in ally.assistedTroops) {
            if (this == troop) {
                return;
            }
        }
        ally.assistingTroop = this;
        assistedTroops.Add(ally);

        GetComponent<LineRenderer>().SetPosition(0, transform.position);
        GetComponent<LineRenderer>().SetPosition(1, ally.transform.position);
        
        actionPower += ally.basePower;
    }

    public void Move() {
        GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, 0));
        GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));
        foreach (Troop troop in assistedTroops) {
            troop.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, 0));
            troop.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));
        }
        actionPower = basePower;
        assistedTroops.Clear();
        if (assistingTroop != null) {
            assistingTroop.assistedTroops.Remove(this);
        }

    }
}


