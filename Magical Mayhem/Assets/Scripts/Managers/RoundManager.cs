using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class RoundManager : MonoBehaviour
{
    public static RoundManager instance;

    [SerializeField] private List<UnitController> players = new List<UnitController>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }
    private void OnClientConnectedCallback(ulong clientId)
    {
        UnitController unit = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<UnitController>();
        players.Add(unit);
    }
    private void OnClientDisconnectCallback(ulong clientId)
    {
        UnitController unit = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<UnitController>();
        players.Remove(unit);
    }

    /// <summary>
    /// Finds nearest unit that is not self.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="self"></param>
    /// <returns></returns>
    public UnitController FindNearestUnit(Vector3 position, UnitController self)
    {
        UnitController closest = null;
        float distance = 1000;
        foreach (UnitController unit in players)
        {
            if (unit != self)
            {
                if ((unit.transform.position - position).magnitude < distance)
                {
                    closest = unit;
                    distance = (unit.transform.position - position).magnitude;
                }
            }
        }
        return closest;
    }

}
