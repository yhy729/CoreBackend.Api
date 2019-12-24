using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBackend.AuthServer.Configuration
{
    public class InMemoryConfiguration
    {
        public static IEnumerable<ApiResource> ApiResources()
        {
            return new[]
            {
                new ApiResource("socialnetwork","社交网络")
            };
        }

        public static IEnumerable<Client> Clients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "socialnetwork",
                    ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new [] { "socialnetwork" }
                },
                new Client
                {
                    /*
                     * ClientId要和MvcClient里面指定的名称一致.
OAuth是使用Scopes来划分Api的, 而OpenId Connect则使用Scopes来限制信息, 例如使用offline access时的Profile信息, 还有用户的其他细节信息. 
这里GrantType要改为Implicit. 使用Implicit flow时, 首先会重定向到Authorization Server, 然后登陆, 然后Identity Server需要知道是否可以重定向回到网站, 如果不指定重定向返回的地址的话, 我们的Session有可能就会被劫持. 
RedirectUris就是登陆成功之后重定向的网址, 这个网址(http://localhost:5002/signin-oidc)在MvcClient里, openid connect中间件使用这个地址就会知道如何处理从authorization server返回的response. 这个地址将会在openid connect 中间件设置合适的cookies, 以确保配置的正确性.
而PostLogoutRedirectUris是登出之后重定向的网址. 有可能发生的情况是, 你登出网站的时候, 会重定向到Authorization Server, 并允许从Authorization Server也进行登出动作.
最后还需要指定OpenId Connect使用的Scopes, 之前我们指定的socialnetwork是一个ApiResource. 而这里我们需要添加的是让我们能使用OpenId Connect的SCopes, 这里就要使用Identity Resources. Identity Server带了几个常量可以用来指定OpenId Connect预包装的Scopes. 上面的AllowedScopes设定的就是我们要用的scopes, 他们包括 openid Connect和用户的profile, 同时也包括我们之前写的api resource: "socialnetwork". 要注意区分, 这里有Api resources, 还有openId connect scopes(用来限定client可以访问哪些信息), 而为了使用这些openid connect scopes, 我们需要设置这些identity resoruces, 这和设置ApiResources差不多:
                     */
                    ClientId = "mvc_implicit",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RedirectUris = { "http://localhost:5002/signin-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:5002/signout-callback-oidc"},
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "socialnetwork"
                    }
                }
            };
        }

        public static IEnumerable<TestUser> User()
        {
            return new[]
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "mail@qq.com",
                    Password = "password"
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>{
                 new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

    }
}
