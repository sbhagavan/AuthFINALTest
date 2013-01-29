using Funq;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace ServiceStack.Hello
{
    [Authenticate]
    public class Hello
    {
        public string Name { get; set; }
    }

    public class HelloResponse
    {
        public string Result { get; set; }
    }

    public class HelloService : IService<Hello>
    {
        public object Execute(Hello request)
        {
            return new HelloResponse { Result = "Hello, " + request.Name };
        }
    }

    public class Global : System.Web.HttpApplication
    {
        public class HelloAppHost : AppHostBase
        {
            //Tell Service Stack the name of your application and where to find your web services
            public HelloAppHost() : base("Hello Web Services", typeof(HelloService).Assembly) { }

            public override void Configure(Container container)
            {
                //register user-defined REST-ful urls
                Routes
                  .Add<Hello>("/hello")
                  .Add<Hello>("/hello/{Name}");

                container.Register<ICacheClient>(new MemoryCacheClient());
                container.Register<ISessionFactory>(c => new SessionFactory(
                                                    c.Resolve<ICacheClient>()));

                //Register AuthFeature with custom user session and Basic auth provider
                Plugins.Add(new AuthFeature(() => new AuthUserSession(), new IAuthProvider[] {
            new BasicAuthProvider()
        }));

                var userRep = new InMemoryAuthRepository();
                container.Register<IUserAuthRepository>(userRep);

                string hash;
                string salt;
                new SaltedHash().GetHashAndSaltString("test", out hash, out salt);
                userRep.CreateUserAuth(new UserAuth
                {
                    Id = 1,
                    DisplayName = "DisplayName",
                    Email = "as@if.com",
                    UserName = "john",
                    FirstName = "FirstName",
                    LastName = "LastName",
                    PasswordHash = hash,
                    Salt = salt,
                }, "test");
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            new HelloAppHost().Init();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}