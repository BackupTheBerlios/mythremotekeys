using Gtk;
using System;
using System.Collections.Generic;
namespace MythRemoteKeyboard
{
	public static class MythTelnetKey
	{
		static List<string> telnetKeys = new List<string> { "#", "$", "%", "&", "(", ")", "*", "+", ",", "-",
		".", "/", ":", ";", "<", "=", ">", "?", "[", "\\",
		"]", "_", "ampersand", "asterisk", "backslash", "backspace", "backtab", "bar", "bracketleft", "bracketright",
		"colon", "comma", "delete", "dollar", "down", "end", "enter", "equal", "escape", "f1",
		"f10", "f11", "f12", "f13", "f14", "f15", "f16", "f17", "f18", "f19",
		"f2", "f20", "f21", "f22", "f23", "f24", "f3", "f4", "f5", "f6",
		"f7", "f8", "f9", "greater", "hash", "home", "insert", "left", "less", "minus",
		"numbersign", "pagedown", "pageup", "parenleft", "parenright", "percent", "period", "pipe", "plus", "poundsign",
		"question", "return", "right", "semicolon", "slash", "space", "tab", "underscore", "up", "|" };

		public static string Parse (KeyPressEventArgs keyPress)
		{
			string gtkKey = keyPress.Event.Key.ToString ().ToLower ();
			if (telnetKeys.Contains (gtkKey))
				return gtkKey;
			if(tranlsateKey(ref gtkKey))
				return gtkKey;
			
			if (gtkKey.Length == 1){
				return gtkKey;	
			}
			
			return string.Empty;
		}

		private static bool tranlsateKey (ref string key)
		{
			string oldkey = key;
			
			if (oldkey == "l1")
				key = "f11"; 
			else if (oldkey == "l2")
				key = "f12";
			else if (oldkey == "page_up")
				key = "pageup";
			else if (oldkey == "next")
				key = "pagedown";
			
			return (oldkey != key);
			
		}
	}
}
