using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bev.RedSignal.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Bev.RedSignal.Api.SignalR
{
    [Authorize]
    public class RedHub : Hub
    {
        private readonly IUserQueryString _userQueryString;

        public RedHub(IUserQueryString userQueryString)
        {
            _userQueryString = userQueryString;
        }

        public override Task OnConnectedAsync()
        {
            var username = Context.GetHttpContext().Request.Query.FirstOrDefault(x => x.Key == "ad").Value;
            if (!_userQueryString.DictUsers.ContainsKey(username))
            {
                _userQueryString.DictUsers.TryAdd(username, Context.ConnectionId);
            }

            Console.WriteLine("--> Querystring user: " + username);
            Console.WriteLine("--> ConnectionId user: " + Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var username = Context.GetHttpContext().Request.Query.FirstOrDefault(x => x.Key == "ad").Value;
            if (_userQueryString.DictUsers.ContainsKey(username))
            {
                _userQueryString.DictUsers.Remove(username);
            }

            Console.WriteLine("--> Connection Closed: " + Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task Hi()
        {
            Console.WriteLine("--> Hi - UserIdentifier: " + Context.UserIdentifier);
            Console.WriteLine("--> Hi - User: " + GetUsername());

            await Task.CompletedTask;
        }

        private string GetUsername()
        {
            return Context.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        ///https://docs.microsoft.com/en-us/aspnet/core/signalr/streaming?view=aspnetcore-6.0
        public async IAsyncEnumerable<int> Counter(
                int count,
                int delay,
                [EnumeratorCancellation]
                CancellationToken cancellationToken)
        {
            for (var i = 0; i < count; i++)
            {
                // Check the cancellation token regularly so that the server will stop
                // producing items if the client disconnects.
                cancellationToken.ThrowIfCancellationRequested();

                yield return i;

                // Use the cancellationToken in other APIs that accept cancellation
                // tokens so the cancellation can flow down to them.
                await Task.Delay(delay, cancellationToken);
            }
        }
    }
}