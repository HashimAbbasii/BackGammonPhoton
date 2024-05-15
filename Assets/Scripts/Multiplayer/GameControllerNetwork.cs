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
using System;
using Random = UnityEngine.Random;
using Assets.SimpleLocalization.Scripts;
using Photon.Realtime;

namespace BackgammonNet.Core
{
    // Mechanisms of generating two random numbers and checking the possibility of making a move.

    public class GameControllerNetwork : MonoBehaviourPunCallbacks
    {
        private PhotonView _photonView;

        public LocalizedTextTMP difficultyTextGameOverPanel;
        public LocalizedTextTMP difficultyTextYouWinPanel;
        public LocalizedTextTMP difficultyTextPausePanel;

        [Header("Panels")]
        public GameObject GameOverPanel;
        public GameObject YouWinPanel;
        public GameObject YouWinPanelPlayerLeftPanel;

        public CanvasHandlerNetwork canvasHandlerNetwork;

        [SerializeField] private Image[] diceImages;
        public List<Sprite> diceFaces = new List<Sprite>();


        public HorizontalLayoutGroup topMenu;
        public List<GameObject> buttons;
        public bool menuToggle;
        

        public PawnNetwork randomSelectPawn;
        public PawnNetwork randomSelectPawn2;
        public static int turn;                     // indicates whose turn it is now
        public static int[] dices = new int[2];     // recently drawn numbers
        public static bool isDublet;                // whether a doublet was thrown
        public static bool dragEnable;              // is it possible to drag the pieces

        [HideInInspector] public bool newTurn;
        [HideInInspector] public int sidesAgreed;   // the current number of players agreeing to continue the game

        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject newGameInfoPanel;
        [SerializeField] private Button diceButton;
        [SerializeField] private Image[] turnImages;
        [SerializeField] private Text[] diceTexts;


       
        public List<PawnNetwork> allPawns = new();
        public List<int> _allSlotsInts = new();
        public List<PawnNetwork> topEPawns = new();
        public List<PawnNetwork> checkExistingPawn = new();
        public List<PawnNetwork> ePawns = new();
        public bool NetworkTurn=true;

        public bool diceEnable = true;             // permission to roll the dice (after making moves)

        private bool timeSetOnlyOnce = false;
        public DateTime startDateTime;

        public static GameControllerNetwork Instance { get; set; }
        public static bool GameOver { get; set; }

        private void Awake()
        {
            Instance = this;

            diceButton.gameObject.SetActive(false);
            _photonView = GetComponent<PhotonView>();

            PawnNetwork.OnCompleteTurn += PawnOnCompleteTurn;
            PawnNetwork.OnGameOver += Pawn_OnGameOver;
            TimeController.OnTimeLimitEnd += Pawn_OnGameOver;

            mainMenuButton.onClick.AddListener(GoToMainMenu);
            newGameButton.onClick.AddListener(NewGame);

            

            if (MyPhotonManager.instance.multiPlayerMode == true)
            {
                Debug.Log("GN");

                diceButton.onClick.AddListener(GenerateNetworks);


            }
            


            turn = 0;

            turnImages[0].gameObject.SetActive(turn == 0);
            turnImages[1].gameObject.SetActive(1 - turn == 0);

            
        }

        private void Start()
        {
            //if (Board.Instance.client)
            //    if (!Board.Instance.isClientWhite)
            //        diceButton.gameObject.SetActive(false);

            //if (Board.Instance.observer)     // lock buttons
            //    ActivateButtons(false);

            AudioManager.Instance.PlayGameMusic();
            if (MyPhotonManager.instance.multiPlayerMode == true)
            {
                StartCoroutine(NetworkButton());
            
            }
        }

        private bool YourTurn()
        {
            return turn == int.Parse(PhotonNetwork.NickName);
        }

        IEnumerator NetworkButton()
        {
            //diceButton.enabled = false;
            diceButton.gameObject.SetActive(false);
            yield return new WaitForSeconds(2f);

            if (MyPhotonManager.instance.multiPlayerMode == true)
            {
                if (YourTurn())
                {
                    //diceButton.gameObject.SetActive(true);


                    //_photonView.RPC(nameof(DiceEnbleForMaster), RpcTarget.AllBuffered);
                }


              // _photonView.RPC(nameof(SlotButtonDisable), RpcTarget.AllBuffered);
         
            }
        }


       // [PunRPC]
        
        
        [PunRPC]

        public void DiceEnbleForMaster()
        {
            diceButton.gameObject.SetActive(true);
            diceButton.enabled = true;
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
            _photonView.RPC(nameof(ClientTurnStarted), RpcTarget.AllBuffered);
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
            PawnNetwork.OnCompleteTurn -= PawnOnCompleteTurn;
            PawnNetwork.OnGameOver -= Pawn_OnGameOver;
            TimeController.OnTimeLimitEnd -= Pawn_OnGameOver;
        }
        public bool timeCounter = false;
        public Text counterTime;
        private void Update()
        {
            if (sidesAgreed == 2)
                LoadGameScene();
            
            if (timeCounter) UpdateTimeText();


            TryDeactivateDigit();
        }

        private float currentTime = 0f;
        

        void UpdateTimeText()
        {
            //Debug.Log("Timer Running");
            //int minutes = Mathf.FloorToInt(currentTime / 60);
            //int seconds = Mathf.FloorToInt(currentTime % 60);
            // counterTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            //counterTime.text = minutes.ToString("00") + ":" + seconds.ToString("00");

            counterTime.text = string.Format("{0:mm\\:ss}", DateTime.Now - startDateTime);
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
            if (diceEnable && Board.Instance.acceptance >= 2)
            {
                dragEnable = true;
                diceEnable = false;

                //SoundManager.GetSoundEffect(4, 0.25f);
                AudioManager.Instance.DiceRoll();

                CheckIfTurnChange(1, 3);

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

                //if (Board.Instance.client)      // network game
                //{
                //    newTurn = false;

                //    string msg = "CDCS|" + dices[0].ToString() + "|" + dices[1].ToString();
                //    Board.Instance.client.Send(msg);            // Send the information about the roll of the dice to the server.
                //}
            }
        }

        public void MenuButtonToggle()
        {
            Debug.Log("Menu Button Toggle");
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

        #region _NetworkGeneration

        public void GenerateNetwork()
        {
           
            if (diceEnable)
            {
                if (turn == 0)
                {
                    dragEnable = true;
                    diceEnable = false;
                    // SoundManager.GetSoundEffect(4, 0.25f);
                    AudioManager.Instance.DiceRoll();
                    CheckIfTurnChange(Random.Range(1, 7), Random.Range(1, 7));
                }
            }

        }


        #endregion



        [PunRPC]
        public void TimerSetActive(int viewID)
        {
            if (PhotonView.Find(viewID).GetComponent<GameControllerNetwork>().timeSetOnlyOnce) return;

            PhotonView.Find(viewID).GetComponent<GameControllerNetwork>().timeCounter = true;
            PhotonView.Find(viewID).GetComponent<GameControllerNetwork>().startDateTime = DateTime.Now;
            PhotonView.Find(viewID).GetComponent<GameControllerNetwork>().timeSetOnlyOnce = true;
        }

        public void GenerateNetworks()
        {
            //timeCounter = true;


            _photonView.RPC(nameof(TimerSetActive),RpcTarget.AllBuffered, _photonView.ViewID);

            Debug.Log("Button Press");

            if (diceEnable)
            {
                Debug.Log("Button Press DE");
                if (turn == 0)
                {
                    Debug.Log("Button Press T0");
                    //  Debug.Log("Human Turn");
                    dragEnable = true;
                    diceEnable = false;
                    // SoundManager.GetSoundEffect(4, 0.25f);
                    AudioManager.Instance.DiceRoll();
                    CheckIfTurnChange(Random.Range(1, 7), Random.Range(1, 7));
                }
                else
                {
                    Debug.Log("Button Press T1");
                    _allSlotsInts.Clear();
                 
                    topEPawns.Clear();
                    checkExistingPawn.Clear();
                    //SoundManager.GetSoundEffect(4, 0.25f);
                    AudioManager.Instance.DiceRoll();
                    CheckIfTurnChange(Random.Range(1, 7), Random.Range(1, 7));

                }


            }
        }



        // #region _CheckifTurnChangeAI

        public void CheckifTurnChangeAI(int dice0, int dice1)      // Load the values ​​rolled by the opponent's dice.
        {
            diceButton.gameObject.SetActive(false);
            isDublet = false;

            dices[0] = dice0;
            dices[1] = dice1;

            diceTexts[0].text = dices[0].ToString();
            diceTexts[1].text = dices[1].ToString();

            if (dices[0] == dices[1])
            {

                isDublet = true;
            }

            if (!CanMoveAi(2))
            {
                StartCoroutine(ChangeTurn());

            }

        }

        public void CheckIfTurnChange(int dice0, int dice1)      // Load the values ​​rolled by the opponent's dice.
        {

           
            diceButton.gameObject.SetActive(false);
            isDublet = false;
           
            _photonView.RPC(nameof(TextShow), RpcTarget.AllBuffered, dice0, dice1);
            if (dices[0] == dices[1])
                isDublet = true;

            if (!CanMove(2))
                StartCoroutine(ChangeTurn());
        }

        [PunRPC]
        public void TextShow(int dice0, int dice1)
        {
            dices[0] = dice0;
            dices[1] = dice1;

            //for (int i = 0; i < 12; i++)
            //{
            //    diceImages[0].sprite = diceFaces[Random.Range(1, 7)];
            //    diceImages[1].sprite = diceFaces[Random.Range(1, 7)];
            //}

            diceImages[0].sprite = diceFaces[dice0];
            diceImages[1].sprite = diceFaces[dice1];

            diceTexts[0].text = dices[0].ToString();
            diceTexts[1].text = dices[1].ToString();

            canvasHandlerNetwork.diceResults.SetActive(true);
        }


        private IEnumerator ChangeTurn()
        {
            yield return new WaitForSeconds(2f);
            PawnOnCompleteTurn(turn);

        }

        public void PawnOnCompleteTurn(int isWhiteColor)
        {
            _photonView.RPC(nameof(Pawn_OnCompleteTurnRPC), RpcTarget.AllBuffered, isWhiteColor);
        }

        [PunRPC]
        public void Pawn_OnCompleteTurnRPC(int isWhiteColor)
        {
            diceButton.gameObject.SetActive(false);
            dices[0] = dices[1] = 0;

            diceEnable = true;
            dragEnable = false;

            turn = 1 - turn;                                                // turn change

            turnImages[0].gameObject.SetActive(1 - isWhiteColor == 0);
            turnImages[1].gameObject.SetActive(isWhiteColor == 0);

            if (YourTurn())
            {
                diceButton.gameObject.SetActive(true);
            }

            //if (BoardNetwork.Instance.client)      // network game
            //{
            //    newTurn = true;

            //    if (BoardNetwork.Instance.isClientWhite == (turn == 0 ? true : false))    // if we are white (red) and the turn is white (red)
            //        diceButton.gameObject.SetActive(true);
            //}
            //else


            //if (turn == 1 && LobbyManager.AiMode == true)
            //{
            //    //GenerateForAi();
            //}
            //else
            //{
            //    diceButton.gameObject.SetActive(true);                // offline game

            //}
        }

        private void Pawn_OnGameOver(bool isWhite)
        {
            GameOver = true;
            gameOverPanel.SetActive(true);
            gameOverPanel.GetComponentInChildren<Text>().text = "Winner: " + (isWhite ? "white" : "red");

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

            if (winner == 0)
            {
                //difficultyTextYouWinPanel.variableText = difficulty;
                //LanguageManager.OnVariableChanged();
                YouWinPanel.gameObject.SetActive(true);
                //difficultyTextGameOverPanel.LocalizationKey = LobbyCanvas.Instance.difficulty.ToString();
                // LanguageManager.OnVariableChanged();

                AudioManager.Instance.GameWon();
            }
            else
            {
                // difficultyTextGameOverPanel.variableText = difficulty;
                // LanguageManager.OnVariableChanged();
                gameOverPanel.gameObject.SetActive(true);
                //  difficultyTextYouWinPanel.LocalizationKey = LobbyCanvas.Instance.difficulty.ToString();
                //  LanguageManager.OnVariableChanged();

                AudioManager.Instance.GameLost();
            }

        }

        public void ActivatePausePanel()
        {
            // difficultyTextPausePanel.variableText = difficulty;
            // LanguageManager.OnVariableChanged();

          //  difficultyTextPausePanel.LocalizationKey = LobbyCanvas.Instance.difficulty.ToString();
           // LanguageManager.OnVariableChanged();
        }

        public void NewGame()
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

           // SoundManager.GetSoundEffect(4, 0.25f);
        }

        private void LoadGameScene()
        {
            Debug.Log("LoadGameScene");

            sidesAgreed = 0;
            GameOver = false;
            isDublet = false;
            dragEnable = false;
            turn = 0;
            PawnNetwork.InitializePawn();
            SceneManager.UnloadSceneAsync(1);
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
        }

        public void GoToMainMenu()     // delete the Backgammon scene and show the Lobby scene main menu
        {
            StartCoroutine(DelayedGoToMainMenu());
        }

        private IEnumerator DelayedGoToMainMenu()
        {

          //  SoundManager.GetSoundEffect(4, 0.25f); // Play sound effect

            // Disconnect from Photon network
            PhotonNetwork.Disconnect();

            // Wait until disconnected
            while (PhotonNetwork.IsConnected)
            {
                yield return null;
            }

            // Leave the room if currently in one
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();

                // Wait for leave room operation to complete
                while (PhotonNetwork.InRoom)
                {
                    yield return null;
                }
            }
            SceneManager.LoadScene(0);

            // Load the main menu scene via RPC
            //photonView.RPC(nameof(LoadScene), RpcTarget.AllBuffered);
            // Load the main menu scene

        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            YouWinPanelPlayerLeftPanel.gameObject.SetActive(true);
            //base.OnPlayerLeftRoom(otherPlayer);
        }

        public override void OnLeftRoom()
        {
            //YouWinPanel.gameObject.SetActive(true);
            // This method is called after the local player leaves the room
           // SceneManager.LoadScene(0);
            //photonView.RPC(nameof(LoadScene), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void LoadScene()
        {
            SceneManager.LoadScene(0);
        }

        public  void LeaveRoomAndReturnToLobby()
        {
            // Ensure we're connected to the network
            if (!PhotonNetwork.IsConnected)
                return;

            // If we're the master client, inform all clients to leave the room
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(RPCLeaveRoom), RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        private void RPCLeaveRoom()
        {

            // Leave the room
            PhotonNetwork.LeaveRoom();
            while (PhotonNetwork.InRoom)
            {

            }
            SceneManager.LoadScene(0);
        }
       
        //---------------- checking the possibility of making a move -----------------------------------------------------------------

        public static bool CanMove(int amount)     // detect the situation when it is not possible to make a move
        {
            int count = 0;
            int sign = turn == 0 ? 1 : -1;
            int value = turn == 0 ? 24 : -1;

            if (PawnNetwork.imprisonedSide[turn] > 0)
            // while they are in jail 
            {
                Debug.Log("Imprisoned Slot");
                return CanMoveFromJail(amount, count, sign);

            }
            else                                               // when they are not in jail
            {
                if (PawnNetwork.shelterSide[turn])
                {
                    
                    return CanMoveInShelter(value, sign);
                }                 // when the mode of entering the shelter
                else if (CanMoveFree(value, sign))
                {
                    Debug.Log("Moves Free");
                    return true;
                }  // we go through each slot with white pieces and check if it is possible to make a move from this slot
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

            if (PawnNetwork.imprisonedSide[turn] > 0)
            {
                //................When You are in the Jail.............//
                Debug.Log("Can Move from Jail");
                return CanMoveFromJail(amount, count, sign);
            }
            else                                                // when they are not in jail
            {
                if (PawnNetwork.shelterSide[turn])
                {  //............. when the mode of entering the shelter..................//
                    Debug.Log("CanMoveInShelter");
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
                    if (SlotNetwork.slots[(val + 1) + sign * dices[i]].Height() > 1 && SlotNetwork.slots[(val + 1) + sign * dices[i]].IsWhite() != turn)
                        count++;

            return !(count == amount);
        }

        private static bool CanMoveFree(int value, int sign)
        {
            for (int i = 1; i <= 24; i++)
                if (SlotNetwork.slots[i].Height() > 0 && SlotNetwork.slots[i].IsWhite() == turn)   // slot with whites
                    for (int j = 0; j < 2; j++)
                        if (dices[j] != 0 && dices[j] + sign * i <= value)
                            if (SlotNetwork.slots[i + sign * dices[j]].Height() < 2)
                                return true;
                            else if (SlotNetwork.slots[i + sign * dices[j]].Height() > 1 && SlotNetwork.slots[i + sign * dices[j]].IsWhite() == turn)
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
                    if (SlotNetwork.slots[endSlotNo + sign * j].Height() > 0)              // if there is a pawn on the slot
                    {
                        if (SlotNetwork.slots[endSlotNo + sign * j].IsWhite() == turn)     // if this pawn is ours
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

                                    if (ind >= 0 && ind < SlotNetwork.slots.Count)             // index must be non negative
                                    {
                                        if (SlotNetwork.slots[ind].Height() > 0)               // if there are pieces on the slot
                                        {
                                            if (SlotNetwork.slots[ind].IsWhite() != turn)      // if there is an opponent's pawn on the target slot
                                            {
                                                if (SlotNetwork.slots[ind].Height() < 2)       // if there is no wall on the destination slot
                                                {
                                                    return true;
                                                }
                                            }

                                            if (SlotNetwork.slots[ind].IsWhite() == turn)      // if there is a pawn on the target slot
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
    }
}