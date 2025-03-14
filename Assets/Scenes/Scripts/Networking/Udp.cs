using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEditor;
using System.Threading.Tasks;

public class Udp : MonoBehaviour
{
    public static Udp client;
    public string deviceId;
    private void Awake()
    {
        this.deviceId = SystemInfo.deviceUniqueIdentifier;
        client = this;
    }
    public UdpClient socket;

    public GameObject playerModel;

    private readonly IPEndPoint ep = new IPEndPoint(IPAddress.Parse(NetworkingCredentials.ip), NetworkingCredentials.port);

    public int port = NetworkingCredentials.udpClientPort;

    private bool closing = false;
    public ServerData ServerData = new ServerData();
    public Dictionary<long, OtherPlayer> idsObjs = new Dictionary<long, OtherPlayer>();

    Player player;

    public bool loggedIn = false;
    public new string name;
    public long id = -1;
    public string loginSessionId;

    public string sessionId;
    public string lobbyId;
    public Rules Rules = RulesStatic.rulesObj;

    public LobbyRules lobbyRules = new LobbyRules();
    //AntiCheat

    private long timeSendPing = 0;
    private long lastReceivePong = 0;
    public int ping = 999;

    public void Create()
    {
        if (socket == null)
            socket = new UdpClient(port);
    }
    public void BeginReceive()
    {
        socket.BeginReceive(new AsyncCallback(OnUdpData), socket);
    }

    public void OnUdpData(IAsyncResult result)
    {
        UdpClient socket = result.AsyncState as UdpClient;
        IPEndPoint source = new IPEndPoint(0, 0);
        byte[] message = socket.EndReceive(result, ref source);
        string returnData = Encoding.UTF8.GetString(message);

        UpdateServerData(returnData);
        BeginReceive();
    }
    bool isOnCorrection = false;
    Vector3? _correctPosition;
    Vector3? correctPosition { get { return _correctPosition; } 
        set
        {
            _correctPosition = value;
            isOnCorrection = value != null;
        } 
    }
    private void UpdateServerData(string json)
    {
        ServerData data = JsonParser.FromCompressedJson<ServerData>(json);
        ServerForce force = JsonParser.FromCompressedJson<ServerForce>(json);
        if (force.type != null)
        {
           if (force.type == "position")
           {
                if (force.reason == "startPos")
                {
                    sessionId = force.sessionId;
                    lobbyId = force.lobbyId;
                    ServerData.creatorId = force.creatorId;
                    ServerData.creator = force.creator;
                    loggedIn = true;

                    if (force.rules != null)
                    {
                       Rules = force.rules;
                    }
                }
                correctPosition = force.correctPosition;
                ServerData.players = force.players;
           }else if (force.type == "pong")
           {
                long time = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                ping = (int)(time - timeSendPing);
                lastReceivePong = time;
                PingDisplay.main.UpdatePing();
            }else if (force.type == "ExitGame")
            {
                CloseConnection();
                ErrorUi.main.NewError(force.reason, true);
            }
        }
        else
        {
            if (sessionId == "")
            {
                return;
            }
            ServerData.players = data.players;
           
            isOnCorrection = false;
        }
    }
    
    public void CorrectPosition(Vector3? correct)
    {
        if (gameObject.transform.position != correct)
        {
            gameObject.transform.position = (Vector3)correct;
            correctPosition = null;
        }
    }
    void Start()
    {
        player = gameObject.GetComponent<Player>();
        Create();
        BeginReceive();
        Login();
        InvokeRepeating("PingCycle", 0, 1f);
        InvokeRepeating("UpdateCycle", 0, 0.04f);
    }
    public async void Login()
    {
        int timePassed = 0;
        while (sessionId == "" || lobbyId == "" || id == -1)
        {
            if (timePassed >= 1000)
            {
                timePassed = 0;
                return;
            }
            if (timePassed == 0)
            {
                PlayerData data = new PlayerData(name, new Vector3(), new Vector3(), new CameraData(new Vector3(), new Vector3()), true);

                byte[] sendbuf = JsonParser.ToCompressedJsonByteArray(data);

                try
                {
                    socket.Send(sendbuf, sendbuf.Length, ep);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
            timePassed += 25;
            await Task.Delay(25);
        }
        
    }
    public void EmptyPacket()
    {
        if (closing == true || loggedIn == false)
        {
            return;
        }
        Event data = new Event("keepAlive");
        data.state = player.state;
        data.ping = this.ping;
        byte[] sendbuf = JsonParser.ToCompressedJsonByteArray(data);
        socket.Send(sendbuf, sendbuf.Length, ep);
    }
    public void SendData(object data)
    {
        if (socket == null || loggedIn == false)
        {
            return;
        }
        byte[] sendbuf = JsonParser.ToCompressedJsonByteArray(data);
        socket.Send(sendbuf, sendbuf.Length, ep);
    }

    public void Ping()
    {
        if (closing == true || loggedIn == false)
        {
            return;
        }
        timeSendPing = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        SendData(new ServerForce("ping"));
    }
    public void UpdatePos(Vector3 pos, Vector3 rot, CameraData CameraData)
    {
        if (closing == true || socket == null || loggedIn == false || isOnCorrection == true || lastFrameIsDead == true)
        {
            return;
        }
        PlayerData data = new PlayerData(name, CustomMath.RoundVector(pos,3), rot, CameraData);
        data.state = Player.instance.state;
        byte[] sendbuf = JsonParser.ToCompressedJsonByteArray(data);
        try
        {
            socket.Send(sendbuf, sendbuf.Length, ep);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    void UpdateOrAddPlayer(PlayerData player)
    {
        OtherPlayer otherPlayer = idsObjs.GetValueOrDefault(player.id);
        GameObject obj = otherPlayer == null ? null : otherPlayer.gameObject;
        OtherPlayer UpdateObjPlayerData()
        {
            OtherPlayer otherPlayer = obj.GetComponent<OtherPlayer>();
            otherPlayer.name = player.name;
            otherPlayer.health = player.health;
            otherPlayer.kills = player.kills;

            if (otherPlayer.isDead == false && player.isDead == true)
            {
                DieEffect.main.AddPosition(otherPlayer.transform.position);
            }

            otherPlayer.isDead = player.isDead;
            otherPlayer.position = player.position;
            otherPlayer.rotation = player.rotation;
            otherPlayer.cameraData = player.CameraData;
            otherPlayer.bullets = player.bullets;
            otherPlayer.reloading = player.reloading;
            otherPlayer.weaponName = player.weapon;
            otherPlayer.state = player.state;
            otherPlayer.id = player.id;

            otherPlayer.UpdateWeapon(player.weapon);

            WeaponRules weaponRules = Rules.weaponsRules.Find((item) => item.WeaponName == player.weapon);
            otherPlayer.weaponRules = weaponRules == null ? Player.instance.weaponRules : weaponRules;
            return otherPlayer;
        }
        if (obj == null || obj.CompareTag("Player") == false)
        {
            obj = Instantiate(playerModel);
            obj.transform.position = player.position;
            obj.transform.eulerAngles = player.rotation;
            obj.SetActive(true);
            obj.name = player.name;
            OtherPlayer otherPlayerData =  UpdateObjPlayerData();

            idsObjs.Add(player.id, otherPlayerData);
            
            LoadImageFromUrl.DownloadProfileImageStart(player.id);
        }
        else
        {
           OtherPlayer data = UpdateObjPlayerData();

           data.gameObject.transform.DOMove(player.position, 0.15f);
           data.gameObject.transform.DORotate(player.rotation, 0.15f);

            data.headInvisible.transform.DORotate(player.CameraData.rotation, 0.15f);
           data.torsoInvisible.transform.DORotate(player.CameraData.rotation, 0.15f);
        }
    }
    void UpdateCurrentPlayer(PlayerData player)
    {
        if (this.player.health != player.health)
        {
            HealthBar.main.UpdateHealthBar();
        }
        if (this.player.health != player.health && this.player.isDead == false)
        {
            if (this.player.health > player.health)
            {
                FullScreenStuff.instance.DamagedScreen(0.5f);
            }else
            {
                FullScreenStuff.instance.HealedScreen(0.5f);
            }
        }
        this.player.health = player.health;
        this.player.kills = player.kills;
        this.player.isDead = player.isDead;
        this.player.bullets = player.bullets;
        this.player.reloading = player.reloading;
        this.player.weapon = player.weapon;
        this.id = player.id;
        Udp.client.name = player.name;
        if (!player.name.Equals(gameObject.name))
        {
            gameObject.name = player.name;
        }
        this.player.UpdateWeapon(player.weapon);

        if (lastFrameIsDead == true && player.isDead == false || (player.position - transform.position).magnitude > AntiCheatRules.maxMoveDistance)
        {
           gameObject.transform.position = player.position;
        }
    }
    void RemoveDisconnectedPlayers()
    {
        GameObject[] playersObjs = GameObject.FindGameObjectsWithTag("Player");
        if (ServerData == null || ServerData.players.Length == 0)
        {
            if (playersObjs.Length > 1)
            {
                foreach (GameObject obj in playersObjs)
                {
                    OtherPlayer player = null;
                    try
                    {
                        player = obj.GetComponent<OtherPlayer>();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    if (player != null && player.id != this.id)
                    {
                        LoadImageFromUrl.RemoveImageFromStorage(player.id);

                        obj.transform.DOComplete();
                        obj.transform.DOKill();
                        obj.transform.GetChild(0).DOComplete();
                        obj.transform.GetChild(0).DOKill();

                        Destroy(obj);
                    }
                }
            }
        }
        else
        {
            foreach (GameObject obj in playersObjs)
            {
                OtherPlayer otherPlayerData = null;
                try
                {
                    otherPlayerData = obj.GetComponent<OtherPlayer>();
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
                PlayerData playerData = ServerData.players.FirstOrDefault(player => player.id.Equals(otherPlayerData.id));
                if (player != null && otherPlayerData.id != id && playerData == null)
                {
                    LoadImageFromUrl.RemoveImageFromStorage(otherPlayerData.id);

                    if (idsObjs.ContainsKey(otherPlayerData.id))
                    {
                        idsObjs.Remove(otherPlayerData.id);
                    }

                    obj.transform.DOComplete();
                    obj.transform.DOKill();
                    obj.transform.GetChild(0).DOComplete();
                    obj.transform.GetChild(0).DOKill();
                    
                    Destroy(obj);
                }
            }
        }
    }


    bool lastFrameIsDead = false;
    long timeLostConnection = 0;
    void PingCycle()
    {
        Ping();
        long time = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        if (time - lastReceivePong > 5000)
        {
            if (timeLostConnection == 0)
            {
                timeLostConnection = time;
            }
            else if (time - timeLostConnection > 30000)
            {
                CloseConnection();
                TCP.main.CloseConnection();
                ErrorUi.main.NewError("Lost connection.", true);

            }
            ping = 999;
        }
    }
    void UpdateCycle()
    {
        //remove disconnected player
        RemoveDisconnectedPlayers();
        //update positions
        if (ServerData != null && ServerData.players.Length >= 1)
        {
            for (int i = 0; i < ServerData.players.Length; i++)
            {
                PlayerData player = ServerData.players[i];
                if (player.id != this.id)
                {
                    UpdateOrAddPlayer(player);
                }
                else
                {
                    UpdateCurrentPlayer(player);
                }
            }

        }
        lastFrameIsDead = player.isDead;
    }
    private void FixedUpdate()
    {
        if (isOnCorrection == true && (player.isDead == false && lastFrameIsDead == false))
        {
            CorrectPosition(correctPosition);
        }
    }
    public void CloseConnection()
    {
        try
        {
            socket.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        finally
        {
            socket?.Close();
        }
    }
    public void ExitGame()
    {
        if (socket != null)
        {
            byte[] sendbuf = JsonParser.ToCompressedJsonByteArray(new Event("disconnect"));
           
            try
            {
                socket.Send(sendbuf, sendbuf.Length, ep);
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
            
            loggedIn = false;
            sessionId = null;
        }
    }
    void OnApplicationQuit()
    {
        ExitGame();
        CloseConnection();
    }
}
