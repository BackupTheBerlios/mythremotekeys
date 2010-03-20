
using System;

namespace MythRemoteKeyboard
{
	public class Frontend
	{
		string m_Hostname;
		int m_Port;
		string m_LastCommand = String.Empty;
		string m_LastCommandResult = String.Empty;
		object m_Lock = new object ();
		TelnetConnection m_TelnetConn;

		public string Hostname {
			get { return m_Hostname; }
		}
		public bool Connected {
			get { return m_TelnetConn.Connected; }
		}
		public string LastCommandResult {
			get { return m_LastCommandResult; }
		}
		public string LastCommand {
			get { return m_LastCommand; }
		}
		public Frontend (string hostname, int port)
		{
			m_Hostname = hostname;
			m_Port = port;	
		}
		~Frontend ()
		{
			Disconnect();
		}
		public void Connect (object status)
		{
			m_TelnetConn = new TelnetConnection ();
			lock (m_Lock) {
				Console.WriteLine (m_Hostname + ":" + m_Port);
				m_TelnetConn.Connect(m_Hostname, m_Port);
			}
		}
		public void Disconnect(){
			if (m_TelnetConn != null){
				SendCommand("exit");
				m_TelnetConn.Disconnect();	
			}
		}
		public void SendKey (string key)
		{
			if (Connected && key != string.Empty) {				
					string command = "key " + key;
					SendCommand(command);				
			}
		}
		public void SendCommand (string command)
		{
			lock (m_Lock){
				m_LastCommand = command;
				string ans = m_TelnetConn.SendCommand(m_LastCommand);
				m_LastCommandResult = ans.Replace("\r\n# ",""); //remove telnet prompt..
			}
		}
		
		public override string ToString ()
		{
			return string.Format ("[Frontend: Hostname={0}, Connected={1}, LastCommandResult={2}, LastCommand={3}]", Hostname, Connected, LastCommandResult, LastCommand);
		}		
	}
}
