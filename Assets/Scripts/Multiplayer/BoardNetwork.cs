﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using BackgammonNet.Lobby;
using Photon.Pun;

namespace BackgammonNet.Core
{
    // Create slots and assign them the initial order of pieces.

    public class BoardNetwork : MonoBehaviourPunCallbacks
    {
        [SerializeField] private PhotonView photonView;
        [SerializeField] private SlotNetwork slotPrefab;
        [SerializeField] private PawnNetwork pawnPrefab;
        [SerializeField] public Transform slotsContainer;
        [SerializeField] private Text[] playersNames;
        [SerializeField] private Text infoText;
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private Text infoPanelText;
        [SerializeField] public Button[] submitBtns;

        [HideInInspector] public Client client;             // The client associated with our game created in the Lobby scene.
        [HideInInspector] public bool isClientWhite;        // Is the client white or red?
        [HideInInspector] public bool observer;        
        [HideInInspector] public int acceptance;            // Both players confirm they want to start the game.

        public static BoardNetwork Instance { get; set; }
                
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            photonView=GetComponent<PhotonView>(); 
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("MASTER Client");
                StartCoroutine(createSlotNetwork());
                photonView.RPC(nameof(slotButtonOff), RpcTarget.All);
                StartCoroutine(createPawnNetwork());


               // photonView.RPC(nameof(SpawnSlots), RpcTarget.All);

            }

            if (observer)
                client.Send("COBSRV|observer");             // Send a request for game status information to the host.


            
        }



                
        private void OnDestroy() => StopAllCoroutines();

        public void ShowText(string txt)
        {
            infoPanelText.text = txt;
        }

        private IEnumerator HostExistenceTest()             // Periodically sent information to test the existence of the host.
        {
            while (true)
            {
                yield return new WaitForSeconds(10);

                if (!client.Send("TEST"))
                    ShowText("Host Unreachable!");
            }
        }

        public void OpponentTryMove(int fromSlot, int toSlot, bool isShelter, float moveTime)   // move your network opponent's piece
        {
            PawnNetwork pawn = SlotNetwork.slots[fromSlot].GetTopPawn(false);
            pawn.OpponentMove(toSlot, isShelter);

            if (isClientWhite)
                TimeController.Instance.timeLapse[1] = moveTime;    // if we are white we only synchronize red time
            else
                TimeController.Instance.timeLapse[0] = moveTime;    // if we are red we only synchronize white time
        }
        IEnumerator createSlotNetwork()
        {
            yield return new WaitForSecondsRealtime(1f);
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(SpawnSlots), RpcTarget.AllBuffered);
            }
        }

        IEnumerator createPawnNetwork()
        {
            yield return new WaitForSecondsRealtime(1f);
            if (PhotonNetwork.IsMasterClient)
            {
               photonView.RPC(nameof(PawnSlots), RpcTarget.All);

            }

        }

        #region _SlotInstantiate
        [PunRPC]
        void SpawnSlots()
        {
            float UP_POS = 5.43f;
            SlotNetwork.slots = new List<SlotNetwork>();
            Vector3 slotPos = new Vector3(0, UP_POS, -0.2f);
            Quaternion slotRot = Quaternion.identity;
            CreateSlot(0, slotPos, slotRot);              // prison slot for white

            for (int i = 1; i <= 24; i++)
            {
                float xDelta = (i < 13) ? -1.125f : 1.125f;             // increments on the x-axis of slot positions
                float xOffset = (((i - 1) / 6) % 3 == 0) ? 0 : -1.25f;  // jumping over the middle gang
                float iOffset = (i < 13) ? 1 : 24;
                float ySign = (i < 13) ? 1 : -1;

                slotPos = new Vector3(6.81f + (i - iOffset) * xDelta + xOffset, ySign * UP_POS, -0.2f);
                slotRot = (i < 13) ? Quaternion.identity : Quaternion.Euler(new Vector3(0, 0, 180));
                CreateSlot(i, slotPos, slotRot);
            }

            slotPos = new Vector3(0, -UP_POS, -0.2f);
            slotRot = Quaternion.Euler(new Vector3(0, 0, 180));
            CreateSlot(25, slotPos, slotRot);
        }

        [PunRPC]
        void PawnSlots()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i < 2) CreatePawn(1, 0);    // slot  1
                if (i < 2) CreatePawn(24, 1);   // slot 24            

                if (i < 3) CreatePawn(8, 1);    // slot  8
                if (i < 3) CreatePawn(17, 0);   // slot 17

                CreatePawn(6, 1);               // slot  6
                CreatePawn(12, 0);              // slot 12
                CreatePawn(13, 1);              // slot 13
                CreatePawn(19, 0);              // slot 19
            }
        }

        #endregion
        #region _SlotButton
        [PunRPC]
        public void slotButtonOff()
        {
            submitBtns[0].gameObject.SetActive(false);
            submitBtns[1].gameObject.SetActive(false);
        }

        #endregion

        private void CreateSlot(int slotNo, Vector3 slotPos, Quaternion slotRot)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                object[] data = new object[] { slotNo, "slot" + slotNo.ToString() };

                var slotGo = PhotonNetwork.Instantiate("SlotNetwork", slotPos, slotRot, 0, data);
            }
        }

        
        [PunRPC]
        public void CreateSlotRPC(int slotViewID, int slotNo)
        {
            var slotGo = PhotonView.Find(slotViewID).gameObject;
            slotGo.transform.SetParent(slotsContainer);
            var slot = slotGo.GetComponent<SlotNetwork>();
            slot.name = "slot" + slotNo.ToString();
            slot.slotNo = slotNo;
            SlotNetwork.slots.Add(slot);
        }

        private void DefaultPawns()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i < 2) CreatePawn(1, 0);    // slot  1
                if (i < 2) CreatePawn(24, 1);   // slot 24            

                if (i < 3) CreatePawn(8, 1);    // slot  8
                if (i < 3) CreatePawn(17, 0);   // slot 17

                CreatePawn(6, 1);               // slot  6
                CreatePawn(12, 0);              // slot 12
                CreatePawn(13, 1);              // slot 13
                CreatePawn(19, 0);              // slot 19
            }
        }

        private void CreatePawn(int slotNo, int isWhite)        // assign a pawn to the appropriate slot
        {
            if (PhotonNetwork.IsMasterClient)
            {
                object[] data = new object[] { slotNo, isWhite };
                var slotGo = PhotonNetwork.Instantiate("PawnNetwork", transform.position, Quaternion.identity, 0, data);

            }
            //PawnNetwork pawn = Instantiate(pawnPrefab);
            //SlotNetwork.slots[slotNo].PlacePawn(pawn, isWhite);
          //  GameController.Instance.allPawns.Add(pawn);
           // if (isWhite == 1) { GameController.Instance.ePawns.Add(pawn); }
        }

        //---- recovery of the game

        public void CreateSlotPawns(int slotNo, int height, int isWhite)
        {
            for (int i = 0; i < height; i++)
                CreatePawn(slotNo, isWhite);
        }

        public void PlaceInShelter(int amount, int color)
        {
            GameObject go = GameObject.Find((color == 0 ? "White" : "Red") + " House");

            for (int i = 0; i < amount; i++)
                go.transform.GetChild(i).gameObject.SetActive(true);
        }

        [ContextMenu("Test")]
        public void InstantiateTest()
        {
            PhotonNetwork.Instantiate("Test", Vector3.zero, Quaternion.identity);
        }
    }
}