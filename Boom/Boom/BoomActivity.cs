using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.Xna.Framework;

namespace Boom
{
	[Activity (Label = "Boom_Android", 
	           MainLauncher = true,
	           Icon = "@drawable/icon",
	           Theme = "@style/Theme.Splash",
                AlwaysRetainTaskState=true,
	           LaunchMode=Android.Content.PM.LaunchMode.SingleInstance,
	           ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | 
	                                  Android.Content.PM.ConfigChanges.KeyboardHidden | 
	                                  Android.Content.PM.ConfigChanges.Keyboard)]
	public class BoomActivity : AndroidGameActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create our OpenGL view, and display it
			BoomGame.Activity = this;
			var g = new BoomGame ();
			SetContentView (g.Window);
			g.Run ();
		}
	}
}


