using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexMapToolsExamples;

public class Troop : MonoBehaviour {

    public int movement;
    public int basePower;
    public int actionPower;
    public float attackDistance;
    public Vector3 currentPos;
    public Vector3 newPos;
    public Vector3 targetPos;
    public CellColor color;

    public List<Troop> assistedTroops = new List<Troop>();
    public Troop assistingTroop;

    // Use this for initialization
    void Start() {
        currentPos = gameObject.transform.position;
        newPos = currentPos;
        assistedTroops.Add(this);
        color = GetComponentInParent<Cell>().Color;

    }
    void Update() {
        //currentPos = transform.position;
    }

    public void Engage(Troop troop) {
        if (troop.GetComponentInParent<Cell>().Color == this.GetComponentInParent<Cell>().Color) {
            AssistedBy(troop);
        } 
    }

    void AssistedBy(Troop ally) {
        foreach (Troop troop in assistedTroops) {
            if (ally == troop) {
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
        
        actionPower += ally.basePower;
    }

    public void Move() {
        actionPower = basePower;
        assistedTroops.Clear();
        if (assistingTroop != null) {
            assistingTroop.assistedTroops.Remove(this);
        }
        
    }
}


