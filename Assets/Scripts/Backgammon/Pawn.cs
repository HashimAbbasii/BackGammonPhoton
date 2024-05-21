using UnityEngine;
using System;
using Broniek.Stuff.Sounds;
using Photon.Pun;
using BackgammonNet.Lobby;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace BackgammonNet.Core
{
    // Implementation of dragging the pieces.

    public class Pawn : MonoBehaviour
    {
       // public List<>
        public static Pawn instance;
        public static event Action<int> OnCompleteTurn = delegate { };
        public static event Action<bool> OnGameOver = delegate { };

        public static int[] imprisonedSide = new int[2];        // Is the mode of taking the pieces of given side out of prison?
        public static bool[] shelterSide = new bool[2];         // Is the mode of introducing the pieces of given side into the shelter?

        public static int endSlotNo;
        public int endSlotCheck;
        public static int moves;

        public int pawnColor;                                   // The color of this pawn.
        public int slotNo;                                      // slot to which this pawn is currently assigned
        public int pawnNo;                                      // the position taken by a pawn in a slot

        private Slot slot;                                      // the slot over which the piece being drawn is currently located
        private Vector3 startPos;
        public GameObject house;
        private bool isDown;                                    // Is the mouse button pressed?
        public bool imprisoned;                                // a given pawn is in prison
        private bool shelter;                                   // whether the piece is above the shelter area
        private int rescuedPawns;                               // the number of pieces of a given color in the shelter
        private int beginSlot;                                  // starting slot number
        public int maxMoves;
        [SerializeField] public Sprite whitePawn;
        [SerializeField] public Sprite blackPawn;

        //  public void  Start()
        //{

        //}
        private void Awake()
        {
            instance = this;
        }
        public void SetColorAndHouse(int color)
        {
          
            GetComponent<SpriteRenderer>().sprite = color == 0 ? GetComponent<SpriteRenderer>().sprite = whitePawn : GetComponent<SpriteRenderer>().sprite = blackPawn;
            //GetComponent<SpriteRenderer>().color = color == 0 ? Color.white : Color.black; // black == red
            house = GameObject.Find((color == 0 ? "White" : "Red") + " House");
            pawnColor = color;
        }

        //-------- events that carry out dragging a piece

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Slot"))
                slot = other.GetComponent<Slot>();
            else if (other.CompareTag("Shelter"))
                if (shelterSide[0] || shelterSide[1])
                    shelter = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Slot"))
                slot = Slot.slots[slotNo];         // when we are not in the area of ​​any of the slots (in the context of OnTriggerStay2D)
            else if (other.CompareTag("Shelter"))
                shelter = false;
        }

        private void OnMouseDown()
        {
            //if (GameControllerNetwork.turn != int.Parse(PhotonNetwork.NickName)) return;


            if (GameController.GameOver) return;
            
            if (Board.Instance.client)              // if network game
            {
                if (Board.Instance.isClientWhite)
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


        public void Selectimprisoned()
        {
           // GameController.Instance.Kills++;
            //   if (imprisoned && (imprisonedSide[1] > 0 && pawnColor == 1))

         

            int count = 0;
            int sign = GameController.turn == 0 ? 1 : -1;
            int value = GameController.turn == 0 ? 24 : -1;
            int val = GameController.turn == 0 ? -1 : 24;

            var prisonSlot = Slot.slots[25];
            //if (prisonSlot[25].m == maxMoves)
            //{

            //}


           // Debug.Log("PAWN cOLOR" + pawnColor);
            if (prisonSlot.Height() >= 1 /*&& Slot.slots[25].IsWhite()==pawnColor*/)
            {


              //  Debug.Log("Height Greater than 1 ");
                if (Slot.slots[25 + sign * GameController.dices[0]].Height() > 0)
                {
                    if (Slot.slots[25 + sign * GameController.dices[0]].Height() == 1 && Slot.slots[25 + sign * GameController.dices[0]].IsWhite() != prisonSlot.IsWhite())
                    {
                        Debug.Log("Place JAIL 1");
                        GameController.Instance.playerScores[1].Kills++;
                        //Debug.Log("JAIL KAR DOOH");
                        var prisonPawn = prisonSlot.GetTopPawn(true);
                       
                        int slot0 = Slot.slots[25 + sign * GameController.dices[0]].slotNo;
                        TryHighlight(true);
                        Slot.slots[slot0].GetTopPawn(false).PlaceJail();
                       // Slot.slots[slot0].GetTopPawn(true);
                        Slot.slots[slot0].PlacePawn(prisonPawn, prisonPawn.pawnColor);
                        prisonPawn.imprisoned = false;
                        imprisonedSide[pawnColor]--;
                        CheckShelterStage();
                        CheckShelterAndMore();
                        CheckIfNextTurn();
                        //  StartCoroutine(SecondDice());
                        StartCoroutine(GameController.Instance.SecondDice());


                    }
                    else if (Slot.slots[25 + sign * GameController.dices[0]].Height() > 0 && Slot.slots[25 + sign * GameController.dices[0]].IsWhite() == prisonSlot.IsWhite())


                    {

                        Debug.Log("Same Color Slot 1");
                        GameController.Instance.playerScores[1].Moves++;
                        var prisonPawn = prisonSlot.GetTopPawn(true);
                        int slot0 = Slot.slots[25 + sign * GameController.dices[0]].slotNo;
                        var slotHighlight= Slot.slots[25 + sign * GameController.dices[0]].slotNo;
                        TryHighlight(true);
                        Slot.slots[slot0].PlacePawn(prisonPawn, prisonPawn.pawnColor);
                        prisonPawn.imprisoned = false;
                        imprisonedSide[pawnColor]--;
                        CheckShelterStage();
                        CheckShelterAndMore();
                        CheckIfNextTurn();
                        StartCoroutine(GameController.Instance.SecondDice());

                    }

                    else if (Slot.slots[25 + sign * GameController.dices[0]].Height() > 1 && Slot.slots[25 + sign * GameController.dices[0]].IsWhite() != prisonSlot.IsWhite())
                    {
                        Debug.Log("Height greater than 1 but opposite color");
                        CheckShelterStage();
                        CheckShelterAndMore();
                        CheckIfNextTurn();
                        if (Slot.slots[25 + sign * GameController.dices[1]].Height() > 1 && Slot.slots[25 + sign * GameController.dices[1]].IsWhite() != prisonSlot.IsWhite())
                        {

                            Debug.Log("Give the Dice to the Human Player");
                            CheckShelterStage();
                            CheckShelterAndMore();
                            CheckIfNextTurn();


                        }
                        else
                        {
                            Debug.Log("Moves According to Dice 1");
                            if (Slot.slots[25 + sign * GameController.dices[1]].Height() == 1 && Slot.slots[25 + sign * GameController.dices[1]].IsWhite() != prisonSlot.IsWhite())
                            {
                                Debug.Log("jail karna hain according to Dice 1");

                                //...................................Dice Jail...................//


                                GameController.Instance.playerScores[1].Kills++;
                                //Debug.Log("JAIL KAR DOOH");
                                var prisonSlot1 = prisonSlot.GetTopPawn(true);

                                int slotDice = Slot.slots[25 + sign * GameController.dices[1]].slotNo;
                                TryHighlight(true);
                                Slot.slots[slotDice].GetTopPawn(false).PlaceJail();
                               
                                Slot.slots[slotDice].PlacePawn(prisonSlot1, prisonSlot1.pawnColor);
                                prisonSlot1.imprisoned = false;
                                imprisonedSide[pawnColor]--;
                                CheckShelterStage();
                                CheckShelterAndMore();
                                CheckIfNextTurn();
                             
                                StartCoroutine(GameController.Instance.SecondDice());





                            }

                            else if (Slot.slots[25 + sign * GameController.dices[1]].Height() > 0 && Slot.slots[25 + sign * GameController.dices[1]].IsWhite() == prisonSlot.IsWhite())
                            {
                                //...........If same Color Slot ................//

                                GameController.Instance.playerScores[1].Moves++;
                                var prisonDice = prisonSlot.GetTopPawn(true);
                                int slotDice = Slot.slots[25 + sign * GameController.dices[1]].slotNo;
                                var slotHighlight = Slot.slots[25 + sign * GameController.dices[1]].slotNo;
                                TryHighlight(true);
                                Slot.slots[slotDice].PlacePawn(prisonDice, prisonDice.pawnColor);
                                prisonDice.imprisoned = false;
                                imprisonedSide[pawnColor]--;
                                CheckShelterStage();
                                CheckShelterAndMore();
                                CheckIfNextTurn();
                                StartCoroutine(GameController.Instance.SecondDice());



                            }
                            else if (Slot.slots[25 + sign * GameController.dices[1]].Height() == 0)
                            {

                            var prisonPawn = prisonSlot.GetTopPawn(true);
                            int slot0 = Slot.slots[25 + sign * GameController.dices[1]].slotNo;
                            Slot.slots[slot0].PlacePawn(prisonPawn, prisonPawn.pawnColor);
                            TryHighlight(true);
                            prisonPawn.imprisoned = false;
                            imprisonedSide[pawnColor]--;
                            CheckShelterStage();
                            CheckShelterAndMore();
                            CheckIfNextTurn();
                            StartCoroutine(GameController.Instance.SecondDice());
                            }


                        }
                    }


                }


                else if (Slot.slots[25 + sign * GameController.dices[0]].Height() == 0)
                {
                    GameController.Instance.playerScores[1].Moves++;
                     Debug.Log("HEIGHT IS Zero or Slot is Empty");
                    var prisonPawn = prisonSlot.GetTopPawn(true);
                    int slot0 = Slot.slots[25 + sign * GameController.dices[0]].slotNo;
                    Slot.slots[slot0].PlacePawn(prisonPawn, prisonPawn.pawnColor);
                    prisonPawn.imprisoned = false;
                    imprisonedSide[pawnColor]--;
                    CheckShelterStage();
                    TryHighlight(true);
                    CheckShelterAndMore();
                    CheckIfNextTurn();
                    StartCoroutine(GameController.Instance.SecondDice());

                }


                //...................... aghr Prison Hu gi hain ................
            }

        }
        public void Selectimprisoned2()
        {

          //  Debug.Log("Imprisoned Slot");

            int count = 0;
            int sign = GameController.turn == 0 ? 1 : -1;
            int value = GameController.turn == 0 ? 24 : -1;
            int val = GameController.turn == 0 ? -1 : 24;
           // Debug.Log("PAWN cOLOR" + pawnColor);
            if (Slot.slots[25].Height() >= 1 /*&& Slot.slots[25].IsWhite()==pawnColor*/)
            {


               // Debug.Log("Height Greater than 1 ");
                if (Slot.slots[25 + sign * GameController.dices[1]].Height() > 0)
                {
                    if (Slot.slots[25 + sign * GameController.dices[1]].Height() == 1 && Slot.slots[25 + sign * GameController.dices[1]].IsWhite() != pawnColor)
                    {
                        GameController.Instance.playerScores[1].Kills++;
                        Debug.Log("JAIL KAR DOOH 2");
                        var prisonPawn = Slot.slots[25].GetTopPawn(true);
                        int slot0 = Slot.slots[25 + sign * GameController.dices[1]].slotNo;
                        TryHighlight(true);
                        Slot.slots[slot0].GetTopPawn(false).PlaceJail();
                        Slot.slots[slot0].PlacePawn(prisonPawn, prisonPawn.pawnColor);
                        prisonPawn.imprisoned = false;
                        imprisonedSide[pawnColor]--;
                        CheckShelterStage();
                        CheckShelterAndMore();
                        CheckIfNextTurn();
                      //  OnCompleteTurn(pawnColor);
                        //  StartCoroutine(SecondDice());
                        // StartCoroutine(GameController.Instance.SecondDice());


                    }
                    else if (Slot.slots[25 + sign * GameController.dices[1]].Height() > 0 && Slot.slots[25 + sign * GameController.dices[1]].IsWhite() == pawnColor)


                    {

                       Debug.Log("Same Color Slot For Dice  1");

                        var prisonPawn = Slot.slots[25].GetTopPawn(true);
                        int slot0 = Slot.slots[25 + sign * GameController.dices[1]].slotNo;
                        Slot.slots[slot0].PlacePawn(prisonPawn, prisonPawn.pawnColor);
                        prisonPawn.imprisoned = false;
                        imprisonedSide[pawnColor]--;
                        CheckShelterStage();
                        CheckShelterAndMore();
                        CheckIfNextTurn();
                       // OnCompleteTurn(pawnColor);
                        // StartCoroutine(GameController.Instance.SecondDice());

                    }

                    else if (Slot.slots[25 + sign * GameController.dices[1]].Height() > 1 && Slot.slots[25 + sign * GameController.dices[1]].IsWhite() != pawnColor)
                    {
                        Debug.Log("Not same Color For Dice 1");
                        CheckShelterStage();
                        CheckShelterAndMore();
                        CheckIfNextTurn();
                       // OnCompleteTurn(pawnColor);


                    }


                }


                else if (Slot.slots[25 + sign * GameController.dices[1]].Height() == 0)
                {
                   Debug.Log("HEIGHT IS Zero For Dice 1  ");
                    var prisonPawn = Slot.slots[25].GetTopPawn(true);
                    int slot0 = Slot.slots[25 + sign * GameController.dices[1]].slotNo;
                    Slot.slots[slot0].PlacePawn(prisonPawn, prisonPawn.pawnColor);
                    prisonPawn.imprisoned = false;
                    imprisonedSide[pawnColor]--;
                    CheckShelterStage();
                    CheckShelterAndMore();
                     CheckIfNextTurn();
                 //   OnCompleteTurn(pawnColor);


                }

                //if (Slot.slots[25 + sign * GameController.dices[0]].Height() >= 0 && Slot.slots[25 + sign * GameController.dices[0]].IsWhite() == Slot.slots[25].IsWhite())
                //{
                //    Debug.Log("BAIL OUT");
                //    //..................Slot Prison Bail Out...................//
                //    var prisonPawn = Slot.slots[25].GetTopPawn(true);
                //    int slot0 = Slot.slots[25 + sign * GameController.dices[0]].slotNo;
                //    Slot.slots[slot0].PlacePawn(prisonPawn, prisonPawn.pawnColor);
                //    imprisoned = false;
                //    imprisonedSide[pawnColor]--;
                //    Debug.Log("BAIL OUT afafasf");
                //    GameController.Instance.PrisonIenumertor();

                //    //...............Call the Second Dice movement...........................//



                //}
                //else if (Slot.slots[25 + sign * GameController.dices[0]].Height() == 1 && Slot.slots[25 + sign * GameController.dices[0]].IsWhite() != Slot.slots[25].IsWhite())
                //    {
                //    /// .................Jail Hoon gi hain..................... ///
                //      Debug.Log("IN JAIL");
                //    var prisonPawn = Slot.slots[25].GetTopPawn(false);
                //        int slot0= Slot.slots[25 + sign * GameController.dices[0]].slotNo;
                //        Slot.slots[slot0].GetTopPawn(false).PlaceJail();
                //        Slot.slots[slot0].PlacePawn(prisonPawn, prisonPawn.pawnColor);
                //         imprisoned = false;
                //          imprisonedSide[pawnColor]--;
                //   // StartCoroutine(GameController.Instance.SecondDice());

                //    //...............Call the Second Dice movement...........................//

                //}


                //...................... aghr Prison Hu gi hain ................
            }

        }
        private void TrySelectPawn()
        {
            if (!GameController.dragEnable || GameController.turn != pawnColor) return;
            if (Slot.slots[slotNo].Height() - 1 != pawnNo) return; // only the highest pawn in the slot can be moveds
    
            beginSlot = slotNo;
            startPos = transform.position;
            isDown = true;
            TryHighlight(true);     // we turn on the highlighting of the appropriate slots
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

                    if (Board.Instance.client)          // network game
                    {
                        string msg = "CMOV|" + beginSlot.ToString() + "|" + slot.slotNo.ToString() + "|" + shelter.ToString() + "|";
                        
                        if (Board.Instance.isClientWhite)                                       // time synchronization
                            msg += TimeController.Instance.timeLapse[0].ToString("0.00");
                        else
                            msg += TimeController.Instance.timeLapse[1].ToString("0.00");

                        Board.Instance.client.Send(msg);           // Send information about the move to the server.
                    }
                }

                CheckIfNextTurn();
            }
        }

        public void OpponentMove(int toSlot, bool isShelter)    // move your network opponent's piece
        {
            shelter = isShelter;
            slot = Slot.slots[toSlot];

            CheckShelterStage();

            if (TryPlace())                                     // prison mode support
                CheckShelterAndMore();

            CheckIfNextTurn();
        }

        public void CheckIfNextTurn()
        {
           // Debug.Log("Moves" + moves);
           // Debug.Log("MaxMoves" + maxMoves);
            if (moves == maxMoves && !GameController.GameOver)           // all moves have been made
            {
                moves = 0;
                OnCompleteTurn(pawnColor);
            }


            if (MyGameManager.AiMode == true)
            {


                if (moves == 2 && !GameController.GameOver && GameController.isDublet == true)
                {
                    GameController.Instance.CallDublet();
                }
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
                if (!GameController.CanMove(1))        // when a move cannot be made
                {
                  
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
          //  Debug.Log("Moves " + moves);
          //  Debug.Log("Max Moves" + maxMoves);
            if (++moves < maxMoves)      // after each move
            {
             //   Debug.Log("andr aya hain");
                if (!GameController.CanMove(1))        // when a move cannot be made
                {
                  //  Debug.Log("andr aya hain For Complete");
                    moves = 0;
                    OnCompleteTurn(pawnColor);
                }
            }
        }

        private bool IsPatology()
        {
            if (slot.slotNo == 0 || slot.slotNo == 25)                  // prison slots
            {
           
                transform.position = startPos;
                return true;
            }

            if (slot.Height() > 1 && slot.IsWhite() != pawnColor)     // there is more than one opponent's piece in a slot
            {
            
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

                if (slot.slotNo == slotNo + sign * GameController.dices[0])   // that it matches the values ​​on the dice
                    DoCorrectMove(0);
                else if (slot.slotNo == slotNo + sign * GameController.dices[1])
                    DoCorrectMove(1);
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

            int slot0 = slotNo + sign * GameController.dices[0];
            int slot1 = slotNo + sign * GameController.dices[1];

            if (slot0 > 0 && slot0 < 25 && slot0 != slotNo)
                if (!(Slot.slots[slot0].Height() > 1 && Slot.slots[slot0].IsWhite() != pawnColor))    // there is no more than one piece of a different color in a slot
                    Slot.slots[slot0].HightlightMe(state);

            if (slot1 > 0 && slot1 < 25 && slot1 != slotNo)
                if (!(Slot.slots[slot1].Height() > 1 && Slot.slots[slot1].IsWhite() != pawnColor))    // there is no more than one piece of a different color in a slot
                    Slot.slots[slot1].HightlightMe(state);
        }

        private void DoCorrectMove(int diceNo)
        {
            if (slot.Height() == 1 && slot.IsWhite() != pawnColor)   // a slot with one opponent's piece
                PlaceJail();

            Slot.slots[slotNo].GetTopPawn(true);                      // we remove the piece from the slot that has been occupied so far
                                                                      //Debug.Log("Pawn after MU Before Parent Local Pos " + transform.localPosition);
                                                                      //Debug.Log("Pawn after MU Before Parent Global Pos " + transform.position);

            slot.PlacePawn(this, pawnColor);                          // put a piece in the new slot

            if (!GameController.isDublet)
                GameController.dices[diceNo] = 0;

            //SoundManager.GetSoundEffect(1, 0.2f);
            AudioManager.Instance.PawnPlacement();

            if (MyGameManager.AiMode)
            {
                Debug.Log("Mode Check");
                if (GameController.turn == 0 && MyGameManager.AiMode == true)
                {
                   
                    GameController.Instance.playerScores[0].Moves++;
                }

                if (GameController.turn == 1 && MyGameManager.AiMode == true)
                {
                   
                    GameController.Instance.playerScores[1].Moves++;
                }
            }

            else
            {
                if (GameController.turn == 0 )
                {
                    GameController.Instance.playerScores[0].Moves++;
                }

                if (GameController.turn == 1 )
                {
                    GameController.Instance.playerScores[1].Moves++;
                }
            }
           

            
        }

        public void PlaceJail()                   // placing a whipped piece in prison (suspension of introduction to the shelter)
        {
            Pawn pawn = slot.GetTopPawn(true);                              // get a whipped piece
            pawn.imprisoned = true;

            Slot.slots[pawn.pawnColor == 0 ? 0 : 25].PlacePawn(pawn, pawn.pawnColor); // put the piece in the prison slot
            imprisonedSide[pawn.pawnColor]++;
            shelterSide[pawn.pawnColor] = false;                            // a piece in prison, therefore no entry into the shelter

            if (MyGameManager.AiMode == true)
            {
                if (GameController.turn == 0 && MyGameManager.AiMode == true)
                {
                    GameController.Instance.playerScores[0].Kills++;
                }

                if (GameController.turn == 1 && MyGameManager.AiMode == true)
                {
                    GameController.Instance.playerScores[1].Kills++;
                }
            }
            else
            {
                if (GameController.turn == 0 )
                {
                    GameController.Instance.playerScores[0].Kills++;
                }

                if (GameController.turn == 1 )
                {
                    GameController.Instance.playerScores[1].Kills++;
                }
            }

           




         

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
            int diceVal = GameController.dices[ind];
            bool result = lastOrNearer ? diceVal >= val : diceVal == val;

            if (result)
            {
                GameController.dices[ind] = GameController.isDublet ? GameController.dices[ind] : 0;
                PlaceInShelter();
            }

            return result;
        }

        private void PlaceInShelter()
        {
            GameController.Instance.playerScores[0].Shelter++;
            //...........it Check the How many pawn in the House.............//
            house.transform.GetChild(rescuedPawns++).gameObject.SetActive(true);
            //SoundManager.GetSoundEffect(0, 0.3f);

            if (rescuedPawns == 15)
            {
                OnGameOver(pawnColor == 0);
                GameController.GameOver = true;
            }

            Slot.slots[slotNo].GetTopPawn(true);            // remove from current slot
            gameObject.transform.localScale = Vector3.zero;
            Destroy(gameObject, 1f);

            if(MyGameManager.AiMode)
            {
                if (GameController.turn == 0 && MyGameManager.AiMode == true)
                {
                    GameController.Instance.playerScores[0].Shelter++;
                }
                if (GameController.turn == 0 && MyGameManager.AiMode == true)
                {
                    GameController.Instance.playerScores[1].Shelter++;
                }

            }
            else
            {
                if (GameController.turn == 0 )
                {
                    GameController.Instance.playerScores[0].Shelter++;
                }
                if (GameController.turn == 0 )
                {
                    GameController.Instance.playerScores[1].Shelter++;
                }
            }
           
        }

        //.....................Plce in Shelter Ai........................//
        public void PlaceInShelterAi()
        {
            //......Place In Shelter.............//

            //...........it Check the How many pawn in the House.............//
            house.transform.GetChild(rescuedPawns++).gameObject.SetActive(true);
           
            GameController.Instance.playerScores[1].Shelter++;
            //SoundManager.GetSoundEffect(0, 0.3f);

            if (rescuedPawns == 15)
            {
                OnGameOver(pawnColor == 0);
                GameController.GameOver = true;
            }

            Slot.slots[slotNo].GetTopPawn(true);            // remove from current slot
            gameObject.transform.localScale = Vector3.zero;
            Destroy(gameObject, 1f);

            //if (MyGameManager.AiMode)
            //{
            //    if (GameController.turn == 0 && MyGameManager.AiMode == true)
            //    {
            //        GameController.Instance.playerScores[0].Shelter++;
            //    }
            //    if (GameController.turn == 0 && MyGameManager.AiMode == true)
            //    {
            //        GameController.Instance.playerScores[1].Shelter++;
            //    }

            //}
            //else
            //{
            //    if (GameController.turn == 0)
            //    {
            //        GameController.Instance.playerScores[0].Shelter++;
            //    }
            //    if (GameController.turn == 0)
            //    {
            //        GameController.Instance.playerScores[1].Shelter++;
            //    }
            //}

        }


            public bool CheckShelterStage()                   // check if it is possible to bring a given player's pieces into the shelter
        {
            maxMoves = GameController.isDublet ? 4 : 2;    // four the same movements or two different movements
            //  ..........Is mein yeah sare shelter slots check karta hain k kon si    shlter pr hain means slot ko check kare .....//
            house = GameObject.Find((pawnColor == 0 ? "White" : "Red") + " House");
            rescuedPawns = house.GetComponentsInChildren<SpriteRenderer>().Length - 1;
        

            int count = 0;
            int offset = pawnColor == 0 ? 18 : 0;
            int b = pawnColor == 0 ? -1 : 1;

            for (int i = 1 + offset; i <= 6 + offset; i++)
                if (Slot.slots[7 * pawnColor - b * i].Height() > 0 && Slot.slots[7 * pawnColor - b * i].IsWhite() == pawnColor)
                {
                    //its Check all the shelter slot whch contain pawns
                    if (count == 0)
                    {

                        endSlotNo = 7 * pawnColor - b * i;
                        endSlotCheck = endSlotNo;     // its Check End Slot
                    }

                    count += Slot.slots[7 * pawnColor - b * i].Height();
                  
                }
            ////................ITS Check k pawn kiny shelter par aye hain..........//
            return (count == 15 - rescuedPawns);   // if all the pieces of a given color, remaining on the board, are in the last quadrant
        }

        public static void InitializePawn()
        {
            moves = 0;
            imprisonedSide = new int[2];
            shelterSide = new bool[2];
        }
        public int movesShow;
        private void Start()
        {
            movesShow = moves;
            shelterSide[0] = false;
            shelterSide[1] = false;
        }
    }
}