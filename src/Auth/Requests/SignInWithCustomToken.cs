namespace Firebase.Auth.Requests
{
	public class SignInWithCustomTokenRequest
	{
		public string Token { get; set; }
		
		public bool ReturnSecureToken { get; set; }
	}

	public class SignInWithCustomTokenResponse
	{		
		public string IdToken { get; set; }
	
		public string RefreshToken { get; set; }

		public int ExpiresIn { get; set; }
	}

	/// <summary>
	/// Uses a custom token generated in a different backend (server) proces to login to firebase.
	/// see: https://firebase.google.com/docs/auth/admin/create-custom-tokens
	/// </summary>
	public class SignInWithCustomToken : FirebaseRequestBase<SignInWithCustomTokenRequest, SignInWithCustomTokenResponse>
	{
		public SignInWithCustomToken(FirebaseAuthConfig config) : base(config)
		{
		}

		protected override string UrlFormat => Endpoints.GoogleSignInWithCustomToken;
	}
}
