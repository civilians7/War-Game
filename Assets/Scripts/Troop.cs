using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexMapTerrain;
using HexMapTools;

public enum CellColor { White = 0, Blue, Red, Purple, Orange, Yellow, Brown, Green }

public class Troop : MonoBehaviour {

    //Comments should describe "Why" | "What" should be clear from the code

    //game variables
    public int movement; //keep public
    public int basePower; //keep public
    public int actionPower; //make private
    public float attackDistance; //keep public

    //location and movement
    public Vector3 currentPos;
    public Vector3 newPos;
    public CellColor color = CellColor.Blue;
    public Vector3 direction = new Vector3(0,0,0);
    public bool movingRight;
    public bool isMoving;

    public List<HexCoordinates> animationPath;
    public List<HexCoordinates> reviewAnimation;

    //Support
    public List<Troop> supportedByTroops = new List<Troop>(); //privatize both of these
    public Troop supportingTroop;

    public Troop attackedByTroop;
    public Troop attackingTroop;

    //Action Turn
    public HexCoordinates coords;
    public List<Cell> conflictingCells = new List<Cell>();
    public List<Troop> conflictingTroops = new List<Troop>();

    //class references
    private HexControls hexControls;
    private GameManager gameManager;
    private HexCalculator hexCalculator;

    //action move animation logic
    public bool firstPass = true;
    private bool moving = true;
    private int moveCounter = 0;
    private Vector3 point;
    private float newPosPoint; 
    private float transformPoint;

    // Use this for initialization
    void Start() {
        HexGrid hexGrid = FindObjectOfType<HexGrid>().GetComponent<HexGrid>();
        currentPos = transform.position;
        newPos = currentPos;
        supportedByTroops.Add(this);
        hexControls = FindObjectOfType<HexControls>();
        gameManager = FindObjectOfType<GameManager>();
        hexCalculator = hexGrid.HexCalculator;

    }
    void Update() {
        coords = hexCalculator.HexFromPosition(transform.position);
        if (animationPath.Count > 0) {
            transformPoint = (transform.position.x * direction.x);
            transform.Translate(direction);

            if (firstPass) {
                moving = newPosPoint > transformPoint;
                firstPass = false;
            }
            if (moving != (newPosPoint > transformPoint)) {
                direction = new Vector3(0, 0, 0);
                if (animationPath.Count > moveCounter + 1) {
                    moveCounter++;
                    ActionMove();
                } else {
                    moveCounter = 0;
                    animationPath.Clear();
                    transform.position = newPos;
                    hexControls.TroopMoved(this);
                }
            } 
        }

    }

    public void ActionMove() {
        if (!(animationPath.Count > 0)) {
            moveCounter = 0;
            hexControls.TroopMoved(this);
        } else {
            point = hexCalculator.HexToPosition(animationPath[moveCounter]);
            direction = (point - transform.position) * Time.deltaTime * animationPath.Count;
            newPosPoint = (point.x * direction.x);
        }
    }

    public void Support(Troop troop) {
        HexCoordinates[] neighbors = HexUtility.GetNeighbours(coords);
        if (troop.color != color) { return; }
        foreach (HexCoordinates neighbor in neighbors) {
            if (neighbor == troop.coords) {
                SupportedBy(troop);
                return;
            }
        }
        
    }

    void SupportedBy(Troop ally) {

        foreach (Troop troop in supportedByTroops) {
            if (ally == troop || ally.supportingTroop != null) {
                return;
            }
        }
        foreach (Troop troop in ally.supportedByTroops) {
            if (this == troop) {
                return;
            }
        }
        ally.supportingTroop = this;
        supportedByTroops.Add(ally);

        ally.GetComponent<LineRenderer>().SetPosition(0, ally.transform.position);
        ally.GetComponent<LineRenderer>().SetPosition(1, transform.position);
        
        actionPower += ally.basePower;
    }

    public void Move() {
        
        foreach (Troop troop in supportedByTroops) {
            troop.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, 0));
            troop.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));
            troop.supportingTroop = null;
        }
        actionPower = basePower;
        supportedByTroops.Clear();

    }

    public void DestroyTroop() {
        Destroy(gameObject);
    }

    void OnMouseDown() {
        if (hexControls.selectedTroop == this) {
            hexControls.DeselectCell();
        } else {
            hexControls.SelectTroop(this);
        }
    }

    public void CutSupport() { //Move to Troop since it deals with troop properties
        if (supportingTroop && conflictingTroops.Count > 0) {
            supportingTroop.supportedByTroops.Remove(this);
            supportingTroop.actionPower -= basePower;
            supportingTroop = null;
        }
    }

    public void ResolveConflicts() {
        if (conflictingCells.Count == 0) { return; }

        foreach (Cell cell in conflictingCells) { // loop through conflicts 
            //If conflict cell is parent cell only and conflictTroop is greater than -> unit retreats
            //if conflict troop is less than -> add cell to final path list | conflictSolved = true;
            //if conflict troop is equal to or greater than -> stop looping or set pathStopped bool to true and ignore rest
        }
        //Set so troops that had solved conflicts are in their new locations | set new pos
        //if conflictSolved == true -> restart ActionTurn (For troops that have conflicts but didn't have them solved, if spot they were going to move into is now vacant they will be able to move)
        //if conflictSolved == false -> move on to Handle Action


    }

    void ResolveConflicts(List<Cell> conflictCell) { // Move to Troop because each troop should be doing this individually, loop is unnecesary
        print("resolving conflicts of count: " + conflictCell.Count);
        foreach (Troop troop in gameManager.troopArray) {
            if (troop.conflictingCells.Count > 1) {
                print("multiple conflicts");
                foreach (Cell cell in troop.conflictingCells) {
                    HexCoordinates coords = hexCalculator.HexFromPosition(cell.transform.position);
                    int troopIndex = cell.conflictingTroops.IndexOf(troop);
                    int otherTroopIndex = 1 - troopIndex;
                    if (troop.currentPos != cell.transform.position && (cell.conflictingTroops[troopIndex].actionPower == cell.conflictingTroops[otherTroopIndex].actionPower || cell.conflictingTroops[troopIndex].actionPower < cell.conflictingTroops[otherTroopIndex].actionPower)) {
                        print("troop not greater than");
                        if (cell.conflictingTroops[troopIndex].animationPath.Count > 1 && cell.conflictingTroops[troopIndex].animationPath.IndexOf(coords) > 0)
                            cell.conflictingTroops[troopIndex].animationPath.RemoveRange(cell.conflictingTroops[troopIndex].animationPath.IndexOf(coords), cell.conflictingTroops[troopIndex].animationPath.Count - cell.conflictingTroops[troopIndex].animationPath.IndexOf(coords));

                        // troop.newPos = hexCalculator.HexToPosition(troop.animationPath[troop.animationPath.Count - 1]);
                        conflictCell.Remove(cell);
                    }
                }
            }
            troop.conflictingCells.Clear();
        }
        conflictCell.Clear();
        conflictCell = FindConflicts();


        foreach (Cell cell in conflictCell) {
            HexCoordinates coords = hexCalculator.HexFromPosition(cell.transform.position);
            print(cell.conflictingTroops[0].animationPath.Count);
            print(cell.conflictingTroops[1].animationPath.Count);

            if (cell.conflictingTroops[0].color != cell.conflictingTroops[1].color) {

                if (cell.conflictingTroops[0].actionPower == cell.conflictingTroops[1].actionPower) {

                    if (cell.conflictingTroops[0].animationPath.Count > 1 && cell.conflictingTroops[0].animationPath.IndexOf(coords) > 0)
                        cell.conflictingTroops[0].animationPath.RemoveRange(cell.conflictingTroops[0].animationPath.IndexOf(coords), cell.conflictingTroops[0].animationPath.Count - cell.conflictingTroops[0].animationPath.IndexOf(coords));

                    if (cell.conflictingTroops[1].animationPath.Count > 1 && cell.conflictingTroops[1].animationPath.IndexOf(coords) > 0)
                        cell.conflictingTroops[1].animationPath.RemoveRange(cell.conflictingTroops[1].animationPath.IndexOf(coords), cell.conflictingTroops[1].animationPath.Count - cell.conflictingTroops[1].animationPath.IndexOf(coords));

                } else if (cell.conflictingTroops[0].actionPower < cell.conflictingTroops[1].actionPower) {

                    if (cell.conflictingTroops[0].animationPath.Count > 1 && cell.conflictingTroops[0].animationPath.IndexOf(coords) > 0)
                        cell.conflictingTroops[0].animationPath.RemoveRange(cell.conflictingTroops[0].animationPath.IndexOf(coords), cell.conflictingTroops[0].animationPath.Count - cell.conflictingTroops[0].animationPath.IndexOf(coords));


                    GetRetreatPath(cell.conflictingTroops[0], cell);

                } else if (cell.conflictingTroops[0].actionPower > cell.conflictingTroops[1].actionPower) {
                    if (cell.conflictingTroops[1].animationPath.Count > 1 && cell.conflictingTroops[1].animationPath.IndexOf(coords) > 0)
                        cell.conflictingTroops[1].animationPath.RemoveRange(cell.conflictingTroops[1].animationPath.IndexOf(coords), cell.conflictingTroops[1].animationPath.Count - cell.conflictingTroops[1].animationPath.IndexOf(coords));


                    GetRetreatPath(cell.conflictingTroops[1], cell);

                }
            }

            if (cell.conflictingTroops[0].currentPos == cell.transform.position && !(cell.conflictingTroops[0].animationPath.Count > 1)) {
                if (cell.conflictingTroops[1].animationPath.Count > 1)
                    cell.conflictingTroops[1].animationPath.RemoveRange(cell.conflictingTroops[1].animationPath.IndexOf(coords), cell.conflictingTroops[1].animationPath.Count - cell.conflictingTroops[1].animationPath.IndexOf(coords));
            } else if (cell.conflictingTroops[1].currentPos == cell.transform.position && !(cell.conflictingTroops[1].animationPath.Count > 1)) {
                if (cell.conflictingTroops[0].animationPath.Count > 1)
                    cell.conflictingTroops[0].animationPath.RemoveRange(cell.conflictingTroops[0].animationPath.IndexOf(coords), cell.conflictingTroops[0].animationPath.Count - cell.conflictingTroops[0].animationPath.IndexOf(coords));
            }


            print(cell.conflictingTroops[0].animationPath.Count);
            print(cell.conflictingTroops[1].animationPath.Count);
            cell.conflictingTroops.Clear();


        }
        foreach (Troop troop in gameManager.troopArray) {
            troop.newPos = hexCalculator.HexToPosition(troop.animationPath[troop.animationPath.Count - 1]);
        }
        conflictCell = FindConflicts();
        if (conflictCell.Count > 0 && count < 10) {
            count++;
            print("Conflict function count: " + count);
            ResolveConflicts(conflictCell);
        }
    }

    public void HandleAction() { //Troop because it deals only with troop properties, loop is unnecesary
        print("handling action");
        foreach (Troop thisTroop in gameManager.troopArray) {


            thisTroop.reviewAnimation.Clear();
            foreach (HexCoordinates path in thisTroop.animationPath) {
                thisTroop.reviewAnimation.Add(path);
            }
            print("Final count: " + thisTroop.animationPath.Count);
            //thisTroop.newPos = hexCalculator.HexToPosition(thisTroop.animationPath[thisTroop.animationPath.Count - 1]);
            thisTroop.animationPath.RemoveRange(0, 1);
            thisTroop.ActionMove();
            thisTroop.actionPower = thisTroop.basePower;
            thisTroop.supportingTroop = null;
            thisTroop.attackedByTroop = null;
            thisTroop.supportedByTroops.Clear();
            thisTroop.currentPos = thisTroop.newPos;

        }
    }

}


