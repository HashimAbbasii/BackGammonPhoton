using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using DG.Tweening;
using System.Collections;


namespace BackgammonNet.Core
{
    // A class representing a single slot (bar) where pieces are placed.

    public class SlotNetwork : MonoBehaviour
    {
        public static List<SlotNetwork> slots;             // stores created slots
        private float placeOffset = -0.9f;
        public int slotNo;        // slot number assigned at the time of its creation

        public Color lightSlot;
        public Color darkSlot;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject highlighted;              // a reference to a game object representing a lighting effect
        [SerializeField] private Transform pawnsContainer;

        public List<PawnNetwork> pawns = new List<PawnNetwork>();
        private float yOffset = -0.9f;
        private int lastCount;
        private bool isModifyingPosition;

        public PhotonView photonView;

        private void Start()
        {
            photonView = GetComponent<PhotonView>();


            slotNo = (int)photonView.InstantiationData[0];
            name = (string)photonView.InstantiationData[1];
            transform.SetParent(BoardNetwork.Instance.slotsContainer);
            slots.Add(this);
            BoardNetwork.Instance.slots.Add(this);

            spriteRenderer.color = (slotNo % 2 == 0) ? new Color(0.706f, 0.306f, 0.282f) : new Color(0.933f, 0.910f, 0.886f);

            if (slotNo == 0 || slotNo == 25)
                spriteRenderer.color = Color.clear;
        }

        [PunRPC]
        public void PlacePawnRPC(int viewID, int isWhite)
        {
            var pawn = PhotonView.Find(viewID).GetComponent<PawnNetwork>();
            //Debug.Log("Place Pawn  RPC");
            pawn.transform.SetParent(pawnsContainer, true);
            isModifyingPosition = true;
            pawn.transform.DOLocalMove(new Vector3(0, -0.5f + pawns.Count * placeOffset, 0), 0.5f);
            //pawn.transform.localPosition = new Vector3(0, -0.5f + pawns.Count * yOffset, 0);
            StartCoroutine(CorrectHeight());
            pawn.SetColorAndHouse(isWhite);
            Debug.Log("Slot no Differential" + slotNo);
            pawn.slotNo = slotNo;                                   // the slot that the pawn belongs to
            pawn.pawnNo = pawns.Count;                              // the position of the pawn in the slot
            pawns.Add(pawn);
        }

        public void PlacePawn(PawnNetwork pawn, int isWhite)       // put the last piece from the pawns list in the right place in the slot
        {
            Debug.Log("Place Pawn");
            photonView.RPC(nameof(PlacePawnRPC), RpcTarget.AllBuffered, pawn.photonView.ViewID, isWhite);

        }

        public PawnNetwork GetTopPawn(bool pop)
        {
            Debug.Log("pawn Count Check" + pawns.Count);
            if (pawns.Count > 0)
            {
                // Debug.Log("gggg");
                PawnNetwork pawn = pawns[pawns.Count - 1];
                if (pop)
                {
                    photonView.RPC(nameof(RemoveListNetwork), RpcTarget.AllBuffered, photonView.ViewID);
                    //  Debug.Log("LLLLL");
                    // pawns.RemoveAt(pawns.Count - 1);
                }
                return pawn;
            }

            return null;
        }

        public PawnNetwork GetTopPawnForus(bool pop)
        {
            Debug.Log("pawn Count" + pawns.Count);
            if (pawns.Count > 0)
            {
                // Debug.Log("gggg");
                PawnNetwork pawn = pawns[pawns.Count - 1];
                if (pop)
                {
                    photonView.RPC(nameof(RemoveListNetwork), RpcTarget.AllBuffered, photonView.ViewID);
                    //  Debug.Log("LLLLL");
                    // pawns.RemoveAt(pawns.Count - 1);
                }
                return pawn;
            }

            return null;
        }


        [PunRPC]
        public void RemoveListNetwork(int slotViewId)
        {



            var testSlot = PhotonView.Find(slotViewId).GetComponent<SlotNetwork>();
            testSlot.pawns.RemoveAt(pawns.Count - 1);


        }

        public int Height() => pawns.Count;
        public int IsWhite() => pawns[0].pawnColor;
        public void HightlightMe(bool state) => highlighted.SetActive(state);     // highlight yourself

        //---- methods related to the control of the position of the pieces in the slot

        // private void Update() => ModifyPositions();   // adjusting the arrangement of pieces on the slot depending on their number

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
        private void Update()
        {
            if (isModifyingPosition) return;
            ModifyPositions();         // adjusting the arrangement of pieces on the slot depending on their number
        }

        private IEnumerator CorrectHeight()
        {
            yield return new WaitForSeconds(0.5f);

            // Minimum and maximum space between pawns when count > 5
            float minYOffset = -2.0f;
            float maxYOffset = -2.5f;

            if (pawns.Count > 5)
            {
                yOffset = Mathf.Max(minYOffset, maxYOffset / pawnsContainer.childCount);

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
                // Fixed yOffset when pawn count is 5 or less
                float fixedYOffset = -0.5f;
                for (int i = 1; i < pawnsContainer.childCount; i++)
                {
                    placeOffset = -0.9f;
                    pawnsContainer.GetChild(i).transform.localPosition = new Vector3(0, -0.5f + i * fixedYOffset, 0);
                }
            }

            isModifyingPosition = false;
        }
    }
}