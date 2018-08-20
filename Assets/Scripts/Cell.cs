using UnityEngine;
using HexMapTools;

namespace HexMapTerrain
{

    //public enum CellColor { White = 0, Blue, Red, Purple, Orange, Yellow, Brown, Green }

    [RequireComponent(typeof(Animator))]
    public class Cell : MonoBehaviour
    {
        [SerializeField]
        //private CellColor color = CellColor.White;
        private bool isHighlighted = false;
        private bool isSelected = false;
        private Animator animator;

      //  public CellColor Color
        //{
        //    get { return color; }
        //    set
        //    {
        //        if (color == value)
        //            return;

        //        color = value;
        //    }
        //}

        public bool IsHighlighted
        {
            get { return isHighlighted; }
            set
            {
                if (isHighlighted == value)
                    return;

                isHighlighted = value;
                animator.SetBool("IsHighlighted", isHighlighted);
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected == value)
                    return;

                isSelected = value;
                animator.SetBool("IsSelected", isSelected);
            }
        }

        public HexCoordinates Coords
        {
            get;
            private set;
        }

        private void Start()
        {
            animator = GetComponent<Animator>();

        }

        public void Init(HexCoordinates coords)
        {
            Coords = coords;
        }

        //private void OnDrawGizmos()
        //{
        //    if (Color == CellColor.White)
        //        return;

        //    if (Color == CellColor.Red)
        //        Gizmos.color = UnityEngine.Color.red;
        //    else if (Color == CellColor.Blue)
        //        Gizmos.color = UnityEngine.Color.blue;
        //    else if (Color == CellColor.Orange)
        //        Gizmos.color = UnityEngine.Color.gray;
        //    else if (Color == CellColor.Purple)
        //        Gizmos.color = UnityEngine.Color.magenta;
        //    else if (Color == CellColor.Yellow)
        //        Gizmos.color = UnityEngine.Color.yellow;
        //    else if (Color == CellColor.Green)
        //        Gizmos.color = UnityEngine.Color.green;
        //    else if (Color == CellColor.Brown)
        //        Gizmos.color = UnityEngine.Color.black;

        //    Gizmos.DrawWireSphere(transform.position, 0.433f);
        //}
    }



}
