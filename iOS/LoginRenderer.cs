using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using facebookLogin;
using facebookLogin.iOS;
using Xamarin.Auth;
using System.Diagnostics;

//[assembly:ExportRenderer (typeof(facebookLoginPage), typeof(LoginRenderer))]
namespace facebookLogin.iOS
{
	public class LoginRenderer : PageRenderer
	{
		public LoginRenderer()
		{
		}

		bool isShown;

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			if (!isShown)
			{
				isShown = true;

				Debug.WriteLine(@"yes it worked 1");

				var auth = new OAuth2Authenticator(
					clientId: "755702461248187",
					//clientSecret: "a3e2dd2872140d36ab68858176b8ed41",
					scope: "token",
					authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth"),
					redirectUrl: new Uri("https://www.facebook.com/connect/login_success.html")
					//accessTokenUrl: new Uri("https://m.facebook.com/dialog/oauth/token")
				);

				auth.AllowCancel = true;

				auth.Completed += OnAuthenticationCompleted;

				auth.Error += (sender, e) => {
					Debug.WriteLine(e.Message);
				};

				PresentViewController(auth.GetUI(), true, null);

				Debug.WriteLine(@"yes it worked");
			}
			else
			{
				Debug.WriteLine(@"called twice");
			}

		}

		async void OnAuthenticationCompleted(object sender, AuthenticatorCompletedEventArgs e)
		{
			Debug.WriteLine("AUTH Completed!");
			if (e.IsAuthenticated)
			{
				Debug.WriteLine("AUTHENTICATED!!");
			}

			await DismissViewControllerAsync(true);
		}
	}
}
