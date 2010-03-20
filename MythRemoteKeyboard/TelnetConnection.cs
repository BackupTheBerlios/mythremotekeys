using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace MythRemoteKeyboard
{
	public class TelnetConnection
	{
		TcpClient tcpClient = null;
		Stream tcpStream = null;
		ASCIIEncoding asciiEnc = null;
		public bool Connected {
			get { return tcpClient.Connected; }
		}
		
		public TelnetConnection (){
			tcpClient = new TcpClient ();
			asciiEnc = new ASCIIEncoding ();
		}
		
		public bool Connect(string host){
			return Connect (host, 6546);
		}
		
		public bool Connect(string host, int port){
			try {
				Console.WriteLine ("Connecting to " + host + ":" + port + "...");
				tcpClient.Connect (host, port);
				tcpStream = tcpClient.GetStream ();
				string ans = ReadAnswer();
				Console.WriteLine (ans);
				return true;
			} catch (Exception e) {
				Console.WriteLine (host + "failed to connect: " + e.StackTrace);
				return false;
			}
		}
		
		public String SendCommand (string command){
			if (!Connected){
				return "Not connected, can't send \""+command+"\"";	
			}
			command += "\n";
			byte[] ba = asciiEnc.GetBytes (command);
			tcpStream.Write (ba, 0, ba.Length);
			return ReadAnswer ();
		}
		
		private string ReadAnswer (){
			string readString = String.Empty;
			byte[] bb = new byte[256];
			int count;
			Thread.Sleep (100);
			try {
				while (tcpClient.Available > 0) {
					count = tcpStream.Read (bb, 0, bb.Length);
					readString += asciiEnc.GetString (bb, 0, count);
					Thread.Sleep (100);
				}
				return readString;
			} catch (Exception e) {
				Console.WriteLine (e.ToString ());
				return "Could not send command.";
			}
		}
		
		public void Disconnect (){
			try {
				tcpStream.Close();
				tcpClient.Close ();
			} catch {}
		}
		
		~TelnetConnection (){
			Disconnect();
		}
	}
}
