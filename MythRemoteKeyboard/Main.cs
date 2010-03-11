
using Gtk;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;

namespace MythRemoteKeyboard
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
		}
	}
}