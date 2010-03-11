using System;
using System.Data;
using Gtk;
using MythRemoteKeyboard;
using MySql.Data;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;

public partial class MainWindow : Gtk.Window
{
	MythRemoteKeyboard.TelnetConnection m_MyConnection = new MythRemoteKeyboard.TelnetConnection ();
	MythDB db = new MythDB();
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

		this.KeyPressEvent += OnKeyPressed;
		List<String> allFrontends = db.GetFrontendHostnames();
		Console.WriteLine ("All frontends: ");
		foreach (string foo in allFrontends) {
			Console.WriteLine (foo);
		}
		Console.WriteLine ("Online frontends:");
		List<string> frontends = FindOnlineFrontends (allFrontends);
		foreach (string online in frontends) {
			Console.WriteLine (online);
			Hosts.AppendText(online);
		}
		m_MyConnection.Connect(frontends[0]);
		
	}

	


	List<String> FindOnlineFrontends (List<string> hostnames)
	{
		List<string> upHosts = new List<string> ();
		foreach (string host in hostnames) {
			try {
				TcpClient client = new TcpClient ();
				client.Connect (host, 6546);
				upHosts.Add (host);
				Console.WriteLine (host + " up.");
				client.Close ();
			} catch {
				Console.WriteLine (host + " down.");
			}
		}
		return upHosts;
	}
	[GLib.ConnectBefore]
	private void OnKeyPressed (object o, KeyPressEventArgs args)
	{
		Console.WriteLine ("gtk# key read: " + args.Event.Key);
		string key = MythTelnetKey.Parse (args);
		Console.WriteLine ("sending key: \"" + key + "\":");
		SendKey.Text = key;
		if (m_MyConnection.SendKeypress (key))
			SendKey.Text += " OK";
		else
			SendKey.Text += " failed";
	}



	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		m_MyConnection.Disconnect ();
		Application.Quit ();
		a.RetVal = true;
	}
}


