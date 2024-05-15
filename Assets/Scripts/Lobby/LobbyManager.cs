using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace BackgammonNet.Lobby
{
    // In the main menu, depending on the situation, create or choose a table.

    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; set; }
        public static string clientName;

        [Header("For the Ai")]
        public static bool AiMode=false;        // For the  AiMode

        [Header("Configuration")]
        [SerializeField] private bool mainServer;
        [SerializeField] private float controlPeriod;               // Host availability control period.
        [SerializeField] private string[] mainServerAddresses;      // The IPv4 addresses of the server with the host list (important for hosts and clients).
                                                                    // The second and subsequent addresses indicate substitute servers.
        [Header("Prefabs")]
        [SerializeField] private GameObject serverPrefab;
        [SerializeField] private GameObject clientPrefab;
        [SerializeField] private RectTransform playerPrefabRect;
        
        [Header("Menu views")]
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject hostMenu;
        [SerializeField] private GameObject connectMenu;
        [SerializeField] private GameObject mainServerPanel;
        [SerializeField] private GameObject loadingScreenPanel;

        [Header("Other UI elements")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private InputField inputField;
        [SerializeField] private InputField nameInput;
        [SerializeField] private Text monitText;
        [SerializeField] private Text headerTxt;
        private GameObject hostGoButton;

        public static string monitContent = "";
        [HideInInspector] public List<GameObject> hosts;            // List of available hosts (tables).
        [HideInInspector] public string ourIpAddress;               // Our ip address.
        [HideInInspector] public Client client;                     // A dedicated client that connects to the host.
        private Client mainClient;                                  // A dedicated client that connects to the main server.
        private float prefabHeight;
        private string hostAddress;
        private bool mainServerMode;
        private int portNo = 6321;

        private void Start()
        {
            Instance = this;
            hosts = new List<GameObject>();
            prefabHeight = playerPrefabRect.rect.height;

            
          
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            RemoveNetworkParts();
        }

        private bool IsAlreadyHost()
        {
            bool isServer = IsServer(ourIpAddress, portNo);
            hostGoButton.SetActive(!isServer);                                          // toggle button visibility
            if(isServer)
                monitText.text = "The host already exists.";
            return isServer;
        }
        
        //-----------------------------------------------------------------------------

        private string GetLocalIPAddress()                                              // ip address information
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private void InformMainServer(string name)  // reaction to a change in the number of players sitting at the table
        {
            if (client)
            {
                if (client.isHost)     // only the host sends information to the master server about the number of players sitting at the table
                {
                    try
                    {
                        int amount = OpponentsList.Instance.Opponents + 1;

                        if (amount > 0)                                                 // the only message sent to the main server during the game
                            mainClient.Send("CAMNT|" + ourIpAddress + "|" + amount);    //Debug.Log("Host sending CAMNT|" + ourIpAddress + "|" + amount);
                    }

                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }
        }

        //---- maintenance of the list of available hosts -----------------------------------------------------

        public void AddHost(string ipAddress, string hostName)
        {
            if (!hosts.Exists(h => h.name == ipAddress))
            {                
                GameObject host = Instantiate(playerPrefabRect.gameObject);
                host.name = ipAddress;
                host.GetComponentInChildren<Text>().text = hostName;

                host.GetComponent<Button>().onClick.AddListener(delegate
                {
                    string[] aData = hostName.Split(' ');
                    inputField.text = aData[0];
                    hostAddress = ipAddress;             // ip addresses should not be explicitly visible
                });

                host.transform.SetParent(scrollRect.content);
                hosts.Add(host);
                scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, hosts.Count * prefabHeight);
            }
        }

        public void RemoveHost(string ipAddress)
        {
            if (hosts.Exists(h => h.name == ipAddress))
            {
                GameObject host = hosts.Find(h => h.name == ipAddress);
                hosts.Remove(host);
                Destroy(host);
                scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, hosts.Count * prefabHeight);                
            }
        }
        
        public void UpdateHost(string ipAddress, string amount)
        {
            if (hosts.Exists(h => h.name == ipAddress))
            {
                GameObject host = hosts.Find(h => h.name == ipAddress);
                Text text = host.GetComponentInChildren<Text>();
                string txt = text.text;
                string[] aData = txt.Split(' ');
                text.text = aData[0] + " (" + amount + ")";
            }
        }
        
        //---- main server methods ------------------------------------------------------------------

        private void CreateMainServer()                     // Create the main server.
        {
            try
            {
                Server server = Instantiate(serverPrefab).GetComponent<Server>();
                server.Init(true);
                server.name = "Main Server";
                headerTxt.text = GetLocalIPAddress() + ":\n" + headerTxt.text;
            }

            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        private IEnumerator HostAvailabilityControl(float period)         // Periodically updating the availability of hosts.
        {
            int k = 0;
            string txt = headerTxt.text;

            while (true)
            {
                yield return null;

                if (hosts.Count > 0)
                {
                    int count = hosts.Count;

                    for (int i = 0; i < count; i++)
                    {
                        int j = count - 1 - i;

                        if (!IsServer(hosts[j].name, portNo))
                            RemoveHost(hosts[j].name);
                                                
                        headerTxt.text = txt + " (" + (k++ % 2 == 0 ? "-" : " ") + ")";

                        yield return new WaitForSeconds(period / count);        // all hosts checked within a period of seconds
                    }
                }

                yield return new WaitForSeconds(1);
            }
        }
        
        //-----------------------------------------------------------------------------
        
        private bool IsServer(string address, int port)     // detect if a host already exists on a given address and port
        {
            bool hostExists;

            try
            {
                Client client = Instantiate(clientPrefab).GetComponent<Client>();           // Create a remote client.

                if (client.ConnectToServerTest(address, port))
                    hostExists = true;                                                      // if there is already a host
                else
                    hostExists = false;

                Destroy(client.gameObject);
            }

            catch (Exception e)
            {
                Debug.Log(e.Message);
                return false;
            }

            return hostExists;
        }

        //---- operation of the initial menu buttons ----------------------------------------------------------
        public GameObject Canvas1;
        public GameObject Canvas2;
        public void StartGame()                             // Start a local game.
        {
          //  SwitchMenuView(false, false, false, false);

            if(SceneManager.sceneCount > 1)
                SceneManager.UnloadSceneAsync(1);
            Canvas1.gameObject.SetActive(false);
            Canvas2.gameObject.SetActive(false);
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
            //SceneManager.LoadScene(1);
        }



        //public void StartGame()                             // Start a local game.
        //{
        //    //  SwitchMenuView(false, false, false, false);

        //    if (SceneManager.sceneCount > 1)
        //        SceneManager.UnloadSceneAsync(1);

        //    SceneManager.LoadScene(1, LoadSceneMode.Additive);
        //    //SceneManager.LoadScene(1);
        //}

        #region  _ForAiModeStart
        public void StartGameAi()                             // Start a local game.
        {
            AiMode = true;
            SwitchMenuView(false, false, false, false,false);

            if (SceneManager.sceneCount > 1)
                SceneManager.UnloadSceneAsync(1);

            SceneManager.LoadScene(1, LoadSceneMode.Additive);
        }


        #endregion

        // Creating a single game table (we first connect to the main server to send it the address of our newly created table).

        public void CreateHostRequest()
        {
            if (IsAlreadyHost())
                return;

            StartCoroutine(CreateHostRequestRoutine());
        }

        private IEnumerator CreateHostRequestRoutine()
        {
            if (nameInput.text != "")
            {
                clientName = nameInput.text;
                monitText.text = "Waiting for main server!";
                yield return null;

                if (!IsServer(ourIpAddress, portNo))       // if the host server is not running on our address yet
                {
                    for (int i = 0; i < mainServerAddresses.Length; i++)
                    {
                        try
                        {
                            if (mainClient == null)
                            {
                                mainClient = Instantiate(clientPrefab).GetComponent<Client>();      // Create a remote client.
                                mainClient.clientName = clientName;
                                mainClient.isHost = true;
                                mainClient.name = "Host Client to Main Server";
                            }
                                                                                                    // connect to the first available main server
                            if (mainClient.ConnectToServer(mainServerAddresses[i], portNo))         // if there is already a given Main Server
                            {
                                monitContent = "";
                                mainServerMode = true;
                                break;
                            }
                        }
                        
                        catch (Exception e)
                        {
                            Debug.Log(e.Message);
                        }
                    }

                    if (!mainServerMode)
                    {
                        yield return new WaitForSeconds(1f);
                        monitText.text = "No main server!";
                        yield return new WaitForSeconds(1f);
                        monitContent = "";
                        SwitchMenuView(false, false, true, false,false);
                        CreateHost();
                    }
                }
            }
            else
                monitText.text = "Enter your name!";
        }

        public void CreateHost()                            // The host should be created only after information has been exchanged with the master server.
        {
            try
            {
                Server server = Instantiate(serverPrefab).GetComponent<Server>();       // Create the server.
                server.Init(false);
                server.name = "Host Server";

                client = Instantiate(clientPrefab).GetComponent<Client>();              // Create a remote client.
                client.clientName = clientName;
                client.isHost = true;
                client.name = "Host Client";

                client.ConnectToServer(ourIpAddress, portNo);                           // From his perspective, the server address is localhost.
            }

            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        // If we want to choose a table, we first connect to the main server to get a list of available tables.

        public void GoToConnectMenu()                       // Go to the panel that allows you to choose a table.
        {
            StartCoroutine(GoToConnectMenuRoutine());
        }

        private IEnumerator GoToConnectMenuRoutine()
        { 
            if (nameInput.text != "")
            {
                clientName = nameInput.text;
                monitText.text = "Waiting for main server!";
                yield return null;

                for (int i = 0; i < mainServerAddresses.Length; i++)
                {
                    try
                    {
                        if (mainClient == null)
                        {
                            mainClient = Instantiate(clientPrefab).GetComponent<Client>();      // Create a remote client.
                            mainClient.clientName = clientName;
                            mainClient.name = "Remote Client to Main Server";
                        }

                        if (mainClient.ConnectToServer(mainServerAddresses[i], portNo))          // if there is already a given Main Server
                        {
                            SwitchMenuView(false, true, false, true,false);   // The list with the addresses of the hosts sent by the master server appears.
                            monitContent = "";
                            mainServerMode = true;
                            break;
                        }
                    }

                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }

                if(!mainServerMode)
                {
                    yield return new WaitForSeconds(1f);
                    monitText.text = "No main server!";
                    yield return new WaitForSeconds(1f);
                    SwitchMenuView(false, true, false, true,false);   // The list with the addresses of the hosts sent by the master server appears.
                    monitContent = "";
                    inputField.text = ourIpAddress;
                    //inputField.text = "xxx.162.1.6";
                }
            }
            else
                monitText.text = "Enter your name!";
        }

        public void ConnectToHost()                         // Attempting to establish a remote game client (joining a single game table).
        {
            if(!mainServerMode)
                hostAddress = ourIpAddress;

            if (hostAddress == "")
                return;

            try
            {
                client = Instantiate(clientPrefab).GetComponent<Client>();      // Create a remote client.
                client.clientName = clientName;
                client.name = "Remote Client to Host";

                if (client.ConnectToServer(hostAddress, portNo))                // if there is already a given host
                    connectMenu.SetActive(false);
                else
                {
                    monitText.text = "The specified host does not exist!";
                    GoToMainMenu();
                }
            }

            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        
        //---- operating the buttons (MainMenu, Cancel, Back) to go back to the initial menu view ---------------------

        public void GoToMainMenu()
        {
            monitText.text = monitContent;
            SwitchMenuView(true, false, false, false,false);         // return to the main menu
            RemoveNetworkParts();
        }

        public void RemoveNetworkParts()
        {
            Server server = FindObjectOfType<Server>();

            if (server != null)
            {
                server.Stop();
                Destroy(server.gameObject);                    // remove the server
            }
            
            if (client != null)
                Destroy(client.gameObject);                    // remove local client

            if (mainClient != null)
                Destroy(mainClient.gameObject);
        }

        public void SwitchMenuView(bool vis1, bool vis2, bool vis3, bool vis4,bool vis5)
        {
            mainMenu.SetActive(vis1);
            connectMenu.SetActive(vis2);
            hostMenu.SetActive(vis3);
            mainServerPanel.SetActive(vis4);
            loadingScreenPanel.SetActive(vis5);
        }
    }
}