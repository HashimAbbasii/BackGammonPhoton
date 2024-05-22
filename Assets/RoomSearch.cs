using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomSearch : MonoBehaviour
{
    public TMP_InputField searchInputField;
    public Transform scrollContent; 
    public List<RoomEntry> allRooms = new List<RoomEntry>();



    void OnEnable()
    {
        allRooms = GetComponentsInChildren<RoomEntry>().ToList();
        searchInputField.onValueChanged.AddListener(FilterRooms);
    }


    //[ContextMenu("ABC")]
    //public void FillRooms()
    //{
    //    //allRooms = GetComponentsInChildren<RoomEntry>().ToList();
    //    foreach (var rlgo in MyPhotonManager.instance.roomListGameObject)
    //    {
    //        Debug.Log(rlgo.Key);
    //        Debug.Log(rlgo.Value);
    //    }
    //}

    void FilterRooms(string searchText)
    {
        searchText = searchText.ToLower();

        if (string.IsNullOrEmpty(searchText))
        {
            foreach (var roomEntry in MyPhotonManager.instance.roomListGameObject)
            {
                roomEntry.Value.SetActive(true);
            }
        }

        else
        {
            foreach (var roomEntry in MyPhotonManager.instance.roomListGameObject)
            {
                roomEntry.Value.SetActive(false);
            }

            foreach (var roomEntry in MyPhotonManager.instance.roomListGameObject.Where(t => t.Key.Split("|")[0].Contains(searchText)))
            {
                roomEntry.Value.SetActive(true);
            }
        }
    }


}
