using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HexMapTools;

//priority is dealing with the hexes and movement of troops

namespace HexMapTerrain
{


    [RequireComponent(typeof(HexGrid))]
    public class HexControls : MonoBehaviour {

        public Animator cameraAnimator;
        public bool planningMode;

        private HexCalculator hexCalculator;
        private PathFinder pathFinder;
        private HexContainer<Cell> cells;
        

        private HexCoordinates selectedCoords;
        private List<HexCoordinates> possibleMoves;

        private CellColor player;
        private GameManager gameManager;
        private Troop selectedTroop;


        private void Start()
        {
            HexGrid hexGrid = GetComponent<HexGrid>();

            hexCalculator = hexGrid.HexCalculator;
            pathFinder = FindObjectOfType<PathFinder>();
            gameManager = FindObjectOfType<GameManager>();
            possibleMoves = new List<HexCoordinates>();

            cells = new HexContainer<Cell>(hexGrid);
            cells.FillWithChildren();

           // SetUpMap();
        }



        private void Update()
        {
             


            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                HexCoordinates mouseCoords = hexCalculator.HexFromPosition(mouse);

                if (player == CellColor.White)
                    return;

                //Move or select cell
                if (possibleMoves.Contains(mouseCoords) && selectedTroop && mouseCoords != hexCalculator.HexFromPosition(selectedTroop.transform.position))
                {
                    Move(mouseCoords);
                    
                }


            }
        }

        public void SelectTroop(Troop troop) {
            Vector3 cellPos = troop.GetComponentInParent<Cell>().transform.position;
            HexCoordinates cellCoords = hexCalculator.HexFromPosition(cellPos);
            if (player == CellColor.White)
                return;

            //Move or select cell
            if (!selectedTroop) {
                SelectCell(cellCoords);
            }
            


        }

        private void Move(HexCoordinates coords) {
            Troop thisTroop = cells[selectedCoords].GetComponentInChildren<Troop>();
            if (thisTroop.attackingTroop) {
                thisTroop.attackingTroop.attackedByTroop = null;
                thisTroop.attackingTroop = null;
            }
            foreach (Troop troop in gameManager.troopArray) {

                if (cells[coords].transform.position == troop.transform.position && troop.color == thisTroop.color) {
                    troop.Support(thisTroop);
                    DeselectCell();
                    return;
                } else if (cells[coords].transform.position == troop.transform.position && troop.color != thisTroop.color) {
                    if (!troop.attackedByTroop) {
                        troop.attackedByTroop = thisTroop;
                        thisTroop.attackingTroop = troop;
                    }
                    
                }
            }

            thisTroop.Move();
            thisTroop.transform.position = cells[coords].transform.position;
           
            DeselectCell();

        }

        public void HandleWin() //Call from GameManager when game is over
        {


            CellColor winner = CellColor.White;


            cameraAnimator.SetInteger("Player", (int)winner);
            
        }


        //Change the player and the background
        public void ChangePlayer(CellColor color)  
        {

            player = color;

            cameraAnimator.SetInteger("Player", (int)player);

        }


        //Change cell state and count points
        private void ChangeCellState(Cell cell, CellColor state)
        {
            if(cell != null)
            {
                if (cell.Color == state)
                    return;


                cell.Color = state;

         
            }
        }


        List<HexCoordinates> GetPossibleMoves(HexCoordinates coords, int movement)
        {
            HexPathFinder pathing = new HexPathFinder(HexCost);
            List<HexCoordinates> path;
            List<HexCoordinates> moves = new List<HexCoordinates>();

            var newCoords = HexUtility.GetInRange(coords, movement);

            foreach (var c in newCoords)
            {
                Cell cell = cells[c];
                
                if (cell != null)
                {
                    pathing.FindPath(coords, c, out path);
                    if (!(pathFinder.CalculatePathCost(path) > cells[coords].GetComponentInChildren<Troop>().movement - 1)) {
                        moves.Add(c);
                    }
                }
            }
            

            return moves;
        }

        //Turn off all highlighted cells
        private void DeselectCell()
        {
            foreach (var move in possibleMoves)
            {
                cells[move].IsHighlighted = false; 
            }
            possibleMoves.Clear();
            selectedTroop = null;
        }

        //Select cell and highlight possible moves
        private void SelectCell(HexCoordinates coords)
        {
            DeselectCell();

            Cell cell = cells[coords];
            
            //if active player isn't owner, return
            if (cell == null || cell.Color != player)
                return;


            selectedCoords = coords;
            selectedTroop = cells[selectedCoords].GetComponentInChildren<Troop>();
            possibleMoves = GetPossibleMoves(coords, cells[selectedCoords].GetComponentInChildren<Troop>().movement);
            if (planningMode == true) {
                
                Troop troop = cell.GetComponentInChildren<Troop>();
                troop.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, 0));
                troop.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));

                if (troop.supportingTroop) {
                    troop.supportingTroop.supportedByTroops.Remove(troop);
                    troop.supportingTroop.actionPower = troop.supportingTroop.basePower;
                    troop.supportingTroop = null;
                }
            }
            //Highlight all possible moves
            foreach(HexCoordinates move in possibleMoves)
            {
                cells[move].IsHighlighted = true;
            }

        }

        public void SetUpMap() {
            foreach (Troop troop in gameManager.troopArray) {
                HexCoordinates troopPos = hexCalculator.HexFromPosition(troop.transform.position);
                troop.transform.SetParent(cells[troopPos].transform);
                troop.GetComponentInParent<Cell>().Color = troop.color;
            }
        }

        public void TroopMoved(Troop troop) {
            HexCoordinates troopPos = hexCalculator.HexFromPosition(troop.newPos);
            troop.transform.SetParent(cells[troopPos].transform);
            troop.GetComponentInParent<Cell>().Color = troop.color;

        }

        
        public float HexCost(HexCoordinates a, HexCoordinates b) { 

            Cell cell = cells[b];

            foreach (Troop troop in gameManager.troopArray)
                if ((cell == null || cell.transform.position == troop.transform.position))
                    return float.PositiveInfinity;

            return 1;
        }

        public void ActionTurn() {
            foreach (Troop thisTroop in gameManager.troopArray) {
                HexCoordinates targetCell = hexCalculator.HexFromPosition(thisTroop.newPos);
                thisTroop.GetComponentInParent<Cell>().Color = CellColor.White;
                if (thisTroop.attackedByTroop) {
                }
                if (thisTroop.attackedByTroop && thisTroop.actionPower < thisTroop.attackedByTroop.actionPower) { //Unit is defeated
                    UnitRetreat(hexCalculator.HexFromPosition(thisTroop.currentPos));
                }
                
                

            }
            foreach (Troop thisTroop in gameManager.troopArray) {
                HexCoordinates targetCell = hexCalculator.HexFromPosition(thisTroop.newPos);
                thisTroop.animationPath = pathFinder.FindPath(thisTroop.currentPos, thisTroop.newPos);

                if (cells[targetCell].GetComponentInChildren<Troop>() && cells[targetCell].GetComponentInChildren<Troop>().color != thisTroop.color && thisTroop.actionPower > thisTroop.attackingTroop.actionPower) {
                    thisTroop.animationPath.Add(targetCell);
                } else if (cells[targetCell].GetComponentInChildren<Troop>() && cells[targetCell].GetComponentInChildren<Troop>().color != thisTroop.color && thisTroop.actionPower == thisTroop.attackingTroop.actionPower) {
                    thisTroop.newPos = thisTroop.currentPos;
                }

                thisTroop.ActionMove();
                thisTroop.actionPower = thisTroop.basePower;
                thisTroop.supportingTroop = null;
                thisTroop.attackedByTroop = null;
                thisTroop.supportedByTroops.Clear();
                thisTroop.currentPos = thisTroop.newPos;
            }
        }

        void UnitRetreat(HexCoordinates coords) {
            print("retreat");
            Troop troop = cells[coords].GetComponentInChildren<Troop>();
            var newCoords = HexUtility.GetInRange(coords, 1);
            Dictionary<int, Vector3> retreatPos = new Dictionary<int, Vector3>();
            print("coords: " + coords);
            foreach (var c in newCoords) {
                
                if ((!cells[c].GetComponentInChildren<Troop>()) && troop.color == CellColor.Blue && c.Y == coords.Y && c.X < coords.X) {
                    retreatPos.Add(1, hexCalculator.HexToPosition(c));
                } else if ((!cells[c].GetComponentInChildren<Troop>()) && troop.color == CellColor.Red && c.Y == coords.Y && c.X > coords.X) {
                    retreatPos.Add(2, hexCalculator.HexToPosition(c));
                } else if ((!cells[c].GetComponentInChildren<Troop>()) && troop.color == CellColor.Blue && c.X < coords.X) {
                    retreatPos.Add(3, hexCalculator.HexToPosition(c));
                } else if ((!cells[c].GetComponentInChildren<Troop>()) && troop.color == CellColor.Red && c.X > coords.X) {
                    retreatPos.Add(4, hexCalculator.HexToPosition(c));
                } else if (!cells[c].GetComponentInChildren<Troop>()) {
                    retreatPos.Add(5, hexCalculator.HexToPosition(c));
                } 
               
            }
            if (retreatPos.ContainsKey(1)) {
                troop.newPos = retreatPos[1];
            } else if (retreatPos.ContainsKey(2)) {
                troop.newPos = retreatPos[2];
            } else if (retreatPos.ContainsKey(3)) {
                troop.newPos = retreatPos[3];
            } else if (retreatPos.ContainsKey(4)) {
                troop.newPos = retreatPos[4];
            } else if (retreatPos.ContainsKey(5)) {
                troop.newPos = retreatPos[5];
            } else {
                troop.DestroyTroop();
            }

        }

    }

}
