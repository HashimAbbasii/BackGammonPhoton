using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;

namespace BackgammonNet.Core
{
    // A class representing a single slot (bar) where pieces are placed.

    public class Slot : MonoBehaviour
    {
        public static List<Slot> slots;             // stores created slots

        [HideInInspector] public int slotNo;        // slot number assigned at the time of its creation


        public Color lightSlot;
        public Color darkSlot;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject highlighted;              // a reference to a game object representing a lighting effect
        [SerializeField] private Transform pawnsContainer;

        public List<Pawn> pawns = new List<Pawn>();
        private float yOffset = -0.9f;
        private float placeOffset = -0.9f;
        private int lastCount;
        private bool isModifyingPosition;

        private void Start()
        {
            spriteRenderer.color = (slotNo % 2 == 0) ? new Color(0.706f, 0.306f, 0.282f) : new Color(0.933f, 0.910f, 0.886f); // R/W



            if (slotNo == 0 || slotNo == 25)
                spriteRenderer.color = Color.clear;
        }

        public void PlacePawn(Pawn pawn, int isWhite)       // put the last piece from the pawns list in the right place in the slot
        {

            pawn.transform.SetParent(pawnsContainer, true);

            isModifyingPosition = true;
            pawn.transform.DOLocalMove(new Vector3(0, -0.5f + pawns.Count * placeOffset, 0), 0.5f);
            StartCoroutine(CorrectHeight());

            pawn.SetColorAndHouse(isWhite);
            pawn.slotNo = slotNo;                                   // the slot that the pawn belongs to
            pawn.pawnNo = pawns.Count;                              // the position of the pawn in the slot
            pawns.Add(pawn);
        }

        private IEnumerator CorrectHeight()
        {
            yield return new WaitForSeconds(0.5f);

            if (pawns.Count > 5)
            {
                float difference = 0;
                for (int i = 1; i < pawnsContainer.childCount; i++)
                {
                    pawnsContainer.GetChild(i).transform.localPosition = new Vector3(0, -0.5f + i * yOffset, 0);
                    float value = (20 - pawnsContainer.childCount) / 15f * 0.85f;
                    float posY = pawnsContainer.GetChild(i).transform.localPosition.y * Mathf.Clamp(value, 0f, 1f);
                    pawnsContainer.GetChild(i).transform.localPosition = new Vector3(0, posY, -i / 150f);
                    if (i == pawnsContainer.childCount - 2)
                    {
                        difference = posY;
                    }
                    else if (i == pawnsContainer.childCount - 1)
                    {
                        difference = posY - difference;
                    }
                }

                placeOffset = difference;

            }
            else
            {
                for (int i = 1; i < pawnsContainer.childCount; i++)
                {
                    placeOffset = -0.9f;
                    pawnsContainer.GetChild(i).transform.localPosition = new Vector3(0, -0.5f + i * yOffset, 0);
                }
            }

            isModifyingPosition = false;
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

        private void Update()
        {
            if (isModifyingPosition) return;
            ModifyPositions();         // adjusting the arrangement of pieces on the slot depending on their number
        }

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