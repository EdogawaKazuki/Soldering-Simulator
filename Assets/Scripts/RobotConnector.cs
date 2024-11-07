using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using Unity.Mathematics;
using System.Data.Common;

public class RobotConnector : MonoBehaviour
{
    #region Variables


    public Transform endEffector;

	[SerializeField]
	private string _robotIP = "localhost";
	[SerializeField]
	private int _robotPort = 1234;

	private bool _connected = false;
	private Thread _receiveThread;
	byte[] sendData;
    byte[] recvData = new byte[4096];
	byte[] byteArray;
	int recvLen;

	public bool SendCmd = true;

	Socket ClientSocket;
	EndPoint ServerEndPoint;
	Thread connectThread;
    Vector3 robotPos;
    Vector3 forceDirection;
	public Transform forceDirectionIndicatorX;
	public Transform forceDirectionIndicatorY;
	public Transform forceDirectionIndicatorZ;
	#endregion

	#region MonoBehaviour
    private void Start(){
        SetConnect(true);
    }
    private void Update(){
        endEffector.localPosition = robotPos;
        forceDirectionIndicatorX.localScale = new Vector3(1, forceDirection.z / 100f, 1);
        forceDirectionIndicatorY.localScale = new Vector3(1, forceDirection.x / 100f, 1);
        forceDirectionIndicatorZ.localScale = new Vector3(1, forceDirection.y / 100f, 1);
        // Check for A button press on right controller
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            SendCalibrationCmd();
			Debug.Log("Calibration sent");
        }
    }
    private void OnApplicationQuit()
	{
        SetConnect(false);
    }
    #endregion

    #region Methods
    public void SetRobotIP(string value)
    {
        _robotIP = value;
		PlayerPrefs.SetString("_robotIP", _robotIP);
		PlayerPrefs.Save();
		Debug.Log(value);
	}
    public void SetRobotPort(string value)
    {
        _robotPort = int.Parse(value);
		PlayerPrefs.SetInt("_robotPort", _robotPort);
		PlayerPrefs.Save();
		Debug.Log(value);
	}
    public void SetConnect(bool value)
    {
        if (value)
		{
            try
            {

				ServerEndPoint = new IPEndPoint(IPAddress.Parse(_robotIP), _robotPort);
				ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

				ClientSocket.SendTimeout = 5000;
				ClientSocket.ReceiveTimeout = 5000;

				byteArray = new byte[2];
				byteArray[0] = 0;
				byteArray[1] = 1;
				ClientSocket.SendTo(byteArray, byteArray.Length, SocketFlags.None, ServerEndPoint);

				connectThread = new Thread(new ThreadStart(ServerMsgHandler));
				connectThread.Start();
                _connected = true;
			}
			catch (Exception ex)
			{
				print(ex);
			}
		}
        else
		{
			_connected = false;
			byteArray = new byte[2];
			byteArray[0] = 0;
			byteArray[1] = 0;
			ClientSocket.SendTo(byteArray, byteArray.Length, SocketFlags.None, ServerEndPoint);
		}
    }

    private void ServerMsgHandler()
    {
		while(_connected){
            try{
                recvLen = ClientSocket.ReceiveFrom(recvData, ref ServerEndPoint);
                if (recvLen > 0)
                {
                    // robotPos = new Vector3(BitConverter.ToSingle(recvData, 0), BitConverter.ToSingle(recvData, 4), BitConverter.ToSingle(recvData, 8));
                    robotPos = new Vector3(BitConverter.ToSingle(recvData, 0), 2 * BitConverter.ToSingle(recvData, 4), BitConverter.ToSingle(recvData, 8));
                    forceDirection = new Vector3(Mathf.Rad2Deg * BitConverter.ToSingle(recvData, 12), Mathf.Rad2Deg * BitConverter.ToSingle(recvData, 16), Mathf.Rad2Deg * BitConverter.ToSingle(recvData, 20));
                    // Debug.Log("Robot Pos: " + robotPos);
                }
            }
            catch (Exception e)
            {
				if(_connected){
					Debug.Log(e.ToString());
					Debug.Log("Reconnecting to server");
					byteArray = new byte[2];
					byteArray[0] = 0;
					byteArray[1] = 1;
					ClientSocket.SendTo(byteArray, byteArray.Length, SocketFlags.None, ServerEndPoint);
				}
            }
		}
    }
	public bool IsConnected() { return _connected; }
    public void SendVelCmd(float linear_vel, float angular_vel)
    {
        if (!_connected) {
            Debug.Log("Not connected to robot");
            return;
        }
        try
        {
            byteArray = new byte[sizeof(float) * 2 + 1];
            byteArray[0] = 1;
            Buffer.BlockCopy(BitConverter.GetBytes(linear_vel), 0, byteArray, 1, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(angular_vel), 0, byteArray, 5, 4);
			Debug.Log("Sending Vel Cmd: " + linear_vel + " " + angular_vel);
            ClientSocket.SendTo(byteArray, byteArray.Length, SocketFlags.None, ServerEndPoint);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
	// todo 
	public void SendPosCmd(Vector3 pos, Vector3 rot){

	}
	public void SendStartCmd(){
		if (!_connected) {
			Debug.Log("Not connected to robot");
			return;
		}else{
			byteArray = new byte[2];
			byteArray[0] = 1;
			byteArray[1] = 0;
			ClientSocket.SendTo(byteArray, byteArray.Length, SocketFlags.None, ServerEndPoint);
		}
	}
	public void SendStopCmd(){
		if (!_connected) {
			Debug.Log("Not connected to robot");
			return;
		}else{
			byteArray = new byte[2];
			byteArray[0] = 1;
			byteArray[1] = 1;
			ClientSocket.SendTo(byteArray, byteArray.Length, SocketFlags.None, ServerEndPoint);
		}

	}
	public void SendCalibrationCmd()
	{
		if (!_connected) {
			Debug.Log("Not connected to robot");
			return;
		}
		try
		{
			byteArray = new byte[1];
			byteArray[0] = 2;  // Using 2 as the command type for calibration
			Debug.Log("Sending Calibration Command");
			ClientSocket.SendTo(byteArray, byteArray.Length, SocketFlags.None, ServerEndPoint);
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	#endregion
}
