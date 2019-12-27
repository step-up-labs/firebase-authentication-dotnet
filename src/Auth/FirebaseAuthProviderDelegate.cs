﻿using System.Threading.Tasks;

namespace Firebase.Auth
{
    /// <summary>
    /// Delegate which should invoke the passed uri for external authentication and return the final redirect uri.
    /// </summary>
    /// <param name="uri"> Uri to take user to. </param>
    /// <returns> Redirect uri that user lands on. </returns>
    public delegate Task<string> ExternalSignInDelegate(string uri);
}
