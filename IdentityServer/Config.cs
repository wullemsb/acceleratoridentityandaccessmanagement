// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };

        public static IEnumerable<ApiResource> Apis =>
           new ApiResource[]
           {
                 new ApiResource("api1", "My API")
                 {
                     UserClaims=new List<string>{ IdentityModel.JwtClaimTypes.Name }
                 }
           };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
            new Client
        {
            ClientId = "angularClient",
            ClientName = "JavaScript Client",
            //ClientUri = "http://identityserver.io",
            RequireConsent=true,

            AllowedGrantTypes = GrantTypes.Implicit,
            AllowAccessTokensViaBrowser = true,

            RedirectUris =           { "http://localhost:4200" },
            PostLogoutRedirectUris = { "http://localhost:4200" },
            AllowedCorsOrigins =     { "http://localhost:4200" },

            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                "api1"
            }
          }
            };
        public static List<TestUser> Users =>
             new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password",

                    Claims = new []
                    {
                        new Claim("name", "Alice"),
                        new Claim("website", "https://alice.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password",

                    Claims = new []
                    {
                        new Claim("name", "Bob"),
                        new Claim("website", "https://bob.com")
                    }
                }
            };
    }
}