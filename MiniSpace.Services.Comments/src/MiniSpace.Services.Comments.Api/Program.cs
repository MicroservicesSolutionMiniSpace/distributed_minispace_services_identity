using System.Collections.Generic;
using System.Threading.Tasks;
using Convey;
using Convey.Secrets.Vault;
using Convey.Logging;
using Convey.Types;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MiniSpace.Services.Comments.Application;
using MiniSpace.Services.Comments.Application.Commands;
using MiniSpace.Services.Comments.Application.Dto;
using MiniSpace.Services.Comments.Application.Services;
using MiniSpace.Services.Comments.Infrastructure;

namespace MiniSpace.Services.Identity.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
            => await WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services
                    .AddConvey()
                    .AddWebApi()
                    .AddApplication()
                    .AddInfrastructure()
                    .Build())
                .Configure(app => app
                    .UseInfrastructure()
                    .UseDispatcherEndpoints(endpoints => endpoints
                        .Get("", ctx => ctx.Response.WriteAsync(ctx.RequestServices.GetService<AppOptions>().Name))
                        //.Get<GetComment, IEnumerable<CommentDto>>("comments")
                        .Post<CreateComment>("comments"
                        //    ,afterDispatch: (cmd, ctx) => ctx.Response.Created($"commens/{cmd.PostId}")
                        )
                        .Put<UpdateComment>("comments/{commentID}")
                        .Delete<DeleteComment>("comments/{commentID}"
                        //    ,afterDispatch: (cmd, ctx) => ctx.Response.Created($"commens/{cmd.Id}")
                        //    to chyba w końcu nie potrzebne
                        )
                        .Post<UpdateLike>("comments/{commentID}/like"
                        //
                        // Działą tak, że jak nie było to dodaje do listy a jak był na liście to usuwa z listy
                        //
                        // ,afterDispatch: (cmd, ctx) => ctx.Response.Created($"comments/{cmd.ID}/like")
                        // nie było w spec ale moze można dodać?
                        )
                    )
                )
                .UseLogging()
                .Build()
                .RunAsync();
    }
}
