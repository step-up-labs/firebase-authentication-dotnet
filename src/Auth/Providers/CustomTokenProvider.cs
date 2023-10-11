using Firebase.Auth.Requests;
using System.Linq;
using System.Threading.Tasks;
using static Firebase.Auth.Providers.EmailProvider;

namespace Firebase.Auth.Providers
{
	public class CustomTokenProvider : FirebaseAuthProvider
	{
		private SignInWithCustomToken signInWithCustomToken;
		private GetAccountInfo getAccountInfo;

		internal override void Initialize(FirebaseAuthConfig config)
		{
			base.Initialize(config);

			this.signInWithCustomToken = new SignInWithCustomToken(config);
			this.getAccountInfo = new GetAccountInfo(this.config);
		}

		public static AuthCredential GetCredential(string customToken)
		{
			return new CustomTokenCredential
			{
				ProviderType = FirebaseProviderType.CustomToken,
				CustomToken = customToken				
			};
		}

		public Task<UserCredential> SignInUserAsync(string customToken)
		{
			return this.SignInWithCredentialAsync(GetCredential(customToken));
		}

		public override FirebaseProviderType ProviderType => FirebaseProviderType.CustomToken;
		
		protected internal override async Task<UserCredential> SignInWithCredentialAsync(AuthCredential credential)
		{
			var ec = (CustomTokenCredential)credential;

			var response = await this.signInWithCustomToken.ExecuteAsync(new SignInWithCustomTokenRequest
			{
				Token = ec.CustomToken,
				ReturnSecureToken = true
			}).ConfigureAwait(false);

			var user = await this.GetUserInfoAsync(response.IdToken).ConfigureAwait(false);
			var fc = new FirebaseCredential
			{
				ExpiresIn = response.ExpiresIn,
				IdToken = response.IdToken,
				RefreshToken = response.RefreshToken,
				ProviderType = FirebaseProviderType.EmailAndPassword
			};

			return new UserCredential(new User(this.config, user, fc), ec, OperationType.SignIn);
		}

		protected internal override Task<UserCredential> LinkWithCredentialAsync(string idToken, AuthCredential credential)
		{
			// Anonnymouse accounts or relinking not supported with this method
			throw new System.NotSupportedException();			
		}

		private async Task<UserInfo> GetUserInfoAsync(string idToken)
		{
			var getResponse = await this.getAccountInfo.ExecuteAsync(new IdTokenRequest { IdToken = idToken }).ConfigureAwait(false);
			var user = getResponse.Users[0];

			return new UserInfo
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				IsEmailVerified = user.EmailVerified,
				Uid = user.LocalId,
				PhotoUrl = user.PhotoUrl,
				IsAnonymous = false
			};
		}

		internal class CustomTokenCredential : AuthCredential
		{
			public string CustomToken { get; set; }
		}
	}


	
}
