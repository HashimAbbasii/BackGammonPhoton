using UnityEngine;
using System.Collections.Generic;

namespace BackgammonNet.Core
{
    // A class representing a single slot (bar) where pieces are placed.

    public class Slot : MonoBehaviour
    {
        public static List<Slot> slots;             // stores created slots

        [HideInInspector] public int slotNo;        // slot number assigned at the time of its creation

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject highlighted;              // a reference to a game object representing a lighting effect
        [SerializeField] private Transform pawnsContainer;

        public List<Pawn> pawns = new List<Pawn>();
        private float yOffset = -0.9f;
        private int lastCount;

        private void Start()
        {
            spriteRenderer.color = (slotNo % 2 == 0) ? new Color(0, 0.6f, 1) : new Color(0.5f, 0.7f, 0.8f);

            if (slotNo == 0 || slotNo == 25)
                spriteRenderer.color = Color.clear;
        }

        public void PlacePawn(Pawn pawn, int isWhite)       // put the last piece from the pawns list in the right place in the slot
        {
            pawn.transform.SetParent(pawnsContainer, false);
            pawn.transform.localPosition = new Vector3(0, -0.5f + pawns.Count * yOffset, 0);
            pawn.SetColorAndHouse(isWhite);
            pawn.slotNo = slotNo;                                   // the slot that the pawn belongs to
            pawn.pawnNo = pawns.Count;                              // the position of the pawn in the slot
            pawns.Add(pawn);
        }

        public Pawn GetTopPawn(bool pop)
        {
            if (pawns.Count > 0)
            {
               // Debug.Log("gggg");
                Pawn pawn = pawns[pawns.Count - 1];
                if (pop)
                {
                  //  Debug.Log("LLLLL");
                    pawns.RemoveAt(pawns.Count - 1);
                }
                return pawn;
            }

            return null;
        }

        public int Height() => pawns.Count;
        public int IsWhite() => pawns[0].pawnColor;
        public void HightlightMe(bool state) => highlighted.SetActive(state);     // highlight yourself

        //---- methods related to the control of the position of the pieces in the slot

        private void Update() => ModifyPositions();   // adjusting the arrangement of pieces on the slot depending on their number

        private void ModifyPositions()     // modify the positions in the pieces container
        {
            if (pawns.Count != lastCount)
            {
                lastCount = pawns.Count;

                if (pawns.Count > 5)
                    for (int i = 1; i < pawnsContainer.childCount; i++)
                    {
                        pawnsContainer.GetChild(i).transform.localPosition = new Vector3(0, -0.5f + i * yOffset, 0);
                        float value = (20 - pawnsContainer.childCount) / 15f * 0.85f;
                        float posY = pawnsContainer.GetChild(i).transform.localPosition.y * Mathf.Clamp(value, 0f, 1f);
                        pawnsContainer.GetChild(i).transform.localPosition = new Vector3(0, posY, -i / 150f);
                    }
                else
                    for (int i = 1; i < pawnsContainer.childCount; i++)
                        pawnsContainer.GetChild(i).transform.localPosition = new Vector3(0, -0.5f + i * yOffset, 0);
            }
        }
    }
}