using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RightNow.AddIns.AddInViews;
using inContact.Integration.RightNow.ConnectService;

namespace inContact.Integration.RightNow
{
    public class CredentialProvider 
    {
        private RightNowObjectProvider _rightNowObjectProvider;
        private IGlobalContext _globalContext;

        public CredentialProvider(IGlobalContext globalContext)
        {
            _globalContext = globalContext;
            _rightNowObjectProvider = new RightNowObjectProvider(globalContext);
        }

        public OAuthInfo GetCredentials()
        {
            string condition = string.Format("WHERE Account = {0}", _globalContext.AccountId);
            OAuthInfo accountInfo = _rightNowObjectProvider.GetObject<OAuthInfo>(condition);
            if (accountInfo == null)
            {
                Logger.Log.Error("CredentilaProvider <<< OAuthInfo returns null from RNT environment.");
                accountInfo = new OAuthInfo();
            }
            return accountInfo;
        }

        public void SaveCredentials(OAuthInfo oauthInfo)
        {
            if (oauthInfo.Account == _globalContext.AccountId)
            {
                _rightNowObjectProvider.UpdateObject<OAuthInfo>(oauthInfo, oauthInfo.ID);
            }
            else
            {
                oauthInfo.Account = _globalContext.AccountId;
                _rightNowObjectProvider.CreateObject<OAuthInfo>(oauthInfo);
            }
        }
    }
}
