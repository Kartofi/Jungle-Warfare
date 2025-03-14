using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class TCP : MonoBehaviour
{
    private TcpClient client = null;
    private NetworkStream stream = null;
    private string serverIP = NetworkingCredentials.ip; // Replace with the server's IP address
    private int serverPort = NetworkingCredentials.port; // Replace with the server's port number

    private byte[] receiveBuffer = new byte[2048];
    private bool isConnected = false;
    private WaitForSeconds reconnectionDelay = new WaitForSeconds(3f);

    public static TCP main;
    void Start()
    {
        StartCoroutine(StartConnectWhenSessionId());
        //Keep Alive
        InvokeRepeating("KeepAlive", 10, 10);
        main = this;
    }
    IEnumerator StartConnectWhenSessionId()
    {
        Debug.Log("Waiting for scene to load...");
        yield return new WaitWhile(() => Time.frameCount < 10);
        ConnectToServer();
    }

    private void ConnectToServer()
    {
        client = new TcpClient();
        client.BeginConnect(serverIP, serverPort, OnConnected, null);
    }

    private async void OnConnected(IAsyncResult result)
    {
        try
        {
            client.EndConnect(result);
            stream = client.GetStream();
            Debug.Log("Connected to server");
            isConnected = true;
            int timePassed = 0;
            while(Udp.client.sessionId == "" || Udp.client.lobbyId == "" || Udp.client.id == -1)
            {
                if (timePassed >= 8000)
                {
                    CloseConnection();
                    return;
                }
                timePassed += 25;
                await Task.Delay(25);
            }
            TcpRequest data = new TcpRequest("join");
            byte[] sendData = JsonParser.ToJsonByteArray(data);
            stream.BeginWrite(sendData, 0, sendData.Length, OnDataSent, null);
            // Begin receiving data from the server
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveData, null);
        }
        catch (Exception e)
        {
            Debug.LogError("Connection failed: " + e.Message);
            ErrorUi.main.NewError("Can't connect to the server", true);
            isConnected = false;
            StartCoroutine(Reconnect());
        }
    }

    private void OnDataSent(IAsyncResult result)
    {
        stream.EndWrite(result);
    }

    private void ReceiveData(IAsyncResult result)
    {
        if (stream.CanRead == false && isConnected == false)
        {
            return;
        }
        try
        {
            int bytesRead = stream.EndRead(result);
            if (bytesRead <= 0)
            {
                // Server disconnected
                Debug.Log("Server disconnected");
                isConnected = false;
                StartCoroutine(Reconnect());
                return;
            }
            if (bytesRead > 0)
            {
                stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveData, null);
                string receivedData = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);

                string[] messages = receivedData.Split("\0");
                if (messages.Length <= 0)
                {
                    return;
                }
                List<string> tcpData = new List<string>();
                
                foreach(string item in messages)
                {
                    string[] parts = item.Split("\u0007");
                    if (parts.Length < 2)
                    {
                        continue;
                    }
                    string count = parts[0];


                    string resultData = "";

                    int lengthToRead = 0;
                    try
                    {
                        lengthToRead = int.Parse(parts[0]);
                        resultData = parts[1];
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Error: " + e.Message);
                        continue;
                    }
                    if (resultData != "")
                    {
                        tcpData.Add(resultData);
                    }
                }
                foreach (string item in tcpData)
                {
                    try
                    {
                        TcpRequest data = JsonUtility.FromJson<TcpRequest>(item);
                        ProcessTcpData(data);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        continue;
                    }
                    
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log("Server disconnected");
            isConnected = false;
            StartCoroutine(Reconnect());
        }
    }

    private IEnumerator Reconnect()
    {
        yield return reconnectionDelay;
        int countTried = 0;
        Udp.client.loggedIn = false;
        Udp.client.sessionId = "";
        while (!isConnected)
        {
            if (countTried > 10)
            {
                ErrorUi.main.NewError("Can't connect to the server", true);
            }
            countTried++;
            
            Debug.Log("Attempting to reconnect...");
            ConnectToServer();
            yield return reconnectionDelay;
        }
        Udp.client.Login();
    }


    public bool CheckName(string name)
    {
        try
        {
            using (TcpClient client = new TcpClient(serverIP, serverPort))
            {
                using (NetworkStream stream = client.GetStream())
                {
                    // Send the message to the server
                    // Send data to the server
                    byte[] sendData = JsonParser.ToJsonByteArray(new TcpRequest(name, "nameCheck"));
                    stream.BeginWrite(sendData, 0, sendData.Length, OnDataSent, null);

                    // Receive the response from the server
                    byte[] receiveBuffer = new byte[1024];
                    int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
                    string responseString = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
                    TcpRequest response = JsonParser.FromJson<TcpRequest>(responseString);
                    if (response.type == "Success")
                    {
                        return true;
                    }else
                    {
                        return false;
                    }
                    
                }
            }
        }catch(Exception e)
        {
            Exception space = e;
            return false;
        }
    }

    public void KeepAlive()
    {
        SendData(new TcpRequest("keepAlive"));
    }

    public void SendData(object data)
    {
        if (data == null || Udp.client.sessionId == "" || isConnected == false)
        {
            return;
        }
        
        byte[] sendData = JsonParser.ToJsonByteArray(data);
        try
        {
            stream.BeginWrite(sendData, 0, sendData.Length, OnDataSent, null);
        }
        catch (Exception e)
        {
            ErrorUi.main.NewError("Error occurred while sending data using TCP , please rejoin!", true);
            Debug.Log("Error occurred while sending the chat message. Source: " + e.Source);
        }
    }
    public void SendChatMessage(string message)
    {
        if (message == "" || message == "\n" || message == " " || Udp.client.sessionId == "" || isConnected == false)
        {
            return;
        }
        TcpRequest data = new TcpRequest(message, "sendMessage");
        data.fromId = Udp.client.id;
        byte[] sendData = JsonParser.ToJsonByteArray(data);
        try
        {
            stream.BeginWrite(sendData, 0, sendData.Length, OnDataSent, null);
        }
        catch(Exception e)
        {
            ErrorUi.main.NewError("Error occurred while sending the chat message.", true);
            Debug.Log("Error occurred while sending the chat message. Source: " + e.Source);
        }
        
    }
   
    private void ProcessTcpData(TcpRequest data)
    {
        switch (data.type)
        {
            case "kill":
                ShootEffects.main.Create(data.fromId, data.hit);
                if (data.toId == Udp.client.id)
                {
                    Debug.Log("You got killed by "+ data.fromId);
                    FullScreenStuff.instance.KilledByText(data.fromId);
                    BasicCamera.main.LookAtFor(data.fromId, 5);
                }else if (data.fromId == Udp.client.id)
                {
                    Debug.Log("You killed " + data.fromId);
                    FullScreenStuff.instance.KilledText(data.toId);
                }
                UiItemsInPanel.killFeed.NewMessage(data.fromId,data.toId);
                break;
            case "sendMessage":
                UiItemsInPanel.chat.NewMessage(data.fromId, data.request);
                break;
            case "shootIndicator":
                if (data.fromId == Udp.client.id)
                {
                    return;
                }
                ShootEffects.main.Create(data.fromId, data.hit, data.hitColor, data.playerHit,data.emissionIntensity);
                break;
            case "reload":
                if (data.fromId == Udp.client.id)
                {
                    return;
                }
                OtherPlayersAnimations.main.Reload(data.fromId);
                break;
            case "ExitGame":
                CloseConnection();
                break;
        }
           
    }
    public void CloseConnection()
    {
        if (client != null && client.Connected)
        {
            Debug.Log("Disconnected from the server");
            isConnected = false;
            stream?.Close();
            client?.Close();
            stream = null;
            client = null;
        }
    }
    private void OnApplicationQuit()
    {
        CloseConnection();
    }
    private void OnDestroy()
    {
        CloseConnection();
    }
}
