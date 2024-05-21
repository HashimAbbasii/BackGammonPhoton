﻿using UnityEngine;
using System;
using Broniek.Stuff.Sounds;
using Photon.Pun;
using System.Reflection;
using System.Collections;

namespace BackgammonNet.Core
{
    // Implementation of dragging the pieces.

    public class PawnNetwork : MonoBehaviour
    {
        public PhotonView photonView;
        public static PawnNetwork instance;
        public static event Action<int> OnCompleteTurn = delegate { };
        public static event Action<bool> OnGameOver = delegate { };

        public static int[] imprisonedSide = new int[2];        // Is the mode of taking the pieces of given side out of prison?
        public static bool[] shelterSide = new bool[2];         // Is the mode of introducing the pieces of given side into the shelter?

        public static int endSlotNo;                            // sloty ostatniej ćwiartki najdalsze od schronienia
        public static int moves;

        public int pawnColor;                                   // The color of this pawn.
        public int slotNo;                                      // slot to which this pawn is currently assigned
        public int pawnNo;                                      // the position taken by a pawn in a slot

        public SlotNetwork slot;                                      // the slot over which the piece being drawn is currently located
        private Vector3 startPos;
        private GameObject house;
        private bool isDown;                                    // Is the mouse button pressed?
        public bool imprisoned;                                // a given pawn is in prison
        private bool shelter;                                   // whether the piece is above the shelter area
        private int rescuedPawns;                               // the number of pieces of a given color in the shelter
        private int beginSlot;                                  // starting slot number
        public int maxMoves;

        [SerializeField] public Sprite whitePawn;
        [SerializeField] public Sprite blackPawn;

        private void Awake()
        {
            //instance = this;
            photonView = GetComponent<PhotonView>();
        }
        public int movesShow;
        private void Start()
        {
            movesShow = moves;
            var slotNo = (int)photonView.InstantiationData[0];
            var isWhite = (int)photonView.InstantiationData[1];
            
            if (PhotonNetwork.IsMasterClient)
            {
                SlotNetwork.slots[slotNo].PlacePawn(this, isWhite);
            }
            GameControllerNetwork.Instance.allPawns.Add(this);
           // if (isWhite == 1) { GameControllerNetwork.Instance.ePawns.Add(this); }
        }

        public void SetColorAndHouse(int color)
        {

            //GetComponent<SpriteRenderer>().color = color == 0 ? Color.white : Color.red;
            GetComponent<SpriteRenderer>().sprite = color == 0 ? GetComponent<SpriteRenderer>().sprite = whitePawn : GetComponent<SpriteRenderer>().sprite = blackPawn;
            house = GameObject.Find((color == 0 ? "White" : "Red") + " House");
            pawnColor = color;
        }

        //-------- events that carry out dragging a piece

        //private void OnTriggerStay2D(Collider2D other)
        //{
        //    if (other.CompareTag("Slot"))
        //        slot = other.GetComponent<SlotNetwork>();
        //    else if (other.CompareTag("Shelter"))
        //        if (shelterSide[0] || shelterSide[1])
        //            shelter = true;
        //}

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Slot"))
            {
                //slot = other.GetComponent<SlotNetwork>().photonView.ViewID;
                photonView.RPC(nameof(SlotAllotmentRPC), RpcTarget.AllBuffered, other.GetComponent<SlotNetwork>().photonView.ViewID);
                
            }
            else if (other.CompareTag("Shelter"))
              //  if (photonView.IsMine)
            //    {
                    if (shelterSide[0] || shelterSide[1])
                    {
                        shelter = true;
                        photonView.RPC(nameof(UpdateShelterState), RpcTarget.AllBuffered, shelter);
                    //}
                }
        }


       // private bool shelter = false;
        [PunRPC]
        public void SlotAllotmentRPC(int SlotViewId)
        {
            slot = PhotonView.Find(SlotViewId).GetComponent<SlotNetwork>();
            //slot = slotinfo;
        }


        [PunRPC]

        public void UpdateShelterState(bool newShelterState)
        {
            shelter = newShelterState;
        }

        [PunRPC]

        public void UpdateShelterStateFalse(bool newShelterState)
        {
            shelter = newShelterState;
        }



        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Slot"))
                //SlotUnallotmentRPC();// when we are not in the area of ​​any of the slots (in the context of OnTriggerStay2D)
            {
                photonView.RPC(nameof(SlotDeallocateRPC), RpcTarget.AllBuffered, BoardNetwork.Instance.photonView.ViewID, slotNo);
            }
            else if (other.CompareTag("Shelter"))
            {
                shelter = false;
                photonView.RPC(nameof(UpdateShelterStateFalse), RpcTarget.AllBuffered, shelter);
            }
        }


        [PunRPC]
        public void SlotDeallocateRPC(int boardViewId, int slotIndex)
        {
            var testBoard = PhotonView.Find(boardViewId).GetComponent<BoardNetwork>();
            slot = testBoard.slots[slotIndex];
        }

        private void OnMouseDown()
        {
            if (GameControllerNetwork.turn != int.Parse(PhotonNetwork.NickName)) return;

            if (GameControllerNetwork.turn != pawnColor) return;

            if (GameControllerNetwork.GameOver) return;

            if (BoardNetwork.Instance.client)              // if network game
            {
                if (BoardNetwork.Instance.isClientWhite)
                {
                    if (pawnColor != 0) return;

                    if (!imprisoned && imprisonedSide[pawnColor] > 0) return;     // in a situation of imprisonment, do not allow unrestricted pieces to be dragged

                    TrySelectPawn();
                }
                else
                {
                    if (pawnColor != 1) return;

                    if (!imprisoned && imprisonedSide[pawnColor] > 0) return;     // in a situation of imprisonment, do not allow unrestricted pieces to be dragged

                    TrySelectPawn();
                }
            }
            else
            {
                if (!imprisoned && ((imprisonedSide[0] > 0 && pawnColor == 0) || (imprisonedSide[1] > 0 && pawnColor == 1))) return;    // in a situation of imprisonment, do not allow unrestricted pieces to be dragged
                
                TrySelectPawn();



            }


        }

        private void TrySelectPawn()
        {
            //if (!GameControllerNetwork.dragEnable || GameControllerNetwork.turn != pawnColor) return;
            if (SlotNetwork.slots[slotNo].Height() - 1 != pawnNo) return; // only the highest pawn in the slot can be moveds
    
            beginSlot = slotNo;
            startPos = transform.position;
            isDown = true;

            if (GameControllerNetwork.turn == int.Parse(PhotonNetwork.NickName))
            {

                TryHighlight(true);     // we turn on the highlighting of the appropriate slots
            }
            //else if(GameControllerNetwork.turn != int.Parse(PhotonNetwork.NickName))
            //{
            //    TryHighlight(true);
            //}
        }

        private void OnMouseDrag()
        {
            if (isDown)                     // you need to convert the cursor positions to world positions
            {
                Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                transform.position = new Vector3(worldPos.x, worldPos.y, -1);
            }
        }

        private void OnMouseUp()
        {
            if (isDown)
            {
                TryHighlight(false);        // we turn off the highlighting of the appropriate slots
                isDown = false;

                if (IsPatology())
                    return;                 // impossible moves (against the rules of the game)
                                            //------------ a mechanism that guarantees the correct movement of the pieces
                CheckShelterStage();

                if (TryPlace())
                {                                       // prison mode support
                    CheckShelterAndMore();

                    if (BoardNetwork.Instance.client)          // network game
                    {
                        string msg = "CMOV|" + beginSlot.ToString() + "|" + slot.slotNo.ToString() + "|" + shelter.ToString() + "|";
                        
                        if (BoardNetwork.Instance.isClientWhite)                                       // time synchronization
                            msg += TimeController.Instance.timeLapse[0].ToString("0.00");
                        else
                            msg += TimeController.Instance.timeLapse[1].ToString("0.00");

                        BoardNetwork.Instance.client.Send(msg);           // Send information about the move to the server.
                    }
                }

                CheckIfNextTurn();
            }
        }

        public void OpponentMove(int toSlot, bool isShelter)    // move your network opponent's piece
        {
            shelter = isShelter;
            slot = SlotNetwork.slots[toSlot];

            CheckShelterStage();

            if (TryPlace())                                     // prison mode support
                CheckShelterAndMore();

            CheckIfNextTurn();
        }

        public void CheckIfNextTurn()
        {
            if (moves == maxMoves && !GameControllerNetwork.GameOver)           // all moves have been made
            {
                moves = 0;
                OnCompleteTurn(pawnColor);
            }
        }


        public void ChangeTheTurn()
        {
            moves = 0;
            OnCompleteTurn(pawnColor);
        }

        private void TryRemovePawnFromJail()
        {
            if (imprisonedSide[pawnColor] > 0 && imprisoned)
            {
                imprisoned = false;
                imprisonedSide[pawnColor]--;
            }
        }

        public void CheckShelterAndMore()
        {
            if (slotNo != 0) TryRemovePawnFromJail();
            if (slotNo != 25) TryRemovePawnFromJail();

            if (CheckShelterStage())                //---- detection of the mode of introducing the pieces into the shelter
                shelterSide[pawnColor] = true;
           // Debug.Log("Moves " + moves);
           // Debug.Log("Max Moves"+maxMoves);
            if (++moves < maxMoves)      // after each move
            {
               // Debug.Log("andr aya hain");
                if (!GameControllerNetwork.CanMove(1))        // when a move cannot be made
                {
                    Debug.Log("andr aya hain For Complete");
                    moves = 0;
                    OnCompleteTurn(pawnColor);
                }
            }
            //if(moves == 3)
            //{
            //    OnCompleteTurn(pawnColor);
            //}
        }

        //..............................Set the CheckShelter and More.........................//

        public void CheckShelterAndMoreAI()
        {
            if (slotNo != 0) TryRemovePawnFromJail();
            if (slotNo != 25) TryRemovePawnFromJail();

            if (CheckShelterStage())                //---- detection of the mode of introducing the pieces into the shelter
                shelterSide[pawnColor] = true;
            Debug.Log("Moves " + moves);
            Debug.Log("Max Moves" + maxMoves);
            if (++moves < maxMoves)      // after each move
            {
                Debug.Log("andr aya hain");
                if (!GameControllerNetwork.CanMove(1))        // when a move cannot be made
                {
                    Debug.Log("andr aya hain For Complete");
                    moves = 0;
                    OnCompleteTurn(pawnColor);
                }
            }
        }

        private bool IsPatology()
        {
            if (slot.slotNo == 0 || slot.slotNo == 25)                  // prison slots
            {
                Debug.Log("Drag Wrong");
                transform.position = startPos;
                return true;
            }

            if (slot.Height() > 1 && slot.IsWhite() != pawnColor)     // there is more than one opponent's piece in a slot
            {
                Debug.Log("tHERE IS NOT MORE THAN ONE SLOT IN THE OPPEONENT");
                transform.position = startPos;
                return true;
            }

            return false;
        }

        private bool TryPlace()
        {
            if (shelter)                                // only when the piece being drawn is above the shelter area
            {
                if (shelterSide[pawnColor])  // we additionally take into account dragging a piece to the field symbolizing their home
                    if (CanPlaceShelter())
                        return true;

                transform.position = startPos;
                return false;
            }
            else
            {
                if (slot.slotNo == slotNo)              // same slot
                {
                    transform.position = startPos;
                    return false;
                }

                int sign = pawnColor == 0 ? 1 : -1;

                if (slot.slotNo == slotNo + sign * GameControllerNetwork.dices[0])
                {
                    Debug.Log("Runs1");
                    DoCorrectMove(0);
                }   // that it matches the values ​​on the dice
                else if (slot.slotNo == slotNo + sign * GameControllerNetwork.dices[1])
                {
                    Debug.Log("Runs2");
                    DoCorrectMove(1);
                }
                else                                    // does not agree with the values ​​thrown out
                {
                    transform.position = startPos;
                    return false;
                }

                return true;
            }
        }

        private void TryHighlight(bool state)           // highlighting the appropriate slots
        {
            int sign = pawnColor == 0 ? 1 : -1;

            int slot0 = slotNo + sign * GameControllerNetwork.dices[0];
            int slot1 = slotNo + sign * GameControllerNetwork.dices[1];

            if (slot0 > 0 && slot0 < 25 && slot0 != slotNo)
                if (!(SlotNetwork.slots[slot0].Height() > 1 && SlotNetwork.slots[slot0].IsWhite() != pawnColor))    // there is no more than one piece of a different color in a slot
                    SlotNetwork.slots[slot0].HightlightMe(state);

            if (slot1 > 0 && slot1 < 25 && slot1 != slotNo)
                if (!(SlotNetwork.slots[slot1].Height() > 1 && SlotNetwork.slots[slot1].IsWhite() != pawnColor))    // there is no more than one piece of a different color in a slot
                    SlotNetwork.slots[slot1].HightlightMe(state);
        }

        private void DoCorrectMove(int diceNo)
        {
            //  var testSlot = PhotonView.Find(slotViewId).GetComponent<SlotNetwork>();
            //............yaha p issue hain Hopefuly...................//
            //   int boardViewId=BoardNetwork.Instance.photonView.ViewID;
            //Debug.Log("Run1");
            if (slot.Height() == 1 && slot.IsWhite() != pawnColor)
            {
                PlaceJail();
               StartCoroutine(delayForSlot());
                GameControllerNetwork.dices[diceNo] = 0;
                return;

            }   // a slot with one opponent's piece
            SlotNetwork.slots[slotNo].GetTopPawn(true);                      // we remove the piece from the slot that has been occupied so far
            slot.PlacePawn(this, pawnColor);             // we remove the piece from the slot that has been occupied so far



            if (!GameControllerNetwork.isDublet)
                GameControllerNetwork.dices[diceNo] = 0;
            photonView.RPC(nameof(MoveScoreUpdateRPC), RpcTarget.AllBuffered, GameManager.instance.myNetworkPlayer.photonView.ViewID);
            // SoundManager.GetSoundEffect(1, 0.2f);
            AudioManager.Instance.PawnPlacement();
        }


       // ..................Code By Hashim Shazad.............................//

        [PunRPC]
        public void MoveScoreUpdateRPC(int playerViewID)
        {
            PhotonView.Find(playerViewID).GetComponent<NetworkPlayer>().Moves++;
        }
        [PunRPC]
        public void ShelterScoreUpdateRPC(int playerViewID)
        {
            PhotonView.Find(playerViewID).GetComponent<NetworkPlayer>().Shelter++;
        }

        //......................Call on Rpc Pun..................................//

        IEnumerator delayForSlot()
        {
            yield return new WaitForSeconds(0.5f);
            SlotNetwork.slots[slotNo].GetTopPawn(true);
            slot.PlacePawn(this, pawnColor);
        }
        public void NormalMovement()
        {

            int boardViewId = BoardNetwork.Instance.photonView.ViewID;
            int index = BoardNetwork.Instance.slots.IndexOf(slot);
            photonView.RPC(nameof(NormalMovementRPC), RpcTarget.AllBuffered, boardViewId, index);
        }


        [PunRPC]
        public void NormalMovementRPC(int boardViewId,int index)
        {
            var testBoeard = PhotonView.Find(boardViewId).GetComponent<BoardNetwork>();

            // var testSlot=PhotonView.Find(index).GetComponent<SlotNetwork>();
            if (PhotonNetwork.IsMasterClient)
            {
               
                var testSlot = testBoeard.slots[slotNo];
                testBoeard.slots[index].GetTopPawn(true);
                testSlot.GetTopPawn(true);
                //Debug.Log("master" + testBoeard.slots[index]);
                //Debug.Log("kha sy ja raha hain" + testSlot);


            }

            //slot.PlacePawn(this.);
          //  slot.PlacePawn(this, pawnColor);
            // testSlot.slot.p



        }
        public void PlaceJail()                   // placing a whipped piece in prison (suspension of introduction to the shelter)
        {
            int boardViewId = BoardNetwork.Instance.photonView.ViewID;
            int index = BoardNetwork.Instance.slots.IndexOf(slot);

            photonView.RPC(nameof(JailScoreUpdateRPC), RpcTarget.AllBuffered, GameManager.instance.myNetworkPlayer.photonView.ViewID);

            photonView.RPC(nameof(PlaceJailRPC), RpcTarget.AllBuffered, boardViewId, index);
            Debug.Log("Hello");
           
        }

        [PunRPC]
        public void JailScoreUpdateRPC(int playerViewID)
        {
            PhotonView.Find(playerViewID).GetComponent<NetworkPlayer>().Kills++;
        }

        [PunRPC]
        public void PlaceJailRPC(int boardViewId, int slotNo)
        {
          
            var testBoeard = PhotonView.Find(boardViewId).GetComponent<BoardNetwork>();
            var testSlot = testBoeard.slots[slotNo];
          //  Debug.Log("Slot test" + slotNo);

            Debug.Log("Slot No" + testBoeard.slots[slotNo]);   // black yaha sy jaha jana hai.....
         
            PawnNetwork pawn = testSlot.GetTopPawn(false);                       // get a whipped piece
            pawn.imprisoned = true;    //...............apny ap ko hi imprsoned mein ly geya........

            if (PhotonNetwork.IsMasterClient)
            {
               // SlotNetwork.slots[slotNo].GetTopPawn(true);
               testBoeard.slots[pawn.pawnColor == 0 ? 0 : 25].PlacePawn(pawn, pawn.pawnColor);
                //var testChck = PhotonView.Find(slotViewId).GetComponent<SlotNetwork>();
                //testBoeard.slots[slotNo].GetTopPawn(true);
               testSlot.GetTopPawn(true);
            }

            imprisonedSide[pawn.pawnColor]++;
            shelterSide[pawn.pawnColor] = false;                            // a piece in prison, therefore no entry into the shelter

            //SoundManager.GetSoundEffect(2, 0.8f);
        }


        //-------- private methods related to shelter mode support

        private bool CanPlaceShelter()
        {
            int value = pawnColor == 0 ? 25 : 0;

            if (slotNo == endSlotNo)                    // the white or red pawn from the farthest slot
            {
                if (CanPlaceShelter(0, value, true) || CanPlaceShelter(1, value, true))
                    return true;
            }
            else if (CanPlaceShelter(0, value, false) || CanPlaceShelter(1, value, false))
                return true;

            return false;
        }

        private bool CanPlaceShelter(int ind, int value, bool lastOrNearer)
        {
            int sign = pawnColor == 0 ? -1 : 1;
            int val = value + sign * slotNo;
            int diceVal = GameControllerNetwork.dices[ind];
            bool result = lastOrNearer ? diceVal >= val : diceVal == val;

            if (result)
            {
                GameControllerNetwork.dices[ind] = GameControllerNetwork.isDublet ? GameControllerNetwork.dices[ind] : 0;
                PlaceInShelter();
            }

            return result;
        }

        private void PlaceInShelter()
        {
            //house.transform.GetChild(rescuedPawns++).gameObject.SetActive(true);
            photonView.RPC(nameof(ActivateHouseChild), RpcTarget.AllBuffered, rescuedPawns);
            photonView.RPC(nameof(ShelterScoreUpdateRPC), RpcTarget.AllBuffered, GameManager.instance.myNetworkPlayer.photonView.ViewID);

            // SoundManager.GetSoundEffect(0, 0.3f);

            if (rescuedPawns == 15)
            {
                OnGameOver(pawnColor == 0);
                GameControllerNetwork.GameOver = true;
            }
            //  photonView.RPC(nameof(RemovePawnFromSlot), RpcTarget.AllBuffered);
            SlotNetwork.slots[slotNo].GetTopPawn(true);
            photonView.RPC(nameof(ScaleAndDestroyGameObject), RpcTarget.AllBuffered);

            // SlotNetwork.slots[slotNo].GetTopPawn(true);            // remove from current slot
            // gameObject.transform.localScale = Vector3.zero;
           // Destroy(gameObject, 1f);
        }



        [PunRPC]
        private void ActivateHouseChild(int childIndex)
        {
            house.transform.GetChild(childIndex).gameObject.SetActive(true);
            rescuedPawns++;
        }

        [PunRPC]
        private void RemovePawnFromSlot()
        {
            SlotNetwork.slots[slotNo].GetTopPawn(true);
        }

        [PunRPC]
        private void ScaleAndDestroyGameObject()
        {
            gameObject.transform.localScale = Vector3.zero;
            Destroy(gameObject, 1f);
        }



        public bool CheckShelterStage()                   // check if it is possible to bring a given player's pieces into the shelter
        {
            maxMoves = GameControllerNetwork.isDublet ? 4 : 2;    // four the same movements or two different movements

            house = GameObject.Find((pawnColor == 0 ? "White" : "Red") + " House");
            rescuedPawns = house.GetComponentsInChildren<SpriteRenderer>().Length - 1;

            int count = 0;
            int offset = pawnColor == 0 ? 18 : 0;
            int b = pawnColor == 0 ? -1 : 1;

            for (int i = 1 + offset; i <= 6 + offset; i++)
                if (SlotNetwork.slots[7 * pawnColor - b * i].Height() > 0 && SlotNetwork.slots[7 * pawnColor - b * i].IsWhite() == pawnColor)
                {
                    if (count == 0)
                        endSlotNo = 7 * pawnColor - b * i;

                    count += SlotNetwork.slots[7 * pawnColor - b * i].Height();
                }

            return (count == 15 - rescuedPawns);   // if all the pieces of a given color, remaining on the board, are in the last quadrant
        }

        public static void InitializePawn()
        {
            moves = 0;
            imprisonedSide = new int[2];
            shelterSide = new bool[2];
        }
    }
}