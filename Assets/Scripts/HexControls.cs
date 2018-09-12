﻿using System.Collections;
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
        public Troop selectedTroop;
        private PlayBack playBack;


        private void Start() {
            HexGrid hexGrid = GetComponent<HexGrid>();

            hexCalculator = hexGrid.HexCalculator;
            pathFinder = FindObjectOfType<PathFinder>();
            gameManager = FindObjectOfType<GameManager>();
            possibleMoves = new List<HexCoordinates>();
            playBack = FindObjectOfType<PlayBack>();

            cells = new HexContainer<Cell>(hexGrid);
            cells.FillWithChildren();

            // SetUpMap();
        }



        private void Update() {



            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                HexCoordinates mouseCoords = hexCalculator.HexFromPosition(mouse);

                if (player == CellColor.White)
                    return;

                //Move or select cell
                if (possibleMoves.Contains(mouseCoords) && selectedTroop && mouseCoords != hexCalculator.HexFromPosition(selectedTroop.transform.position)) {
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

        public void HandleWin(CellColor color) //Call from GameManager when game is over
        {


            CellColor winner = CellColor.White;

            print("Game over! " + color + " Wins!");
            cameraAnimator.SetInteger("Player", (int)winner);

        }


        //Change the player and the background
        public void ChangePlayer(CellColor color) {

            player = color;

            cameraAnimator.SetInteger("Player", (int)player);

        }


        List<HexCoordinates> GetPossibleMoves(HexCoordinates coords, int movement) {
            HexPathFinder pathing = new HexPathFinder(HexCost);
            List<HexCoordinates> path;
            List<HexCoordinates> moves = new List<HexCoordinates>();

            var newCoords = HexUtility.GetInRange(coords, movement);

            foreach (var c in newCoords) {
                Cell cell = cells[c];

                if (cell != null) {
                    pathing.FindPath(coords, c, out path);
                    if (!(pathFinder.CalculatePathCost(path) > cells[coords].GetComponentInChildren<Troop>().movement - 1)) {
                        moves.Add(c);
                    }
                }
            }


            return moves;
        }

        //Turn off all highlighted cells
        public void DeselectCell() {
            foreach (var move in possibleMoves) {
                cells[move].IsHighlighted = false;
            }
            possibleMoves.Clear();
            selectedTroop = null;
        }

        //Select cell and highlight possible moves
        private void SelectCell(HexCoordinates coords) {
            DeselectCell();

            Cell cell = cells[coords];
            //if active player isn't owner, return
            if (cell == null || !(cell.GetComponentInChildren<Troop>()) || cell.GetComponentInChildren<Troop>().color != player)
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
                    troop.supportingTroop.actionPower -= troop.basePower;
                    troop.supportingTroop = null;
                }
            }
            //Highlight all possible moves
            foreach (HexCoordinates move in possibleMoves) {
                cells[move].IsHighlighted = true;
            }

        }

        public void SetUpMap() { //add snap functionality to troops
            foreach (Troop troop in gameManager.troopArray) {
                troop.transform.SetParent(cells[troop.coords].transform);
            }
        }

        public void TroopMoved(Troop troop) {
            troop.transform.SetParent(cells[troop.coords].transform);
        }


        public float HexCost(HexCoordinates a, HexCoordinates b) {

            Cell cell = cells[b];

            foreach (Troop troop in gameManager.troopArray)
                if ((cell == null || cell.transform.position == troop.transform.position))
                    return float.PositiveInfinity;

            return 1;
        }

        public void FindConflicts() { //Keep here since it returns Cells
            FindTroopPaths();
            List<Cell> conflictCells = new List<Cell>();
            foreach (Troop thisTroop in gameManager.troopArray) {
                thisTroop.conflictingCells.Clear();
                thisTroop.conflictingTroops.Clear();
                HexDirection dir;
                if (thisTroop.animationPath.Count > 1) {
                    thisTroop.isMoving = true;
                    dir = HexUtility.NeighbourToDirection(thisTroop.animationPath[0],thisTroop.animationPath[1]);
                    thisTroop.movingRight = dir == HexDirection.NE || dir == HexDirection.E || dir == HexDirection.SE;
                } else {
                    thisTroop.isMoving = false;
                }
                foreach (Troop thatTroop in gameManager.troopArray) {
                    if (thisTroop != thatTroop) {
                        foreach (HexCoordinates thisPath in thisTroop.animationPath) {
                            foreach (HexCoordinates thatPath in thatTroop.animationPath) {
                                if ((thisTroop.transform.position == thatTroop.transform.position || thisPath == thatPath) && !(conflictCells.Contains(cells[thisPath]))) {
                                    if (!thisTroop.isMoving || thisTroop.movingRight != thatTroop.movingRight) {
                                        conflictCells.Add(cells[thisPath]);
                                        thisTroop.conflictingTroops.Add(thatTroop);
                                        thisTroop.conflictingCells.Add(cells[thisPath]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FindTroopPaths() { // Keep here because it deals with Cell Pathfinding
            
            foreach (Troop thisTroop in gameManager.troopArray) {
                thisTroop.animationPath.Clear();
                thisTroop.animationPath = pathFinder.FindPath(thisTroop.currentPos, thisTroop.newPos);
                if (cells[hexCalculator.HexFromPosition(thisTroop.newPos)].GetComponentInChildren<Troop>() && thisTroop != cells[hexCalculator.HexFromPosition(thisTroop.newPos)].GetComponentInChildren<Troop>()) {
                    thisTroop.animationPath.Add(hexCalculator.HexFromPosition(thisTroop.newPos));
                }
                thisTroop.animationPath.Insert(0, hexCalculator.HexFromPosition(thisTroop.currentPos));
            }
        }


        public void GetRetreatPath(Troop troop, Cell cell) { //Keep here since it deals with cell pathfinding
            HexCoordinates coords = hexCalculator.HexFromPosition(cell.transform.position);
            print("coords: " + coords);
            var newCoords = HexUtility.GetInRange(coords, 1);
            Dictionary<int, Vector3> retreatPos = new Dictionary<int, Vector3>();
            foreach (var c in newCoords) {
                

                if ((!cells[c].GetComponentInChildren<Troop>() || GetComponentInChildren<Troop>() != troop) && troop.color == CellColor.Blue && c.Y == coords.Y && c.X < coords.X) {
                    retreatPos.Add(1, hexCalculator.HexToPosition(c));
                } else if ((!cells[c].GetComponentInChildren<Troop>()|| GetComponentInChildren<Troop>() != troop) && troop.color == CellColor.Red && c.Y == coords.Y && c.X > coords.X) {
                    retreatPos.Add(2, hexCalculator.HexToPosition(c));
                } else if ((!cells[c].GetComponentInChildren<Troop>()|| GetComponentInChildren<Troop>() != troop) && troop.color == CellColor.Blue && c.X < coords.X) {
                    retreatPos.Add(3, hexCalculator.HexToPosition(c));
                } else if ((!cells[c].GetComponentInChildren<Troop>()|| GetComponentInChildren<Troop>() != troop) && troop.color == CellColor.Red && c.X > coords.X) {
                    retreatPos.Add(4, hexCalculator.HexToPosition(c));
                } 


               
            }
            if (retreatPos.ContainsKey(1)) {
                troop.newPos = retreatPos[1];
                troop.animationPath.Add(hexCalculator.HexFromPosition(retreatPos[1]));
            } else if (retreatPos.ContainsKey(2)) {
                troop.newPos = retreatPos[2];
                troop.animationPath.Add(hexCalculator.HexFromPosition(retreatPos[2]));
            } else if (retreatPos.ContainsKey(3)) {
                troop.newPos = retreatPos[3];
                troop.animationPath.Add(hexCalculator.HexFromPosition(retreatPos[3]));
            } else if (retreatPos.ContainsKey(4)) {
                troop.newPos = retreatPos[4];
                troop.animationPath.Add(hexCalculator.HexFromPosition(retreatPos[4]));
            } else {
                troop.DestroyTroop();
            }

        }

    }

}
