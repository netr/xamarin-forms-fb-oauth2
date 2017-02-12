using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace facebookLogin
{
	public delegate void OnGraphSuccessEventHandler(string jsonData);
	public delegate void OnGraphFailureEventHandler(string errorMessage);

	public class FacebookOAuth
	{

		#region variables
		#region private variables
		protected string _clientid;
		protected string _responseType;
		protected string _scope;
		protected string _graphFields;
		protected string _accessToken;
		#endregion

		#region properties
		public string ClientId
		{
			get { return _clientid; }
			set { _clientid = value; }
		}
		public string ResponseType
		{
			get { return _responseType; }
			set { _responseType = value; }
		}
		public string Scope
		{
			get { return _scope; }
			set { _scope = value; }
		}
		public string GraphFields
		{
			get { return _graphFields; }
			set { _graphFields = value; }
		}
		public string AccessToken
		{
			get { return _accessToken; }
			set { _accessToken = value; }
		}
		#endregion
		#endregion

		#region events
		public event OnGraphSuccessEventHandler OnGraphSuccess;
		public event OnGraphFailureEventHandler OnGraphFailure;
		#endregion

		/// <summary>
		/// Initializer
		/// </summary>
		/// <param name="clientId">facebook app id</param>
		/// <param name="responseType">typically: token</param>
		/// <param name="scope">scope of requests from user</param>
		/// reference: https://developers.facebook.com/docs/facebook-login/manually-build-a-login-flow
		public FacebookOAuth(string ClientId = null, string ResponseType = null, string Scope = null)
		{
			_clientid = ClientId;
			_responseType = ResponseType;
			_scope = Scope;
		}

		/// <summary>
		/// Extracts the access token from url
		/// </summary>
		public string ExtractAccessToken(string url)
		{
			if (url.Contains("access_token") && url.Contains("&expires_in="))
			{
				Regex r = new Regex(@"access_token\=([^\&]+)");
				Match m = r.Match(url);
				if (m == null)
					return null;

				// store it before returning it, just in case you want to call it later using this class
				_accessToken = m.Groups[1].Value;

				return _accessToken;
			}

			if (url.Contains("error_reason"))
			{
				Regex r = new Regex(@"error\=([^\&]+)");
				Match m = r.Match(url);
				throw new Exception(m.Groups[1].Value);
			}

			return null;
		}

		/// <summary>
		/// Gets the facebook profile async.
		/// </summary>
		public async Task GetFacebookProfileAsync(string accessToken, string fields)
		{
			var requestUrl = "https://graph.facebook.com/v2.7/me/" +
							 "?fields=" + fields +
							 "&access_token=" + accessToken;

			try
			{
				var httpClient = new HttpClient();
				HttpResponseMessage response = await httpClient.GetAsync(requestUrl);
				response.EnsureSuccessStatusCode();

				var userJson = await response.Content.ReadAsStringAsync();

				// raise event for success
				OnGraphSuccess(userJson);

			}
			catch (HttpRequestException e)
			{
				Debug.WriteLine("profile error: " + e.Message);

				// raise event for failure
				OnGraphFailure(e.Message);
			}
		}

		/// <summary>
		/// Generates the request URL for facebook auth
		/// </summary>
		public string GenerateRequestUrl()
		{
			if(_clientid == null)
				throw new NullReferenceException("Missing Facebook Client Id (App id)");
			
			return "https://www.facebook.com/v2.8/dialog/oauth?client_id=" + _clientid +
					"&display=popup" +
					(_scope != null ? "&scope=" + _scope : "") +
					"&redirect_uri=https://www.facebook.com/connect/login_success.html" +
					(_responseType != null ? "&response_type=" + _responseType : "");
		}

	}
}
