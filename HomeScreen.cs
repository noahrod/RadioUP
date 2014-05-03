using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using CustomRowView.Services;
using System.Net;
using System.IO;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomRowView {
	[Activity(Theme = "@style/Theme.Splash", Label = "MediosUP", MainLauncher = true, Icon = "@drawable/icon")]
    public class HomeScreen : Activity{//, View.IOnClickListener {

        List<TableItem> tableItems = new List<TableItem>();
        ListView listView;
		public static List<string> links = new List<string>();
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
			RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.HomeScreen);
            listView = FindViewById<ListView>(Resource.Id.List);

			tableItems.Add(new TableItem() { Heading = "DiarioUP", SubHeading = "Cargadno Contenido", ImageResourceId = Resource.Drawable.ESCUDO_UP });

            listView.Adapter = new HomeScreenAdapter(this, tableItems);

            listView.ItemClick += OnListItemClick;
			var play = FindViewById<ImageButton> (Resource.Id.playButton);
			var stop = FindViewById<ImageButton> (Resource.Id.stopButton);
			play.Click += (sender,args) => {
				play.Visibility = ViewStates.Gone;
				stop.Visibility = ViewStates.Visible;
				SendAudioCommand (StreamingBackgroundService.ActionPlay);
			};
			stop.Click += (sender,args) => {
				stop.Visibility = ViewStates.Gone;
				play.Visibility = ViewStates.Visible;
				SendAudioCommand (StreamingBackgroundService.ActionStop);
			};

			var urlrss = "http://diarioup.com/feed/";
			XmlTextReader reader = new XmlTextReader (urlrss.TrimStart ());
			reader.MoveToContent ();
			reader.ReadStartElement ();
			int i = 0;
			List<string> titulos = new List<string>();

			List<string> description = new List<string>();
			List<string> media = new List<string>();
			while(reader.Read()){
				i++;
				if (i > 20) {
						if (reader.Name == "title") {
							titulos.Add(reader.ReadString ());	
							Console.WriteLine (reader.ReadString ());
						}
						if (reader.Name == "link") {
							links.Add(reader.ReadString ());
							Console.WriteLine (reader.ReadString ());
						}
						if (reader.Name == "description") {
							description.Add(reader.ReadString ());
							Console.WriteLine (reader.ReadString ());
						}
						//if (reader.Name == "media:thumbnail") {
						//	media [e-1] = reader.GetAttribute("url");
						//}
					}

			}
			tableItems.Clear ();
			for(int j=0; j<titulos.Count; j++){
				string Shortd =""; 
				if (StripHTML (description[j]).Length > 170) {
					Shortd = StripHTML (description[j]).Substring (0, 170) + "...";
				} else {
					Shortd = StripHTML (description[j]);
				}
				tableItems.Add(new TableItem() { Heading = titulos[j], SubHeading = Shortd, ImageResourceId = Resource.Drawable.ESCUDO_UP });
			}
			listView.Adapter = new HomeScreenAdapter(this, tableItems);

        }

        protected void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
			var t = tableItems[e.Position];
			//Android.Widget.Toast.MakeText(this, links[e.Position], Android.Widget.ToastLength.Short).Show();
			var uri = Android.Net.Uri.Parse (links[e.Position]);
			var intent = new Intent (Intent.ActionView, uri);
			StartActivity (intent);
			Console.WriteLine("Clicked on " + t.Heading);
        }
		private void SendAudioCommand(string action){
			var intent = new Intent (action);
			StartService (intent);
		}
		static string StripHTML (string inputString){
			string HTML_TAG_PATTERN = "<.*?>";
			string COLOR_TAG_PATTERN = "[&#0-9?;]";
			return Regex.Replace(Regex.Replace(inputString, HTML_TAG_PATTERN, string.Empty), COLOR_TAG_PATTERN, string.Empty);
		}

    }
}