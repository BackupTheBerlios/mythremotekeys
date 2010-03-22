using System;
using System.Data;
using Gtk;
using MythRemoteKeyboard;
using MySql.Data;
using System.Xml;

using System.Collections.Generic;
using System.Collections;
using MySql.Data.MySqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
//todo: read network control port..
namespace MythRemoteKeyboard
{
	public class MythDB
	{
		string m_HomeDir = Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
		Hashtable m_DBparams = new Hashtable ();
		public MythDB ()
		{
			Console.WriteLine ("Reading configuration:");
			XmlTextReader reader = new XmlTextReader (m_HomeDir + "/.mythtv/config.xml");
			string tmpName,tmpValue= String.Empty;
			while (reader.Read ()) {
				if (reader.NodeType == XmlNodeType.Element) {
					tmpName = reader.Name;
					reader.Read ();
					if (reader.NodeType == XmlNodeType.Text) {
						tmpValue = reader.Value;						
						Console.WriteLine (tmpName + "='" + reader.Value + "'");
						m_DBparams[tmpName] = reader.Value;											
					}
				}								
			}			
		}

		public Dictionary<String, int> GetFrontendHostnames ()
		{
			string connectionString = "Server=" + m_DBparams["DBHostName"] + ";" + "Database=" + m_DBparams["DBName"] + ";" + "User ID=" + m_DBparams["DBUserName"] + ";" + "Password=" + m_DBparams["DBPassword"] + ";" + "Pooling=false";			
			IDbConnection dbcon;
			dbcon = new MySqlConnection (connectionString);
			dbcon.Open ();
			IDbCommand dbcmd = dbcon.CreateCommand ();
			string sql = "SELECT DISTINCT hostname FROM settings WHERE hostname IS NOT NULL AND value = 'NetworkControlEnabled' AND data = '1'";
			dbcmd.CommandText = sql;
			IDataReader reader = dbcmd.ExecuteReader ();
			Dictionary<string, int> hosts = new Dictionary<string, int> ();
			while (reader.Read ()) {
				hosts.Add ((string)reader["hostname"], 6546);
			}
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			dbcmd = null;
			dbcon.Close ();
			dbcon = null;
			
			return hosts;
		}
	}
}
