using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;


[Serializable]
public class CameraData
{
    public Vector3 position;
    public Vector3 rotation;

    public CameraData(Vector3 position, Vector3 rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}
public enum PlayerStates
{
    falling = 2,
    walking = 1,
    idle = 0
}


[Serializable]
public class PlayerData
{
    public string name;
    public long id;

    public string versionHash;

    public string loginSessionId;

    public string deviceId;
    public string sessionId;
    public string lobbyId;
    public LobbyRules rules;
    public int ping;

    public int bullets;
    public string weapon;

    public float health;
    public int state;

    public int kills;
    public bool isDead;
    public bool reloading;

    public CameraData CameraData;

    public Vector3 position;
    public Vector3 rotation;

    public PlayerData(string name, Vector3 position, Vector3 rotation, CameraData CameraData, bool withRules = false)
    {
        this.name = name;

        this.versionHash = AntiCheatRules.versionHash;

        string loginSessionId = Udp.client.loginSessionId;

        this.loginSessionId = loginSessionId == null ? "" : loginSessionId;

        this.position = position;
        this.rotation = rotation;
        this.health = 100;
        this.state = 0;
        this.kills = 0;
        this.isDead = false;
        this.CameraData = CameraData;
        this.deviceId = Udp.client.deviceId;
        this.id = Udp.client.id;
        this.sessionId = Udp.client.sessionId;
        this.lobbyId = Udp.client.lobbyId.ToString().Trim();
        if (withRules == true)
        {
            this.rules = Udp.client.lobbyRules;
        }
        
        this.ping = Udp.client.ping;
    }
}

[Serializable]
public class ServerData
{
    [SerializeField]
    

    public PlayerData[] players;

    public Rules rules;

    public string creator;

    public long creatorId;

    public ServerData()
    {
        this.players = new PlayerData[0];
    }
}

[Serializable]
public class Event
{
    public string name;
    public long playerId;

    public string loginSessionId;

    public string deviceId;
    public string sessionId;
    public string lobbyId;

    public int state;
    public int ping;

    public string shootType;

    public long secondId;

    public Vector3 positionHit;
    public bool headShot = false;

    public string type;

    public long time;
    public Event(string type, Vector3 positionHit, long secondId, string shootType,bool headShot) // Shoot
    {
        this.type = type;
        this.secondId = secondId;
        this.shootType = shootType;
        this.headShot = headShot;
        this.positionHit = positionHit;
        this.playerId = Udp.client.id;
        this.deviceId = Udp.client.deviceId;
        this.sessionId = Udp.client.sessionId;
    }
    public Event(string type) // keepAlive, Disconnect and Reload
    {
        this.type = type;
        this.playerId = Udp.client.id;
        this.deviceId = Udp.client.deviceId;
        this.sessionId = Udp.client.sessionId;
    }

   
}


[Serializable]
public class ServerForce
{
    [SerializeField]

    public string type;
    public string sessionId;
    public string lobbyId;

    public Rules rules;
    public long creatorId;
    public string creator;

    public string reason;

    public Vector3 correctPosition;

    public PlayerData[] players;

    public ServerForce(string type)
    {
        this.type = type;
        this.sessionId = Udp.client.sessionId;
    }
}




[Serializable] 
public class TcpRequest
{
    public string request;

    public string type;


    public long playerId;

    
    public string deviceId;
    public string sessionId;
    public string lobbyId;

    public long fromId;
    public long toId;

    public Vector3 hit;
    public Color hitColor;
    public float emissionIntensity;
    public bool playerHit = false;
    public string shootType;

    public TcpRequest(string request, string type, Vector3 hit = new Vector3())
    {
        this.fromId = Udp.client.id;
        this.playerId = Udp.client.id;
        this.deviceId = Udp.client.deviceId;
        this.sessionId = Udp.client.sessionId;
        this.lobbyId = Udp.client.lobbyId;
        this.request = request;
        this.type = type;
        this.hit = hit;
    }
    public TcpRequest(string type, Vector3 hit = new Vector3())
    {
        this.playerId = Udp.client.id;
        this.deviceId = Udp.client.deviceId;
        this.sessionId = Udp.client.sessionId;
        this.lobbyId = Udp.client.lobbyId;
        this.type = type;
        this.hit = hit;
    }

}