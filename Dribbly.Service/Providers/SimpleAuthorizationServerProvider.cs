﻿using Dribbly.Authentication.Enums;
using Dribbly.Authentication.Models;
using Dribbly.Authentication.Models.Auth;
using Dribbly.Core.Helpers;
using Dribbly.Identity.Models;
using Dribbly.Model;
using Dribbly.Model.Account;
using Dribbly.Service.Repositories;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dribbly.Authentication.Attributes;
using Dribbly.Core.Enums;

namespace Dribbly.Service.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// Responsible for validating the client
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            Client client = null;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                context.Validated();
                //context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult<object>(null); 
            }

            using (AuthRepository _repo = new AuthRepository(null, new AuthContext()))
            {
                client = _repo.FindClient(context.ClientId);
            }

            if (client == null)
            {
                context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                return Task.FromResult<object>(null); 
            }

            if (client.ApplicationType == ApplicationTypesEnum.NativeConfidential)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret should be sent.");
                    return Task.FromResult<object>(null); 
                }
                else
                {
                    if (client.Secret != Hash.GetHash(clientSecret))
                    {
                        context.SetError("invalid_clientId", "Client secret is invalid.");
                        return Task.FromResult<object>(null); 
                    }
                }
            }

            if (!client.Active)
            {
                context.SetError("invalid_clientId", "Client is inactive.");
                return Task.FromResult<object>(null); 
            }

            context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated();
            return Task.FromResult<object>(null); 
        }

        /// <summary>
        /// Responsible to validate the username and password when logging in
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

            if (allowedOrigin == null) allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            ApplicationUser user = null;
            PlayerModel account = null;
            List<PermissionModel> userPermissions = new List<PermissionModel>();

            using (var authContext = new AuthContext())
            {

                using (AuthRepository _repo = new AuthRepository(null, authContext))
                {
                    user = await _repo.FindUserByName(context.UserName);

                    if (user == null || await _repo.FindUser(context.UserName, string.Concat(context.Password, user.Salt)) == null)
                    {
                        context.SetError("invalid_grant", "The user name or password is incorrect.");
                        return;
                    }

                    using (AccountRepository _accountRepo = new AccountRepository(authContext, _repo))
                    {
                        account = await _accountRepo.GetAccountByIdentityId(user.Id);
                        if (account.IsDeleted)
                        {
                            // return the same error for security
                            context.SetError("invalid_grant", "The user name or password is incorrect.");
                            return;
                        }
                        else if (account.IsInactive)
                        {
                            _accountRepo.ActivateByUserId(user.Id.ToString());
                            var indexedAccount = authContext.IndexedEntities.Find(account.Id, account.EntityType);
                            indexedAccount.EntityStatus = EntityStatusEnum.Active;
                            await authContext.SaveChangesAsync();
                        }
                    }

                    userPermissions = (await PermissionsRepository.GetUserPermissionsAsync(authContext, user.Id.ToString())).ToList();
                }

            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim("userId", user.Id.ToString()));
            identity.AddClaim(new Claim("accountId", account.Id.ToString()));
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            // Add user claims for authorization
            /// These claims are checked by <see cref="DribblyAuthorize"/>
            foreach (var userPermission in userPermissions)
            {
                identity.AddClaim(new Claim("Permission", userPermission.Value.ToString()));
            }

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    {
                        "username", context.UserName
                    },
                    {
                        "userId", user.Id.ToString()
                    },
                    {
                        "accountId", account.Id.ToString()
                    },
                    {
                        "profilePicture", account.ProfilePhoto?.Url ?? ""
                    }
                });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }


        /// <summary>
        /// Allows us to issue new claims or updating existing claims and contain them into the new access
        /// token generated before sending it to the user
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }

    }
}