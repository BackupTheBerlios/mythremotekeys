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
		public TelnetConnection ()
		{
			tcpClient = new TcpClient ();			
			asciiEnc = new ASCIIEncoding ();
		}
		public bool Connect (string host)
		{
			try {
				Console.WriteLine ("Connecting to "+host+".....");
				tcpClient.Connect (host, 6546);
				tcpStream = tcpClient.GetStream ();
				string ans = ReadAnswer();
				Console.WriteLine (ans);
				return true;
			} catch (Exception e) {
				Console.WriteLine ("Error..... " + e.StackTrace);
				return false;
			}
			
		}
		public String SendCommand (string command)
		{
			Console.WriteLine ("transmitting string: \""+ command +"\"");
			command += "\n";
			byte[] ba = asciiEnc.GetBytes (command);
			tcpStream.Write (ba, 0, ba.Length);
			return ReadAnswer ();
		}
		public bool SendKeypress (string key)
		{
			string command = "key " + key;
			string answer = SendCommand (command);
			return answer.StartsWith ("OK");
		}
		private string ReadAnswer ()
		{
			string readString = String.Empty;
			byte[] bb = new byte[256];
			int count;
			Thread.Sleep(200);
			while(tcpClient.Available > 0) {
				count = tcpStream.Read (bb, 0, bb.Length);
				readString += asciiEnc.GetString(bb,0,count);
				Thread.Sleep(200);
			}
			Console.WriteLine ("frontend answer: " + readString);
			return readString;
		}
		public void Disconnect(){
			SendCommand("exit");
		}
		~TelnetConnection ()
		{
			try{
				tcpClient.Close ();
			}
			catch{}
		}
	}
}
