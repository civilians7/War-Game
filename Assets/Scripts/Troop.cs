using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexMapTerrain;
using HexMapTools;

public enum CellColor { White = 0, Blue, Red, Purple, Orange, Yellow, Brown, Green }

public class Troop : MonoBehaviour {

    public int movement; //keep public
    public int basePower; //keep public
    public int actionPower; //make private
    public float attackDistance; //keep public

    public Vector3 currentPos;
    public Vector3 newPos;
    public CellColor color = CellColor.Blue;
    public Vector3 direction = new Vector3(0,0,0);

    public List<HexCoordinates> animationPath;

    public List<Troop> supportedByTroops = new List<Troop>(); //privatize both of these
    public Troop supportingTroop;

    public Troop attackedByTroop;
    public Troop attackingTroop;

    public HexCoordinates coords;

    private HexControls hexControls;
    private GameManager gameManager;
    private HexCalculator hexCalculator;
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
        if (animationPath.Count > 0 && gameManager.turnNum == 0) {
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
                    animationPath.Clear();
                    transform.position = newPos;
                    hexControls.TroopMoved(this);
                }
            } 
        }

    }

    public void ActionMove() {
        if (!(animationPath.Count > 0)) {
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
        //GetComponentInParent<Cell>().Color = CellColor.White;
        Destroy(gameObject);
    }

    void OnMouseDown() {
        if (hexControls.selectedTroop == this) {
            hexControls.DeselectCell();
        } else {
            hexControls.SelectTroop(this);
        }
    }

}


