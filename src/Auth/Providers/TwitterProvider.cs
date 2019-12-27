﻿namespace Firebase.Auth.Providers
{
    public class TwitterProvider : ExternalAuthProvider
    {
        public override FirebaseProviderType ProviderType => FirebaseProviderType.Twitter;

        protected override string LocaleParameterName => "lang";
    }
}
