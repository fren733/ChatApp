using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        #region Fields

        private readonly AppDbContext _context;

        #endregion

        #region Constructor

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        public override Task OnConnectedAsync()
        {
            _context.Connections.Add(new Connection { ConnectionId = Context.ConnectionId, Username = Context.User.Identity.Name });
            _context.SaveChanges();

            string username = Context.User.Identity.Name;
            string isOnline = "true";

            Clients.All.SendAsync("ReceiveConnectionInfo", username, isOnline, null);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            User user         = _context.Users.Where(x => x.UserName == Context.User.Identity.Name).FirstOrDefault();
            user.LastOnline   = DateTime.Now;
            string username   = Context.User.Identity.Name;
            string lastOnline = _context.Users.Where(x => x.UserName == username).FirstOrDefault().LastOnline.ToString();
            string isOnline   = "false";

            List<Connection> garbage = _context.Connections.Where(x => x.Username == Context.User.Identity.Name).ToList();

            if (garbage!= null && _context.Connections != null)
            {
                _context.Connections.RemoveRange(garbage);
                _context.SaveChanges();
            }

            Clients.All.SendAsync("ReceiveConnectionInfo", username, isOnline, lastOnline);

            return base.OnDisconnectedAsync(exception);
        }

        #endregion
    }
}


