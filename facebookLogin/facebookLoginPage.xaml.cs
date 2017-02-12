using Xamarin.Forms;
using System.Diagnostics;
using System;

namespace facebookLogin
{
	public partial class facebookLoginPage : ContentPage
	{
		public facebookLoginPage()
		{
			InitializeComponent();

			// initialize facebook class
			FacebookOAuth fb = new FacebookOAuth();

			// property initialization
			fb.ClientId = "{YOUR_APPID}";
			fb.ResponseType = "token";
			fb.Scope = "email";
			fb.GraphFields = "name,email,gender,picture,age_range,devices,is_verified";

			// create events for fb
			fb.OnGraphSuccess += new OnGraphSuccessEventHandler(OnGraphSuccess);
			fb.OnGraphFailure += new OnGraphFailureEventHandler(OnGraphFailure);

			// initalize new web view
			var webView = fb.InitializeWebView();

			// create navigated delegate to check when access token has arrived
			webView.Navigated += async (sender, e) =>
			{

				try
				{
					fb.ExtractAccessToken(e.Url);
					if (fb.AccessToken != null)
					{
						Debug.WriteLine(@"access token: " + fb.AccessToken);
						await fb.GetFacebookProfileAsync(fb.AccessToken, fb.GraphFields);
					}

				}
				catch (Exception ex)
				{
					Debug.WriteLine(@"received error: " + ex.ToString());
				}

			};

			// set content to web view
			Content = webView;
		}

		public void OnGraphSuccess(string jsonData)
		{
			Debug.WriteLine("Success!: " + jsonData);
		}

		public void OnGraphFailure(string errorMessage)
		{
			Debug.WriteLine("Failed!: " + errorMessage);
		}

	}
}
