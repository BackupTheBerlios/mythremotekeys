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
using System.Threading;

public partial class MainWindow : Gtk.Window
{
	MythDB m_DB = new MythDB ();
	TreeView m_TreeView;
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		this.KeyPressEvent += OnKeyPressed;
		HBox vBox = new HBox ();
		m_TreeView = CreateTreeView ();
		vBox.Add (m_TreeView);
		HBox actionButtons = new HBox ();
		actionButtons.Add (createActionButton ("LiveTV"));
		actionButtons.Add (createActionButton ("Video"));
		actionButtons.Add (createActionButton ("Recordings"));
		actionButtons.Add (createActionButton ("Music"));
		actionButtons.Add(createActionButton("ReConn"));
		vBox.Add (actionButtons);
		this.Add (vBox);
		this.ShowAll ();
	}
	Button createActionButton (string lableName)
	{
		Button but = new Button ();
		but.Clicked += actionButtonPressed;
		but.Name = lableName;
		but.Label = lableName;
		return but;
	}
	void actionButtonPressed (object o, EventArgs e)
	{
		Button but = o as Button;
		Frontend selectedFrontend;
		TreeIter iter;
		if (getSelectedFrontend (out iter, out selectedFrontend)) {
			//selectedFrontend.SendKey("escape");
			if (but.Name == "LiveTV") {
				selectedFrontend.SendCommand ("jump livetv");
			} else if (but.Name == "Video") {
				selectedFrontend.SendCommand ("jump mythvideo");
			} else if (but.Name == "Recordings") {
				selectedFrontend.SendCommand ("jump playbackrecordings");
			} else if (but.Name == "Music") {
				selectedFrontend.SendCommand ("jump playmusic");
			}
			else if ( but.Name == "ReConn"){
				selectedFrontend.Disconnect();
				ThreadPool.QueueUserWorkItem(selectedFrontend.Connect);
			}
			
			m_TreeView.Model.EmitRowChanged (m_TreeView.Model.GetPath (iter), iter);
		}
	}
	
	TreeView CreateTreeView ()
	{
		TreeView frontendView = new TreeView ();
		frontendView.KeyPressEvent += IgnoreKeyPress;
		Gtk.TreeViewColumn hostnameCol = new Gtk.TreeViewColumn ();
		Gtk.CellRendererText hostnameCell = new Gtk.CellRendererText ();
		hostnameCol.Title = "Host";
		hostnameCol.PackStart (hostnameCell, true);
		hostnameCol.SetCellDataFunc (hostnameCell, new Gtk.TreeCellDataFunc (RenderHostname));
		
		TreeViewColumn statusCol = new TreeViewColumn ();
		CellRenderer statusCell = new CellRendererText ();
		statusCol.Title = "";
		statusCol.PackStart (statusCell, true);
		statusCol.SetCellDataFunc (statusCell, new TreeCellDataFunc (RenderStatus));
		
		Gtk.ListStore allFrontends = new Gtk.ListStore (typeof(Frontend));
		foreach (KeyValuePair<string, int> host in m_DB.GetFrontendHostnames ()) {
			Frontend foo = new Frontend (host.Key, host.Value);
			allFrontends.AppendValues (foo);
			ThreadPool.QueueUserWorkItem (foo.Connect);
			//foo.Connect (new object ());
		}
		
		frontendView.Model = allFrontends;
		frontendView.AppendColumn (hostnameCol);
		frontendView.AppendColumn (statusCol);
		return frontendView;
	}
	private void RenderHostname (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		Frontend frontend = (Frontend)model.GetValue (iter, 0);
		if (frontend.Connected == true) {
			(cell as Gtk.CellRendererText).Foreground = "lightgreen";
			//(cell as Gtk.CellRendererText).
		} else {
			(cell as Gtk.CellRendererText).Foreground = "grey";
		}
		(cell as Gtk.CellRendererText).Text = frontend.Hostname;
	}
	private void RenderStatus (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		Frontend fe = (Frontend)model.GetValue (iter, 0);
		if (fe.LastCommand != String.Empty && fe.LastCommandResult != String.Empty) {
			if (!fe.LastCommandResult.StartsWith ("OK")) {
				cell.CellBackground = "red";
			}
		}
		(cell as Gtk.CellRendererText).Text = fe.LastCommand + " " + fe.LastCommandResult;
	}
	[GLib.ConnectBefore]
	private void IgnoreKeyPress (object o, KeyPressEventArgs args)
	{
		OnKeyPressed (o, args);
		Console.WriteLine ("IgnoreKeyPress");
	}
	[GLib.ConnectBefore]
	private void OnKeyPressed (object o, KeyPressEventArgs args)
	{
		Console.WriteLine ("gtk# key read: " + args.Event.Key);
		string key = MythTelnetKey.Parse (args);
		Console.WriteLine ("sending key: \"" + key + "\":");
		Frontend selectedFrontend;
		TreeIter iter;
		if (getSelectedFrontend (out iter, out selectedFrontend)) {
			selectedFrontend.SendKey (key);
			m_TreeView.Model.EmitRowChanged (m_TreeView.Model.GetPath (iter), iter);
		}
		args.RetVal = true;
	}

	bool getSelectedFrontend (out TreeIter iter, out Frontend selectedFrontend)
	{
		TreeModel model;
		TreeSelection selection = m_TreeView.Selection;
		if (selection.GetSelected (out model, out iter)) {
			selectedFrontend = model.GetValue (iter, 0) as Frontend;
			if (selectedFrontend != null) {
				Console.WriteLine ("selected: " + selectedFrontend);
				return true;
			}
		}
		selectedFrontend = null;
		return false;
	}
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}


