using System;
using System.Data;
using Gtk;
using MythRemoteKeyboard;
using MySql.Data;
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
			try {
				StreamReader Reader = new StreamReader (m_HomeDir + "/.mythtv/mysql.txt");
				string line;
				while ((line = Reader.ReadLine ()) != null) {
					line = line.Trim ();
					if (line.StartsWith ("#"))
						continue;
					
					string[] KeyVal = line.Split ('=');
					if (KeyVal.Length >= 2)
						m_DBparams[KeyVal[0].Trim ()] = KeyVal[1].Trim ();
				}
				
			} catch (Exception ex) {
				Console.WriteLine (ex.ToString ());
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
