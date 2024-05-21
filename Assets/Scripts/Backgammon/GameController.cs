using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using BackgammonNet.Lobby;
using Broniek.Stuff.Sounds;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using static UnityEngine.GraphicsBuffer;
using Assets.SimpleLocalization.Scripts;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;
using TMPro;

namespace BackgammonNet.Core
{
    // Mechanisms of generating two random numbers and checking the possibility of making a move.

    public class GameController : MonoBehaviour
    {
        [Header("PlayerVsPlayerScore GameObjects")]
        public GameObject playerVsplayerScorePausePanel;
        public GameObject playerVsplayerScoreGameOverPanel;
        public GameObject playerVsplayerScoreYouWinPanel;

        public GameObject playerScorePausePanel;
        public GameObject playeScoreGameOverPanel;
        public GameObject playerScoreYouWinPanel;

        [Header("PlayerVsPlayerScore Text")]
        public LocalizedTextTMP scoreTextPlayer1PlayerVsPlayerPausePanel;
        public LocalizedTextTMP scoreTextPlayer2PlayerVsPlayerPausePanel;
        public LocalizedTextTMP scoreTextPlayer1VsPlayerGameOverPausePanel;
        public LocalizedTextTMP scoreTextPlayer2VsPlayerGameOverPausePanel;
        public LocalizedTextTMP scoreTextPlayer1VsPlayerYouWinPausePanel;
        public LocalizedTextTMP scoreTextPlayer2VsPlayerYouWinPausePanel;



        public LocalizedTextTMP scoreTextPausePanel;
        public LocalizedTextTMP scoreTextgameOverPausePanel;
        public LocalizedTextTMP scoreTextyouWinPausePanel;


        [Header("PointsScene")]
        public LocalizedTextTMP player0Points;
        public LocalizedTextTMP player1Points;

        [Header("PLayer Names")]
        public TextMeshProUGUI player0Name;
        public TextMeshProUGUI player1Name;

        public List<PlayerScore> playerScores = new();
        private PhotonView _photonView;

        public LocalizedTextTMP difficultyTextGameOverPanel;
        public LocalizedTextTMP difficultyTextYouWinPanel;
        public LocalizedTextTMP difficultyTextPausePanel;

        





        [Header("Panels")]
        public GameObject GameOverPanel;
        public GameObject YouWinPanel;

        public CanvasHandler canvasHandler;
        public HorizontalLayoutGroup topMenu;
        public List<GameObject> buttons;
        public bool menuToggle;

        public Pawn randomSelectPawn;
        public Pawn randomSelectPawn2;
        public Pawn SelectShelterPawn;
        public Pawn SelectShelterPawn2;
        public static int turn;                     // indicates whose turn it is now
        public static int[] dices = new int[2];     // recently drawn numbers
        public static bool isDublet;                // whether a doublet was thrown
        public static bool dragEnable;              // is it possible to drag the pieces
        public bool dragEnableCheck;
        [HideInInspector] public bool newTurn;
        [HideInInspector] public int sidesAgreed;   // the current number of players agreeing to continue the game


        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject newGameInfoPanel;
        [SerializeField] private Button diceButton;
        [SerializeField] private Image[] turnImages;
        [SerializeField] private Text[] diceTexts;
        [SerializeField] private Image[] diceImages;

        [SerializeField] private GameObject AiDifficulty;
        [SerializeField] private GameObject AiDifficulty2;
        [SerializeField] private GameObject AiDifficulty3;
        [SerializeField] public List<Slot> shelterPawn=new();
        public List<Sprite> diceFaces = new List<Sprite>();



        public List<Slot> allSlots = new();
        public List<Pawn> allPawns = new();
        public List<int> _allSlotsInts = new();
        public List<Pawn> topEPawns = new();
        public List<Pawn> checkExistingPawn = new();
        public List<Pawn> ePawns = new();
        //public bool NetworkTurn=true;

        public bool diceEnable = true;             // permission to roll the dice (after making moves)

        public static GameController Instance { get; set; }
        public static bool GameOver { get; set; }

        private void Awake()
        {
            Instance = this;
            if (MyPhotonManager.instance.multiPlayerMode == true)
            {
                diceButton.enabled=false;
            }
            _photonView = GetComponent<PhotonView>();

            Pawn.OnCompleteTurn += Pawn_OnCompleteTurn;
            Pawn.OnGameOver += Pawn_OnGameOver;
            TimeController.OnTimeLimitEnd += Pawn_OnGameOver;

            mainMenuButton.onClick.AddListener(GoToMainMenu);
            newGameButton.onClick.AddListener(NewGame);

            

            if (MyGameManager.AiMode==true)
            {
                 Debug.Log("Ai Mode");

                diceButton.onClick.AddListener(GenerateForAi);


            }
            else
            {
                diceButton.onClick.AddListener(Generate);
            }


            turn = 0;

            turnImages[0].gameObject.SetActive(turn == 0);
            turnImages[1].gameObject.SetActive(1 - turn == 0);
        }

        private void Start()
        {
            for (var i = 0; i < 2; i++)
            {
                PlayerScore playerScore = new();
                playerScores.Add(playerScore);
            }

            if (Board.Instance.client)
                if (!Board.Instance.isClientWhite)
                    diceButton.gameObject.SetActive(false);

            if (Board.Instance.observer)     // lock buttons
                ActivateButtons(false);


            if (MyPhotonManager.instance.multiPlayerMode == true)
            {
                //StartCoroutine(NetworkButton());
            }


            if(MyGameManager.AiMode == false)
            {
                AiDifficulty.gameObject.SetActive(false);
                playerVsplayerScorePausePanel.gameObject.SetActive(true);
                playerVsplayerScoreGameOverPanel.gameObject.SetActive(true);
                playerVsplayerScoreYouWinPanel.gameObject.SetActive(true);


                playerScorePausePanel.gameObject.SetActive(false);
                playeScoreGameOverPanel.gameObject.SetActive(false);
                playerScoreYouWinPanel.gameObject.SetActive(false);


            }



            if(MyGameManager.AiMode == true)
            {
                player0Name.text = "Player";
                player1Name.text = "AI";

                playerVsplayerScorePausePanel.gameObject.SetActive(false);
                playerVsplayerScoreGameOverPanel.gameObject.SetActive(false);
                playerVsplayerScoreYouWinPanel.gameObject.SetActive(false);

                playerScorePausePanel.gameObject.SetActive(true);
                playeScoreGameOverPanel.gameObject.SetActive(true);
                playerScoreYouWinPanel.gameObject.SetActive(true);

            }

            else
            {
                player0Name.text = "Player 1";
                player1Name.text = "Player 2";
            }

        }

        //IEnumerator NetworkButton()
        //{
        //    diceButton.enabled = false;
        //    yield return new WaitForSeconds(2f);
        //    if (MyPhotonManager.instance.multiPlayerMode == true)
        //    {
        //        _photonView.RPC(nameof(SlotButtonDisable), RpcTarget.AllBuffered);
                

        //        diceButton.onClick.AddListener(GenerateForNetwork);
                
        //    }
        //}       
        
        [PunRPC]

        public void DiceEnbleForMaster()
        {
            diceButton.enabled=true;
        }
        [PunRPC]
        public void DiceEnbleForClient()
        {
            Board.Instance.submitBtns[0].gameObject.SetActive(false);
            Board.Instance.submitBtns[1].gameObject.SetActive(false);
            diceButton.enabled = false;
        }
        [PunRPC]
        void ClientTurnStarted(int viewID)
        {
            //Dice Button Off


            //if (Pt == Yn)
            //{
            //      Dice Button On
            //}
        }

        [PunRPC]

        public  void SlotButtonDisable()
        {
            Board.Instance.submitBtns[0].gameObject.SetActive(false);
            Board.Instance.submitBtns[1].gameObject.SetActive(false);
        }


        void StartClientTurn()
        {
            // Call RPC to inform the master client that the client's turn has started
            _photonView.RPC("ClientTurnStarted", RpcTarget.AllBuffered);
        }
        void HandleClientTurn()
        {
            // Disable the master client's dice button
            diceButton.interactable = false;
        }

        public void ActivateButtons(bool active)
        {
            diceButton.interactable = active;
            newGameButton.interactable = active;

            if (Board.Instance.observer)
                newGameInfoPanel.SetActive(active);
        }

        private void OnDestroy()
        {
            Pawn.OnCompleteTurn -= Pawn_OnCompleteTurn;
            Pawn.OnGameOver -= Pawn_OnGameOver;
            TimeController.OnTimeLimitEnd -= Pawn_OnGameOver;
        }

        private void Update()
        {
            if (sidesAgreed == 2)
                LoadGameScene();

            TryDeactivateDigit();
        }

        private void TryDeactivateDigit()
        {
            if (dices[0] == 0)
                diceTexts[0].color = new Color(diceTexts[0].color.r, diceTexts[0].color.g, diceTexts[0].color.b, 0.3f);
            else
                diceTexts[0].color = Color.black;

            if (dices[1] == 0)
                diceTexts[1].color = new Color(diceTexts[1].color.r, diceTexts[1].color.g, diceTexts[1].color.b, 0.3f);
            else
                diceTexts[1].color = Color.black;
        }

        private void Generate()
        {
           
            if (Board.Instance.acceptance >= 2)
            {
               
                canvasHandler.diceRollButton.SetActive(true);
                canvasHandler.diceResults.SetActive(true);
            }

            if (diceEnable && Board.Instance.acceptance >= 2)
            {
               
                dragEnable = true;
                diceEnable = false;

                // SoundManager.GetSoundEffect(4, 0.25f);
                AudioManager.Instance.DiceRoll();

                CheckIfTurnChange(Random.Range(1, 7), Random.Range(1, 7));
                //CheckIfTurnChange(1, 2);

                if (Board.Instance.client)      // network game
                {
                    newTurn = false;

                    string msg = "CDCS|" + dices[0].ToString() + "|" + dices[1].ToString();
                    Board.Instance.client.Send(msg);            // Send the information about the roll of the dice to the server.
                }
            }
        }
        private void GenerateForNetwork()
        {
            if (diceEnable)
            {
                dragEnable = true;
                diceEnable = false;

                //SoundManager.GetSoundEffect(4, 0.25f);
                AudioManager.Instance.DiceRoll();

                CheckIfTurnChange(Random.Range(1,7), Random.Range(1,7));
            }
        }

        public void MenuButtonToggle()
        {
            
            if (menuToggle)
            {
                menuToggle = false;

                StopAllCoroutines();
                StartCoroutine(AnimateTopMenu(false));
            }
            else
            {
                menuToggle = true;
                foreach (var button in buttons)
                {
                    button.SetActive(true);
                }
                StopAllCoroutines();
                StartCoroutine(AnimateTopMenu(true));
            }
        }

        private IEnumerator AnimateTopMenu(bool toggle)
        {
#if UNITY_ANDROID
            buttons[2].gameObject.SetActive(false);
#endif

            float elapsedTime = 0;
            float percentageComplete = 0;

            if (toggle)
            {
                while (topMenu.spacing < -270f)
                {
                    elapsedTime += Time.deltaTime;
                    percentageComplete = elapsedTime / 1.8f;

                    topMenu.spacing = Mathf.Lerp(topMenu.spacing, 10f, percentageComplete);

                    yield return new WaitForFixedUpdate();
                }
            }
            else
            {
                while (topMenu.spacing > -580f)
                {
                    elapsedTime += Time.deltaTime;
                    percentageComplete = elapsedTime / 1.8f;

                    topMenu.spacing = Mathf.Lerp(topMenu.spacing, -580f, percentageComplete);

                    yield return new WaitForFixedUpdate();
                }

                foreach (var button in buttons)
                {
                    button.SetActive(false);
                }
            }
        }

        //.....................AiModeGeneration ...........................//
        //....................Prevent a  Doublet...........................//


        #region _AiModeGeneration
        public void GenerateForAi()
        {
           
            foreach (var slot in Slot.slots)
            {
                slot.HightlightMe(false);
            }

            if (Board.Instance.acceptance >= 2)
            {
                canvasHandler.diceRollButton.SetActive(true);
                canvasHandler.diceResults.SetActive(true);
            }

            if (diceEnable && Board.Instance.acceptance >= 2)
            {
                

                if (turn == 0)
                {
                  //  Debug.Log("Human Turn");
                    dragEnable = true;
                    diceEnable = false;
                    //SoundManager.GetSoundEffect(4, 0.25f);
                    AudioManager.Instance.DiceRoll();
                    CheckIfTurnChange(Random.Range(1,7), Random.Range(1,7));
                }
                else
                {
                    dragEnableCheck = dragEnable;
                    //  Debug.Log("Ai Turn");
                    _allSlotsInts.Clear();
                    allSlots.Clear();
                    topEPawns.Clear();
                    checkExistingPawn.Clear();
                    //SoundManager.GetSoundEffect(4, 0.25f);
                    AudioManager.Instance.DiceRoll();

                    CheckifTurnChangeAI(Random.Range(1,7), Random.Range(1,7));

                }


            }
        }

        #endregion



        #region _CheckifTurnChangeAI

        public void CheckifTurnChangeAI(int dice0, int dice1)      // Load the values ​​rolled by the opponent's dice.
        {
            diceButton.gameObject.SetActive(false);
            isDublet = false;

            dices[0] = dice0;
            dices[1] = dice1;



            diceTexts[0].text = dices[0].ToString();
            diceTexts[1].text = dices[1].ToString();

            //if (dices[0] == dices[1])
            //{
            //    isDublet = true;
            //}

            //StartCoroutine(PawnMoveCoroutine());

            StartCoroutine(DiceThrowAI(dice0, dice1));

        }


        private IEnumerator DiceThrowAI(int dice0, int dice1)
        {

            for (int i = 0; i < 12; i++)
            {
                diceImages[0].sprite = diceFaces[Random.Range(1, 7)];
                diceImages[1].sprite = diceFaces[Random.Range(1, 7)];
                yield return new WaitForSeconds(0.05f);
            }

            diceImages[0].sprite = diceFaces[dice0];
            diceImages[1].sprite = diceFaces[dice1];



            diceTexts[0].text = dices[0].ToString();
            diceTexts[1].text = dices[1].ToString();

            if (dices[0] == dices[1])
                isDublet = true;


            StartCoroutine(PawnMoveCoroutine());

            //if (!CanMove(2))
            //    StartCoroutine(ChangeTurn());
        }

        public void CallDublet()
        {
            StartCoroutine(PawnMoveCoroutine());
        }

        private IEnumerator PawnMoveCoroutine()
        {
            yield return new WaitForSecondsRealtime(1);
            if (!CanMoveAi(2))
            {
                StartCoroutine(ChangeTurn());

            }

            _allSlotsInts.Clear();
            allSlots.Clear();
            topEPawns.Clear();
            checkExistingPawn.Clear();

            if (Slot.slots[25].Height() > 0)
            {
                //........Prison Slot...............//

                CheckPrisonSlot();
            }
            else if (!GameController.GameOver)
            {
                // Debug.Log("Normal Movement");
                //   CheckPrisonSlot();
                SlotNumberForAI();
            }
        }

        #endregion

        #region _RandomSelectAi
        public void SlotNumberForAI()
        {
           //_allSlotsInts.Clear();
           // allSlots.Clear();

            for (int i = 0; i < ePawns.Count; i++)
            {
                Pawn pawn = ePawns[i];
                if (!_allSlotsInts.Contains(ePawns[i].slotNo))
                {
                    _allSlotsInts.Add(ePawns[i].slotNo);
                    allSlots.Add(Slot.slots[ePawns[i].slotNo]);
                }
            }

            for (int i = 0; i < allSlots.Count; i++)
            {
                topEPawns.Add(allSlots[i].GetTopPawn(false));
            }

            SelectRandomEnemy();
        }

        #endregion

        public void SlotNumberForAI2()
        {
          //  _allSlotsInts.Clear();
          //  allSlots.Clear();

            for (int i = 0; i < ePawns.Count; i++)
            {
                Pawn pawn = ePawns[i];
                if (!_allSlotsInts.Contains(ePawns[i].slotNo))
                {
                    _allSlotsInts.Add(ePawns[i].slotNo);
                    allSlots.Add(Slot.slots[ePawns[i].slotNo]);
                }
            }

            for (int i = 0; i < allSlots.Count; i++)
            {
                topEPawns.Add(allSlots[i].GetTopPawn(false));
            }

            SelectRandomEnemy2();
        }


        #region _Check the PrisonSlot

        public void CheckPrisonSlot()
        {
            Slot.slots[25].GetTopPawn(false).Selectimprisoned();
        }
        #endregion


        #region SECONDPRISONCHECK

        public void CheckPrisonSlot2()
        {
           // Debug.Log("Check prison Slot");
            Slot.slots[25].GetTopPawn(false).Selectimprisoned2();  //  if (Pawn.imprisoned && Pawn.imprisonedSide[0] > 0 && Pawn.pawnColor == 0)

        }


        #endregion


        //....................For the Shelter Work.................//


        #region _ShelterManipulation
        public void ShelterManipulation()
        {

            //............................Its For a Dice 1............................//
            int ShelterSlotChck = GameController.dices[0];
            //  int sign = SelectShelterPawn.pawnColor == 0 ? 1 : -1;

            if (Slot.slots[ShelterSlotChck].Height() >= 1 && turn == 1)
            {
                Debug.Log("height greater 1 and turn equal to 1 Work");
                SelectShelterPawn = Slot.slots[ShelterSlotChck].GetTopPawn(false);
                SelectShelterPawn.CheckShelterStage();
                var finalposition = SelectShelterPawn.house.transform.position;
                SelectShelterPawn.transform.DOLocalMove(finalposition, 0.5f);
                SelectShelterPawn.PlaceInShelterAi();
            }


            else if (Slot.slots[ShelterSlotChck].Height() == 0 && turn == 1)
            {
                for (int i = 1; i <= 6; i++)
                {
                    if (Slot.slots[i].Height() >= 1 && turn == 1)
                    {
                        SelectShelterPawn = Slot.slots[i].GetTopPawn(false);
                        int sign = -1;
                        int slot0 = SelectShelterPawn.slotNo + sign * GameController.dices[0];
                        Debug.Log("Shelter Slot" + slot0);
                        if (slot0 >= 1 && slot0 <= 6 && turn == 1)
                        {
                            Debug.Log("Shelter in In between Range");
                            Slot.slots[SelectShelterPawn.slotNo].GetTopPawn(true);
                            Slot.slots[slot0].PlacePawn(SelectShelterPawn, SelectShelterPawn.pawnColor);
                            break;

                        }
                        else
                        {
                            Debug.Log("Dice value is greater than slot No");
                            //SelectShelterPawn = Slot.slots[i].GetTopPawn(false);
                            SelectShelterPawn.CheckShelterStage();
                            var finalposition = SelectShelterPawn.house.transform.position;
                            SelectShelterPawn.transform.DOLocalMove(finalposition, 0.5f);

                            SelectShelterPawn.PlaceInShelterAi();
                            break;
                        }
                    }
                    //       
                }
            }
            SelectShelterPawn.CheckShelterAndMore();
            SelectShelterPawn.CheckIfNextTurn();
        }

#endregion

        #region _SecondDiceForShelterStage
        IEnumerator ShelterSecondDice()
        {
            yield return new WaitForSecondsRealtime(2f);


            int ShelterSlotChck = GameController.dices[1];
            // int sign = SelectShelterPawn2.pawnColor == 0 ? 1 : -1;

            if (Slot.slots[ShelterSlotChck].Height() >= 1 && turn == 1)
            {
            
                SelectShelterPawn2 = Slot.slots[ShelterSlotChck].GetTopPawn(false);
                SelectShelterPawn2.CheckShelterStage();
                var finalposition = SelectShelterPawn2.house.transform.position;
                SelectShelterPawn2.transform.DOLocalMove(finalposition, 0.5f);
                SelectShelterPawn2.PlaceInShelterAi();
            }


            else if (Slot.slots[ShelterSlotChck].Height() == 0 && turn == 1)
            {
                for (int i = 1; i <= 6; i++)
                {
                    if (Slot.slots[i].Height() >= 1 && turn == 1)
                    {
                        SelectShelterPawn2 = Slot.slots[i].GetTopPawn(false);
                        int sign = -1;
                        int slot0 = SelectShelterPawn2.slotNo + sign * GameController.dices[1];
                     
                        if (slot0 >= 1 && slot0 <= 6 && turn == 1)
                        {
                         
                            Slot.slots[SelectShelterPawn2.slotNo].GetTopPawn(true);
                            Slot.slots[slot0].PlacePawn(SelectShelterPawn2, SelectShelterPawn2.pawnColor);
                            break;

                        }
                        else
                        {
                           
                            //SelectShelterPawn = Slot.slots[i].GetTopPawn(false);
                            SelectShelterPawn2.CheckShelterStage();
                            var finalposition = SelectShelterPawn2.house.transform.position;
                            SelectShelterPawn2.transform.DOLocalMove(finalposition, 0.5f);
                            SelectShelterPawn2.PlaceInShelterAi();
                            break;
                        }
                    }
                
                    //       
                }
            }
            //.................Turn Check..........................//
            SelectShelterPawn2.CheckShelterAndMore();
            SelectShelterPawn2.CheckIfNextTurn();

        }
        #endregion




        #region _SelectRandomEnemy
        public void SelectRandomEnemy()
        {
            //.............SelectRandom Enemy From the list.....


            if (Pawn.shelterSide[1] && MyGameManager.AiMode==true)
            {
                //....................Ai Mode Work Only........................//
                ShelterManipulation();
                StartCoroutine(ShelterSecondDice());
            }
            else
            {
                int RandomSelectEnemy = -1;

                switch (MyGameManager.Instance.botDifficulty)
                {
                    case Difficulty.Beginner:
                        RandomSelectEnemy = 0;
                        break;
                    case Difficulty.Intermediate:
                        RandomSelectEnemy = Random.Range(0, topEPawns.Count);
                        break;
                    case Difficulty.GrandMaster:
                        RandomSelectEnemy = topEPawns.Count - 1;
                        break;
                }



                randomSelectPawn = topEPawns[RandomSelectEnemy];
                checkExistingPawn.Add(randomSelectPawn);
                topEPawns.Remove(randomSelectPawn);
                AiMovesEnemy();
            }
             

           }
        #endregion



        #region _SelectRandomEnemy
        public void SelectRandomEnemy2()
        {
            //.............SelectRandom Enemy............From the list.....
            int RandomSelectEnemy = -1;

            switch (MyGameManager.Instance.botDifficulty)
            {
                case Difficulty.Beginner:
                    RandomSelectEnemy = 0;
                    break;
                case Difficulty.Intermediate:
                    RandomSelectEnemy = Random.Range(0, topEPawns.Count);
                    break;
                case Difficulty.GrandMaster:
                    RandomSelectEnemy = topEPawns.Count - 1;
                    break;
            }


         //   int RandomSelectEnemy = Random.Range(0, topEPawns.Count);

            randomSelectPawn2 = topEPawns[RandomSelectEnemy];
            checkExistingPawn.Add(randomSelectPawn2);
            topEPawns.Remove(randomSelectPawn2);

            AiMovesEnemy2();

        }
        #endregion

        #region _AIEnemyMoves
        public void AiMovesEnemy()
        {
            int sign = randomSelectPawn.pawnColor == 0 ? 1 : -1;
           // Debug.Log("SlotNo" + randomSelectPawn.slotNo);
            int slot0 = randomSelectPawn.slotNo + sign * GameController.dices[0];
            int slot1 = randomSelectPawn.slotNo + sign * GameController.dices[1];
          //  Debug.Log("turn" + turn);

            if (GameController.turn == randomSelectPawn.pawnColor)
            {
                if (slot0 > 0 && slot0 < 25 && slot0 != randomSelectPawn.slotNo)
                {
                    //randomSelectPawn.CheckShelterStage();
                    if (Slot.slots[slot0].Height() >= 1 && Slot.slots[slot0].IsWhite() == randomSelectPawn.pawnColor)
                    {
                        GameController.Instance.playerScores[1].Moves++;
                        Slot.slots[randomSelectPawn.slotNo].GetTopPawn(true);
                        Slot.slots[slot0].PlacePawn(randomSelectPawn, randomSelectPawn.pawnColor);
                        randomSelectPawn.CheckShelterStage();
                        randomSelectPawn.CheckShelterAndMore();
                        Slot.slots[slot0].HightlightMe(true);
                        randomSelectPawn.CheckIfNextTurn();
                        StartCoroutine(SecondDice());
                    }

                    //.............If pawn Slot is Empty or Height ==0...................
                  else  if (Slot.slots[slot0].Height() == 0)
                    {
                        GameController.Instance.playerScores[1].Moves++;
                        Slot.slots[randomSelectPawn.slotNo].GetTopPawn(true);
                        Slot.slots[slot0].PlacePawn(randomSelectPawn, randomSelectPawn.pawnColor);
                        randomSelectPawn.CheckShelterStage();
                        randomSelectPawn.CheckShelterAndMore();
                        Slot.slots[slot0].HightlightMe(true);
                        randomSelectPawn.CheckIfNextTurn();
                        StartCoroutine(SecondDice());
                    }


                  else if (Slot.slots[slot0].Height() == 1 && Slot.slots[slot0].IsWhite() != randomSelectPawn.pawnColor)
                    {
                        GameController.Instance.playerScores[1].Kills++;
                        //.......JAIL KAR Dooh...........//
                      
                        Slot.slots[randomSelectPawn.slotNo].GetTopPawn(true);
                        Slot.slots[slot0].GetTopPawn(false).PlaceJail();
                        Slot.slots[slot0].PlacePawn(randomSelectPawn, randomSelectPawn.pawnColor);

                        randomSelectPawn.CheckShelterStage();
                        randomSelectPawn.CheckShelterAndMore();
                        randomSelectPawn.CheckIfNextTurn();
                        StartCoroutine(SecondDice());
                    }

                    else if(Slot.slots[slot0].Height() > 1 && Slot.slots[slot0].IsWhite() != randomSelectPawn.pawnColor)
                    {
                     
                        if (topEPawns.Count == 0)
                        {
                            //.............. Shift the TURN tO THE HUMAN..................//
                            Debug.Log("Turn on First SLOT");
                            StartCoroutine(SecondDice());
                            Pawn_OnCompleteTurn(turn);

                        }
                        else
                        {

                            SelectRandomEnemy();

                        }
                    }

                }
                else
                {
                    //............Call it Again your Ai................//
                  
                    SelectRandomEnemy();
                }
            }
                
        }
        #endregion


        #region _AIEnemyMoves
        public void AiMovesEnemy2()
        {
            turn = 1;
            int sign = randomSelectPawn2.pawnColor == 0 ? 1 : -1;
            int slot1 = randomSelectPawn2.slotNo + sign * GameController.dices[1];

         
            if (GameController.turn == randomSelectPawn2.pawnColor)
            {
              
                if (slot1 > 0 && slot1 < 25 && slot1 != randomSelectPawn2.slotNo)
                {
                    //randomSelectPawn.CheckShelterStage();
                    if (Slot.slots[slot1].Height() >= 1 && Slot.slots[slot1].IsWhite() == randomSelectPawn2.pawnColor)
                    {
                        GameController.Instance.playerScores[1].Moves++;
                        Slot.slots[randomSelectPawn2.slotNo].GetTopPawn(true);
                        Slot.slots[slot1].PlacePawn(randomSelectPawn2, randomSelectPawn2.pawnColor);
                        randomSelectPawn2.CheckShelterStage();
                        randomSelectPawn2.CheckShelterAndMore();
                        Slot.slots[slot1].HightlightMe(true);
                        randomSelectPawn2.CheckIfNextTurn();

                    }

                    //.............If pawn Slot is Empty or Height ==0...................
                  else  if (Slot.slots[slot1].Height() == 0)
                    {
                        GameController.Instance.playerScores[1].Moves++;
                   
                        Slot.slots[randomSelectPawn2.slotNo].GetTopPawn(true);
                        Slot.slots[slot1].PlacePawn(randomSelectPawn2, randomSelectPawn2.pawnColor);
                        randomSelectPawn2.CheckShelterStage();
                        randomSelectPawn2.CheckShelterAndMore();
                        Slot.slots[slot1].HightlightMe(true);
                        randomSelectPawn2.CheckIfNextTurn();

                    }


                    else if (Slot.slots[slot1].Height() == 1 && Slot.slots[slot1].IsWhite() != randomSelectPawn2.pawnColor)
                    {
                        GameController.Instance.playerScores[1].Kills++;
                        //.......JAIL KAR Dooh...........//
                      
                        // var TopSelectOff= randomSelectPawn2.slotNo;
                        Slot.slots[randomSelectPawn2.slotNo].GetTopPawn(true);
                        
                        Slot.slots[slot1].GetTopPawn(false).PlaceJail();
                        Slot.slots[slot1].PlacePawn(randomSelectPawn2, randomSelectPawn2.pawnColor);
                        randomSelectPawn2.CheckShelterStage();
                        randomSelectPawn2.CheckShelterAndMore();
                        randomSelectPawn2.CheckIfNextTurn();
                        _allSlotsInts.Clear();
                        allSlots.Clear();
                        topEPawns.Clear();
                       // SlotNumberForAI();


                    }
                    else if(Slot.slots[slot1].Height() > 1 && Slot.slots[slot1].IsWhite() != randomSelectPawn2.pawnColor)
                    {
                        if (topEPawns.Count == 0)
                        {
                          
                            //..............Assign a Dice to the next human player.......................//

                            Pawn_OnCompleteTurn(turn);
                            _allSlotsInts.Clear();
                            allSlots.Clear();
                            topEPawns.Clear();
                            checkExistingPawn.Clear();
                           
                        }
                        else
                        {
                            //_allSlotsInts.Clear();
                            //allSlots.Clear();
                          //  topEPawns.Clear();
                          
                            SelectRandomEnemy2();
                        }

                    }

                }
                else
                {
                   
                       
                        SelectRandomEnemy2();
                    
                }
            }

        }
        #endregion

        public void PrisonIenumertor()
        {
            StartCoroutine(SecondDice());
        }
        #region
        public IEnumerator SecondDice()
        {
           
            yield return new WaitForSecondsRealtime(1f);
            topEPawns.Clear();
            checkExistingPawn.Clear();
            _allSlotsInts.Clear();
            allSlots.Clear();
         
            //yield return new WaitForSecondsRealtime(1f);
         
            //yield return new WaitForSecondsRealtime(1.5f);
         
            if (Slot.slots[25].Height() > 0)
            {
              //  Debug.Log("Check Prison Slot 2");
                //........Prison Slot...............//

                CheckPrisonSlot2();
            }
            else
            {
               // Debug.Log("Normal Moves two");
                SlotNumberForAI2();
            }
        }

        #endregion
        public void CheckIfTurnChange(int dice0, int dice1)      // Load the values ​​rolled by the opponent's dice.
        {

            //if (NetworkTurn == true)
            //{
            //    ///.....Check it...........
            //}
            diceButton.gameObject.SetActive(false);
            isDublet = false;

            dices[0] = dice0;
            dices[1] = dice1;

            //diceTexts[0].text = dices[0].ToString();
           // diceTexts[1].text = dices[1].ToString();

            //if (dices[0] == dices[1])
            //    isDublet = true;

            StartCoroutine(DiceThrow(dice0, dice1));

            //  if (!CanMove(2))
            //StartCoroutine(ChangeTurn());
        }


        private IEnumerator DiceThrow(int dice0, int dice1)
        {
          

            for (int i = 0; i < 12; i++)
            {
                diceImages[0].sprite = diceFaces[Random.Range(1, 7)];
                diceImages[1].sprite = diceFaces[Random.Range(1, 7)];
                yield return new WaitForSeconds(0.05f);
            }

            diceImages[0].sprite = diceFaces[dice0];
            diceImages[1].sprite = diceFaces[dice1];



            diceTexts[0].text = dices[0].ToString();
            diceTexts[1].text = dices[1].ToString();

            if (dices[0] == dices[1])
                isDublet = true;

        



       

            if (!CanMove(2))
                StartCoroutine(ChangeTurn());
        }

        private IEnumerator ChangeTurn()
        {
            yield return new WaitForSeconds(2f);
            Pawn_OnCompleteTurn(turn);
        }

        public void Pawn_OnCompleteTurn(int isWhiteColor)
        {
            dices[0] = dices[1] = 0;

            diceEnable = true;
            dragEnable = false;

            turn = 1 - turn;                                                // turn change

            turnImages[0].gameObject.SetActive(1 - isWhiteColor == 0);
            turnImages[1].gameObject.SetActive(isWhiteColor == 0);

            if (Board.Instance.client)      // network game
            {
                newTurn = true;

                if (Board.Instance.isClientWhite == (turn == 0 ? true : false))    // if we are white (red) and the turn is white (red)
                    diceButton.gameObject.SetActive(true);
            }
            else


            if (turn == 1 && MyGameManager.AiMode == true)
            {
                //GenerateForAi();
                StartCoroutine(TurnChangeDelay());
            }
            else
            {
                diceButton.gameObject.SetActive(true);                // offline game

            }
        }

        private IEnumerator TurnChangeDelay()
        {
            yield return new WaitForSeconds(1f);
            GenerateForAi();
        }

        private void Pawn_OnGameOver(bool isWhite)
        {
            GameOver = true;
            gameOverPanel.SetActive(true);
           // gameOverPanel.GetComponentInChildren<Text>().text = "Winner: " + (isWhite ? "white" : "red");

            int winner;

            if (isWhite)
            {
               winner = 0;
            }
            else
            {
                winner = 1;
            }
            
            ActiveGameOver(winner);
        }

        public void ActiveGameOver(int winner)
        {
            Board.Instance.isGameOver = true;
            MyGameManager.AiMode = false;

            if (MyGameManager.AiMode == false)
            {
                AiDifficulty.gameObject.SetActive(false);
                AiDifficulty2.gameObject.SetActive(false);
                AiDifficulty3.gameObject.SetActive(false);

            }


            if (winner == 0)
            {
                YouWinPanel.gameObject.SetActive(true);
                AudioManager.Instance.GameWon();

                difficultyTextYouWinPanel.LocalizationKey = LobbyCanvas.Instance.difficulty.ToString();
                LanguageManager.OnVariableChanged();

            }
            else
            {
                gameOverPanel.gameObject.SetActive(true);
                AudioManager.Instance.GameLost();

                difficultyTextGameOverPanel.LocalizationKey = LobbyCanvas.Instance.difficulty.ToString();
                LanguageManager.OnVariableChanged();
            }

        }

        public void ActivatePausePanel()
        {

            difficultyTextPausePanel.LocalizationKey = LobbyCanvas.Instance.difficulty.ToString();
            LanguageManager.OnVariableChanged();

            Board.Instance.isPaused = true;
        }

        public void ResumeGame()
        {
            Board.Instance.isPaused = false;
        }

        public  void NewGame()
        {
            if (Board.Instance.client)
            {
                sidesAgreed++;

                string msg = "CNG|" + "OpponentAgreed";
                Board.Instance.client.Send(msg);            // Send the information about the consent to the next game to the server.

                if (!Board.Instance.client.isHost)
                    newGameButton.interactable = false;     // deactivate after each use
            }
            else
                LoadGameScene();

            //SoundManager.GetSoundEffect(4, 0.25f);
        }

        private void LoadGameScene()
        {
            Debug.Log("LoadGameScene");

            sidesAgreed = 0;
            GameOver = false;
            isDublet = false;
            dragEnable = false;
            turn = 0;
            Pawn.InitializePawn();
            SceneManager.UnloadSceneAsync(1);
            SceneManager.LoadScene(1);
        }

        public void GoToMainMenu()     // delete the Backgammon scene and show the Lobby scene main menu
        {
            StartCoroutine(DelayedGoToMainMenu());
        }

        private IEnumerator DelayedGoToMainMenu()
        {
            MyGameManager.AiMode = false;

            if (MyGameManager.Instance)
            {
                MyGameManager.Instance.botDifficulty = Difficulty.None;
            }

          //  SoundManager.GetSoundEffect(4, 0.25f);

            yield return new WaitForSeconds(0.2f);

            LobbyManager.Instance.RemoveNetworkParts();
            SceneManager.LoadScene(0);
        }

        //---------------- checking the possibility of making a move -----------------------------------------------------------------

        public static bool CanMove(int amount)     // detect the situation when it is not possible to make a move
        {
            int count = 0;
            int sign = turn == 0 ? 1 : -1;
            int value = turn == 0 ? 24 : -1;

            if (Pawn.imprisonedSide[turn] > 0)                  // while they are in jail
                return CanMoveFromJail(amount, count, sign);
            else                                                // when they are not in jail
            {
                if (Pawn.shelterSide[turn])
                {
                    Debug.Log("Shelter Side");
                    return CanMoveInShelter(value, sign);
                }                  // when the mode of entering the shelter
                else if (CanMoveFree(value, sign))    // we go through each slot with white pieces and check if it is possible to make a move from this slot
                    return true;
            }

            return false;
        }


        //...............................Can Move Ai.......................//


        #region _CanMoveAi
        public static bool CanMoveAi(int amount)     // detect the situation when it is not possible to make a move
        {
            int count = 0;
            int sign = turn == 0 ? 1 : -1;
            int value = turn == 0 ? 24 : -1;

            if (Pawn.imprisonedSide[turn] > 0)
            {
                //................When You are in the Jail.............//
                Debug.Log("Can Move from Jail");
                return CanMoveFromJail(amount, count, sign);
            }
            else                                                // when they are not in jail
            {
                if (Pawn.shelterSide[turn])
                {  //............. when the mode of entering the shelter..................//
                  
                    return CanMoveInShelter(value, sign);
                }
                else if (CanMoveFree(value, sign))
                {
                   // Debug.Log("CanMoveFree");
                    // we go through each slot with white pieces and check if it is possible to make a move from this slot
                    return true;
                }
            }

            return false;
        }

        #endregion
        private static bool CanMoveFromJail(int amount, int count, int sign)
        {
        
         
            int val = turn == 0 ? -1 : 24;

            for (int i = 0; i < 2; i++)
                if (dices[i] != 0)
                    if (Slot.slots[(val + 1) + sign * dices[i]].Height() > 1 && Slot.slots[(val + 1) + sign * dices[i]].IsWhite() != turn)
                        count++;

            return !(count == amount);
        }

        private static bool CanMoveFree(int value, int sign)
        {
            Debug.Log("Can Move Free");
            for (int i = 1; i <= 24; i++)
                if (Slot.slots[i].Height() > 0 && Slot.slots[i].IsWhite() == turn)   // slot with whites
                    for (int j = 0; j < 2; j++)
                        if (dices[j] != 0 && dices[j] + sign * i <= value)
                            if (Slot.slots[i + sign * dices[j]].Height() < 2)
                                return true;
                            else if (Slot.slots[i + sign * dices[j]].Height() > 1 && Slot.slots[i + sign * dices[j]].IsWhite() == turn)
                                return true;

            return false;
        }

        private static bool CanMoveInShelter(int value, int sign)
        {
            int endSlotNo = turn == 0 ? 19 : 6;
            int first = 0;

            for (int j = 0; j < 6; j++)                                             // after successive slots of the last quarter
            {
                if (endSlotNo + sign * j >= 0)
                {
                    if (Slot.slots[endSlotNo + sign * j].Height() > 0)              // if there is a pawn on the slot
                    {
                        if (Slot.slots[endSlotNo + sign * j].IsWhite() == turn)     // if this pawn is ours
                        {
                            //Debug.Log("slot no: " + (endSlotNo + sign * (j - sign)) + ", height: " + Slot.slots[endSlotNo + sign * j].Height());

                            for (int i = 0; i < 2; i++)
                            {
                                if (dices[i] > 0)
                                {
                                    //Debug.Log((endSlotNo + sign * j) + " + " + (sign * dices[i]) + " : " + (value + 1));

                                    int ind = endSlotNo + sign * (j + dices[i]);

                                    if (ind == value + 1)           // pawns from any slot
                                    {
                                        return true;
                                    }

                                    if (first == 0)                 // only for pawns from the extreme slot
                                    {
                                        if (value == 24)
                                        {
                                            if (ind > value + 1)    // when the sum is greater than 25
                                            {
                                                return true;
                                            }
                                        }

                                        if (value == -1)
                                        {
                                            if (ind < value + 1)   // when the sum is less than 0
                                            {
                                                return true;
                                            }
                                        }
                                    }

                                    if (ind >= 0 && ind < Slot.slots.Count)             // index must be non negative
                                    {
                                        if (Slot.slots[ind].Height() > 0)               // if there are pieces on the slot
                                        {
                                            if (Slot.slots[ind].IsWhite() != turn)      // if there is an opponent's pawn on the target slot
                                            {
                                                if (Slot.slots[ind].Height() < 2)       // if there is no wall on the destination slot
                                                {
                                                    return true;
                                                }
                                            }

                                            if (Slot.slots[ind].IsWhite() == turn)      // if there is a pawn on the target slot
                                            {
                                                return true;
                                            }
                                        }
                                        else                                            // if there are no pawns in the slot
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }

                            first++;
                        }
                    }
                }
            }

            return false;
        }

        [ContextMenu("Shelter Check")]
        public void ShelterSideMovesTowardHomeAi()
        {
            if (Pawn.shelterSide[1])
            {
                var slots1to6 = Slot.slots.Where(t => t.slotNo >= 1 && t.slotNo <= 6).ToList();
                for(int i = 0; i < slots1to6.Count; i++)
                {
                    shelterPawn.Add(slots1to6[i]);
                }
               
            }
        }
    }

    [Serializable]
    public class PlayerScore
    {
        #region Score
       
        private int _score;
        private int _moves;
        private int _kills;
        private int _shelter;
        private int _time;

        public int Score
        {
            get => _score;
            set
            {
                _score = 2 * _moves + 20 * _shelter + 10 * _kills - (_time / 30);
                if (_score < 0) _score = 0;

                if (GameController.turn == 0)
                {

                GameController.Instance.scoreTextPausePanel.variableText = _score.ToString();
                LanguageManager.OnVariableChanged();
                GameController.Instance.scoreTextgameOverPausePanel.variableText = _score.ToString();
                LanguageManager.OnVariableChanged();
                GameController.Instance.scoreTextyouWinPausePanel.variableText = _score.ToString();
                LanguageManager.OnVariableChanged();


                ///////////////////////////////////////////////////////////////////////////////////////////////////
                GameController.Instance.scoreTextPlayer1PlayerVsPlayerPausePanel.variableText = _score.ToString();
                LanguageManager.OnVariableChanged();
                GameController.Instance.scoreTextPlayer1VsPlayerGameOverPausePanel.variableText = _score.ToString();
                LanguageManager.OnVariableChanged();
                GameController.Instance.scoreTextPlayer1VsPlayerYouWinPausePanel.variableText = _score.ToString();
                LanguageManager.OnVariableChanged();


                }
                else if(GameController.turn == 1)
                {
                    ///////////////////////////////////////////////////////////////////////////////////////////////////
                    GameController.Instance.scoreTextPlayer2PlayerVsPlayerPausePanel.variableText = _score.ToString();
                    LanguageManager.OnVariableChanged();
                    GameController.Instance.scoreTextPlayer2VsPlayerGameOverPausePanel.variableText = _score.ToString();
                    LanguageManager.OnVariableChanged();
                    GameController.Instance.scoreTextPlayer2VsPlayerYouWinPausePanel.variableText = _score.ToString();
                    LanguageManager.OnVariableChanged();
                }





                if (GameController.turn == 0)
                {
                  
                    GameController.Instance.player0Points.variableText= _score.ToString();
                    LanguageManager.OnVariableChanged();
                }
                else
                {                
                   GameController.Instance.player1Points.variableText = _score.ToString();
                   LanguageManager.OnVariableChanged();
                }


            }
        }

        public int Moves
        {
            get => _moves;
            set
            {
                _moves = value;
                Score++;
            }
        }

        public int Kills
        {
            get => _kills;
            set
            {
                _kills = value;
                Score++;
            }
        }

        public int Shelter
        {
            get => _shelter;
            set
            {
                _shelter = value;
                Score++;
            }
        }

        public int TotalTime
        {
            get => _time;
            set
            {
                _time = value;
                Score++;
            }
        }

        #endregion
    }
}

