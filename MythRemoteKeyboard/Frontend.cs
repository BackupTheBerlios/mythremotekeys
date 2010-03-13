
using System;

namespace MythRemoteKeyboard
{
	public class Frontend
	{
		string m_Hostname;
		int m_Port;
		bool m_Connected;
		string m_LastCommand = String.Empty;
		string m_LastCommandResult = String.Empty;
		object m_Lock = new object ();
		TelnetConnection m_TelnetConn;

		public string Hostname {
			get { return m_Hostname; }
		}
		public bool Connected {
			get { return m_Connected; }
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
			m_TelnetConn = new TelnetConnection ();
		}
		public void Connect (object status)
		{
			lock (m_Lock) {
				Console.WriteLine (m_Hostname + ":" + m_Port);
				if (m_TelnetConn.Connect (m_Hostname, m_Port))
					m_Connected = true;
			}
		}
		public void SendKey (string key)
		{
			if (m_Connected && key != string.Empty) {
				lock (m_Lock) {
					m_LastCommand = "key " + key;
					string ans = m_TelnetConn.SendCommand (m_LastCommand);
					m_LastCommandResult = ans.Replace("\r\n# ",""); //remove telnet prompt..
				}
			}
		}
		~Frontend ()
		{
			if (m_Connected)
				m_TelnetConn.Disconnect ();
		}
		public override string ToString ()
		{
			return string.Format ("[Frontend: Hostname={0}, Connected={1}, LastCommandResult={2}, LastCommand={3}]", Hostname, Connected, LastCommandResult, LastCommand);
		}

		public void SendCommand (string par1)
		{
			lock (m_Lock){
				m_LastCommand = par1;
				string ans = m_TelnetConn.SendCommand(m_LastCommand);
				m_LastCommandResult = ans.Replace("\r\n# ",""); //remove telnet prompt..
			}
		}
	}
}
