using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Broniek.Stuff.Sounds;

namespace BackgammonNet.Lobby
{
    // Support for a list that visualizes the names of opponents connected to the same server.

    public class OpponentsList : MonoBehaviour
    {
        public static OpponentsList Instance;

        [SerializeField] private GameObject playersPanel;
        [SerializeField] private Button playerBtn;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform playerPrefabRect;

        private List<GameObject> opponents = new List<GameObject>();
        private float prefabHeight;

        public int Opponents { get { return opponents.Count; } }

        private void Awake()
        {
            //Instance = this;
            //playersPanel.gameObject.SetActive(false);
            //Client.OnNewPlayer += AddPlayer;
            //Client.OnRemovePlayer += RemovePlayer;
            //playerBtn.onClick.AddListener(delegate { scrollRect.gameObject.SetActive(!scrollRect.gameObject.activeSelf); });
            //prefabHeight = playerPrefabRect.rect.height;
        }

        private void OnDestroy()
        {
            Client.OnNewPlayer -= AddPlayer;
            Client.OnRemovePlayer -= RemovePlayer;
        }

        public void AddPlayer(string name)
        {
            GameObject avatar = Instantiate(playerPrefabRect.gameObject);
            avatar.GetComponentInChildren<Text>().text = name;

            if(LobbyManager.Instance.client.isHost)
                avatar.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    LobbyManager.Instance.client.Send("CRMV|" + name);      // kicking from the game table
                });

            avatar.transform.SetParent(scrollRect.content);
            opponents.Add(avatar);
            scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, opponents.Count * prefabHeight);

            SoundManager.GetSoundEffect(3, 0.25f);
        }

        public void RemovePlayer(string name)
        {
            GameObject avatar = opponents.Find(p => p.GetComponentInChildren<Text>().text == name);
            opponents.Remove(avatar);
            Destroy(avatar);
            scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, opponents.Count * prefabHeight);

            SoundManager.GetSoundEffect(3, 0.25f);
        }

        public void RemoveAll()
        {
            for (int i = 0; i < opponents.Count; i++)
                Destroy(opponents[i]);

            opponents = new List<GameObject>();
            scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        }

        public void SwitchVisibility(bool visible)
        {
            playersPanel.gameObject.SetActive(visible);
        }
    }
}