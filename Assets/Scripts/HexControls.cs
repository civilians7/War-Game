using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HexMapTools;



namespace HexMapToolsExamples
{

    


    [RequireComponent(typeof(HexGrid))]
    public class HexControls : MonoBehaviour {

        public Animator cameraAnimator;


        private HexCalculator hexCalculator;
        private HexContainer<Cell> cells;

        private HexCoordinates selectedCoords;
        private List<HexCoordinates> possibleMoves;
        private List<Cell> blueCells;
        private List<Cell> redCells;
        private CellColor player;
        private bool isGameOver = false;
        private Troop[] troopArray;


        private void Start()
        {
            HexGrid hexGrid = GetComponent<HexGrid>();

            hexCalculator = hexGrid.HexCalculator;
            possibleMoves = new List<HexCoordinates>();
            blueCells = new List<Cell>();
            redCells = new List<Cell>();
            troopArray = FindObjectsOfType<Troop>();
            //player = CellColor.Blue;
            //cameraAnimator.SetInteger("Player", (int)player);


            cells = new HexContainer<Cell>(hexGrid);
            cells.FillWithChildren();

            
            //Count score
            foreach(var pair in cells)
            {
                Cell cell = pair.Value;

                cell.Init(pair.Key);

                if (cell.Color == CellColor.Blue)
                {
                    blueCells.Add(cell);
                }
                else if (cell.Color == CellColor.Red)
                {
                    redCells.Add(cell);
                }
            }


        }

        private void Update()
        {
             troopArray = FindObjectsOfType<Troop>();
            if (isGameOver)
                return;


            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                HexCoordinates mouseCoords = hexCalculator.HexFromPosition(mouse);


                if (player == CellColor.White)
                    return;

                //Move or select cell
                if(possibleMoves.Contains(mouseCoords))
                {
                    Move(mouseCoords);
                    CheckWin();
                    
                }
                else
                {
                    SelectCell(mouseCoords);
                }

            }
        }

        private void Move(HexCoordinates coords) {

            foreach (Troop troop in troopArray) {

                if (cells[coords].transform.position == troop.transform.position && troop.GetComponentInParent<Cell>().Color == player) {
                    troop.Engage(cells[selectedCoords].GetComponentInChildren<Troop>());
                    DeselectCell();
                    return;
                }
            }
            cells[selectedCoords].GetComponentInChildren<Troop>().Move();
            cells[selectedCoords].GetComponentInChildren<Troop>().transform.position = cells[coords].transform.position;
            DeselectCell();

        }

        private void CheckWin()
        {

            //Check if next player can move
            List<Cell> playerCells = null;

            if (player == CellColor.Blue)
                playerCells = redCells;
            else
                playerCells = blueCells;


            //if can move - return
            foreach(Cell cell in playerCells)
            {
                var moves = GetPossibleMoves(cell.Coords, GetComponentInChildren<Troop>().movement);

                if (moves.Count > 0)
                    return;
            }

            //else - game over, check and show who wins
            isGameOver = true;


            CellColor winner = CellColor.White;
            if (blueCells.Count > redCells.Count)
            {
                winner = CellColor.Blue;
            }
            else if (redCells.Count > blueCells.Count)
            {
                winner = CellColor.Red;
            }
            else //Draw
            {
                winner = CellColor.White;
            }


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


                if (cell.Color == CellColor.Blue)
                    blueCells.Remove(cell);
                else if (cell.Color == CellColor.Red)
                    redCells.Remove(cell);


                cell.Color = state;

                if (state == CellColor.Blue)
                    blueCells.Add(cell);
                if (state == CellColor.Red)
                    redCells.Add(cell);
            }
        }


        List<HexCoordinates> GetPossibleMoves(HexCoordinates coords, int movement)
        {
            HexPathFinder pathFinder = new HexPathFinder(HexCost);
            List<HexCoordinates> path;
            List<HexCoordinates> moves = new List<HexCoordinates>();

            var newCoords = HexUtility.GetInRange(coords, movement);

            foreach (var c in newCoords)
            {
                Cell cell = cells[c];
                
                if (cell != null)
                {
                    pathFinder.FindPath(coords, c, out path);
                    if (!(CalculatePathCost(path) > cells[coords].GetComponentInChildren<Troop>().movement - 1)) {
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

            possibleMoves = GetPossibleMoves(coords, cells[selectedCoords].GetComponentInChildren<Troop>().movement);

            //Highlight all possible moves
            foreach(HexCoordinates move in possibleMoves)
            {
                cells[move].IsHighlighted = true;
            }
        }

        float HexCost(HexCoordinates a, HexCoordinates b) {

            Cell cell = cells[b];

            foreach (Troop troop in troopArray)
                if ((cell == null || cell.transform.position == troop.transform.position))
                    return float.PositiveInfinity;                
            
            return 1;
        }

        float CalculatePathCost(List<HexCoordinates> path) {
            float cost = 0;

            for (int i = 1; i < path.Count; ++i) {
                cost += HexCost(path[i - 1], path[i]);
            }

            return cost;
        }

        public void moveTroop(Troop troop) {
            HexCoordinates troopPos = hexCalculator.HexFromPosition(troop.newPos);
            troop.GetComponentInParent<Cell>().Color = CellColor.White;
            troop.transform.SetParent(cells[troopPos].transform);
            troop.GetComponentInParent<Cell>().Color = troop.color;
        }
    }

}
