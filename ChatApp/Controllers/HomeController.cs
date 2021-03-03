using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChatApp.Models;
using Microsoft.AspNetCore.Identity;
using ChatApp.Hubs;
using ChatApp.Data;
using ChatApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatApp.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class HomeController : Controller
    {
        #region Private Fields

        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        #endregion

        #region Constructor

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, AppDbContext context, IHubContext<ChatHub> hubContext)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _hubContext = hubContext;
        }

        #endregion

        #region Views Methods

        public IActionResult Index()
        {
            User user = _context.Users.Where(x => x.UserName.Equals(User.Identity.Name)).SingleOrDefault();
            List<UserGroup> userGroups = _context.UserGroups.Where(x => x.UserId == user.Id).ToList();
            List<Group> groups = new List<Group>();
            List<GroupViewModel> groupVMs = new List<GroupViewModel>();

            foreach (var ug in userGroups)
            {
                Group group = _context.Groups.Where(x => x.Id.Equals(ug.GroupId)).SingleOrDefault();
                Image image = _context.Images.Where(x => x.GroupId.Equals(group.Id)).SingleOrDefault();
                string lastMessage = null;
                bool amIKing = false;

                if (group.LastMessage != null)
                {
                    if (group.LastMessage.Length > 36)
                        lastMessage = group.LastMessage.Substring(0, 35) + "...";
                    else
                        lastMessage = group.LastMessage;
                }

                if (group.OwnerId == user.Id)
                    amIKing = true;

                groupVMs.Add(new GroupViewModel
                {
                    Id = group.Id,
                    Name = group.Name,
                    LastMessage = lastMessage,
                    Src = ReadImage(image),
                    AmIKing = amIKing,
                    LastOnline = group.LastOnline.ToString()
                });
            }

            ViewBag.Groups = groupVMs.OrderByDescending(x => x.LastOnline);
            ViewBag.Users = GetFriends();

            return View(ViewBag);
        }

        public IActionResult Profile(string username)
        {
            if (username != null)
            {
                #region User data

                User user = _context.Users.Where(x => x.UserName == username).FirstOrDefault();
                User currentUser = _context.Users.Where(x => x.UserName.Equals(User.Identity.Name)).SingleOrDefault();
                Image image = _context.Images.Where(x => x.UserId == user.Id).SingleOrDefault();
                bool isFriend = false;
                bool waitForAccept = false;

                if (_context.Friends.Contains(_context.Friends.Where(x => ((x.FirstUserId == user.UserName && x.SecondUserId == currentUser.UserName) ||
                   (x.FirstUserId == currentUser.UserName && x.SecondUserId == user.UserName)) && x.IsConfirmed == true).FirstOrDefault()))
                    isFriend = true;

                else if (_context.Friends.Contains(_context.Friends.Where(x => ((x.FirstUserId == user.UserName && x.SecondUserId == currentUser.UserName) ||
                        (x.FirstUserId == currentUser.UserName && x.SecondUserId == user.UserName)) && x.IsConfirmed == false).FirstOrDefault()))
                    waitForAccept = true;

                UserViewModel userVM = new UserViewModel
                {
                    Email = user.Email,
                    About = user.About,
                    Username = username,
                    IsFriend = isFriend,
                    Source = ReadImage(image)
                };

                ViewBag.UserVM = userVM;
                ViewBag.Birthday = DateTime.Now.Date.ToString();
                ViewBag.WaitForAccept = waitForAccept;

                #endregion

                #region Common groups

                List<GroupViewModel> groupVMs = new List<GroupViewModel>();
                User currUser = _context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                List<UserGroup> listUGs = _context.UserGroups.Where(x => x.User == user || x.User == currUser).ToList();

                foreach (var item in listUGs)
                {
                    Group group = _context.Groups.Where(x => x.Id.Equals(item.GroupId)).FirstOrDefault();
                    List<UserGroup> listUG = _context.UserGroups.Where(x => x.Group == group).ToList();

                    if (listUG.Contains(listUG.Where(x => x.User == user).FirstOrDefault()) &&
                        listUG.Contains(listUG.Where(x => x.User == currUser).FirstOrDefault()))
                    {
                        Image img = _context.Images.Where(x => x.GroupId == group.Id).SingleOrDefault();

                        GroupViewModel groupVM = new GroupViewModel()
                        {
                            Id = group.Id,
                            Name = group.Name,
                            LastMessage = group.LastMessage,
                            Src = ReadImage(img)
                        };

                        if (!groupVMs.Contains(groupVMs.Where(x => x.Id == groupVM.Id).FirstOrDefault()))
                            groupVMs.Add(groupVM);
                    }
                }

                ViewBag.Groups = groupVMs;

                #endregion
            }
            else
                ViewBag.Null = true;

            ViewBag.Friends = GetFriends();

            return View(ViewBag);
        }

        public IActionResult UserProfile()
        {
            User user = _context.Users.Where(x => x.UserName == User.Identity.Name).SingleOrDefault();
            Image userProfileImage = _context.Images.Where(x => x.UserId == user.Id).SingleOrDefault();
            List<UserViewModel> users = new List<UserViewModel>();
            ViewBag.Friends = GetFriends();


            UserViewModel userVM = new UserViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                Source = ReadImage(userProfileImage),
                FirstLogin = user.FirstLogin
            };

            return View(userVM);
        }

        [HttpPost]
        public IActionResult UserProfile(UserViewModel userVM)
        {
            User user = _context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

            if (userVM.Image != null)
                SaveImage(userVM.Image, null, user);

            if (userVM.Email != null)
            {
                user.Email = userVM.Email;
                user.NormalizedEmail = userVM.Email.ToUpper();
            }

            if (userVM.About != null)
                user.About = userVM.About;

            _context.SaveChanges();

            return RedirectToAction();
        }

        [HttpGet]
        public IActionResult Settings() => View();

        [HttpPost]
        public IActionResult Settings(bool darkTheme = false)
        {
            User user = _context.Users.Where(x => x.UserName == User.Identity.Name).SingleOrDefault();
            Settings settings = _context.Settings.Where(x => x.UserId == user.Id).SingleOrDefault();

            if (settings != null)
                settings.DarkMode = darkTheme;
            else
            {
                Settings newSettings = new Settings
                {
                    DarkMode = darkTheme,
                    UserId = user.Id
                };

                _context.Settings.Add(newSettings);
            }

            _context.SaveChanges();

            return View();
        }

        public IActionResult Privacy => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        #endregion

        #region Partial Views Methods

        public IActionResult Modal() => PartialView();

        public IActionResult UsersList() => PartialView();

        public IActionResult GroupDetails(string id)
        {
            if (id == null) return PartialView(null);

            User currentUser = _context.Users.Where(x => x.UserName.Equals(User.Identity.Name)).SingleOrDefault();
            Group group = _context.Groups.Where(x => x.Id.Equals(id)).SingleOrDefault();
            List<UserGroup> userGroups = _context.UserGroups.Where(x => x.GroupId.Equals(group.Id)).ToList();

            if (group != null)
            {
                GroupViewModel groupVM = new GroupViewModel
                {
                    Users = new List<UserViewModel>(),
                    CreationDate = group.CreationDate,
                    Name = group.Name,
                    About = group.About
                };

                foreach (var item in userGroups)
                {
                    UserViewModel userVM = new UserViewModel();
                    User user = _context.Users.Where(x => x.Id.Equals(item.UserId)).SingleOrDefault();
                    Image image = _context.Images.Where(x => x.UserId == user.Id).SingleOrDefault();
                    userVM.Username = user.UserName;

                    if (image != null)
                        userVM.Source = ReadImage(image);

                    groupVM.Users.Add(userVM);
                }

                if (group.OwnerId == currentUser.Id)
                    groupVM.AmIKing = true;

                return PartialView(groupVM);
            }
            else
                return PartialView(null);
        }

        #endregion

        #region Methods for Async Calling

        #region Group CRUD

        public JsonResult AddGroup(string usersList, IFormFile file, string name, string description)
        {
            if (name == null) return Json(false);

            List<string> list = new List<string>
            {
                User.Identity.Name
            };

            if (usersList != null)
            {
                string[] uList = usersList.Split(",");

                foreach (var item in uList)
                    list.Add(item);
            }

            Group group = new Group
            {
                Name = name,
                About = description,
                Owner = _context.Users.Where(x => x.UserName == User.Identity.Name).SingleOrDefault(),
                UserGroups = new List<UserGroup>(),
                CreationDate = DateTime.Now
            };

            if (file != null)
                group.Image = SaveImage(file, group);

            Guid guid = Guid.NewGuid();
            string uniqueId = Convert.ToBase64String(guid.ToByteArray());
            group.Id = uniqueId;
            group.LastOnline = DateTime.Now;

            _context.Groups.Add(group);

            foreach (var username in list)
            {
                if (username == null)
                    continue;

                User user = _context.Users.Where(x => x.UserName == username).SingleOrDefault();

                UserGroup groupUser = new UserGroup()
                {
                    Group = group,
                    User = user,
                    UserId = user.Id,
                    GroupId = group.Id
                };

                _context.UserGroups.Add(groupUser);
            }

            _context.SaveChanges();

            GroupViewModel groupVM = new GroupViewModel
            {
                Id = group.Id,
                Name = group.Name,
                Src = ReadImage(group.Image)
            };

            List<Connection> conns = new List<Connection>();
            List<string> connIds = new List<string>();

            foreach (var item in list)
                conns.AddRange(_context.Connections.Where(x => x.Username == item).ToList());

            foreach (var item in conns)
                connIds.Add(item.ConnectionId);

            _hubContext.Clients.Clients(connIds).SendAsync("ReceiveNewGroup", name, ReadImage(group.Image), group.Id);

            return Json(groupVM);
        }

        public JsonResult GetGroup(string id)
        {
            List<MessageViewModel> list = new List<MessageViewModel>();
            List<Message> messages = new List<Message>();
            Group group = _context.Groups.Where(x => x.Id.Equals(id)).SingleOrDefault();
            User user = _context.Users.Where(x => x.UserName == User.Identity.Name).SingleOrDefault();
            messages = _context.Messages.Where(x => x.Group == group).ToList();

            if (messages != null)
            {
                foreach (var message in messages)
                {
                    Image profileImage = _context.Images.Where(x => x.UserId.Equals(message.UserId)).FirstOrDefault();
                    Image image = _context.Images.Where(x => x.Id == message.Image).FirstOrDefault();

                    MessageViewModel messageVM = new MessageViewModel
                    {
                        Id = message.Id,
                        Date = message.Date.ToString("G"),
                        UserId = message.UserId,
                        Text = message.Text,
                        Username = _context.Users.Find(message.UserId).UserName,
                        Image = ReadImage(image),
                        ProfileImage = ReadImage(profileImage)
                    };

                    list.Add(messageVM);
                }
            }
            return Json(list);
        }

        public JsonResult EditGroup(string groupId, string[] list, IFormFile file, string name, string desc)
        {
            Group group = _context.Groups.Find(groupId);
            User currentUser = _context.Users.Where(x => x.UserName == User.Identity.Name).SingleOrDefault();
            GroupViewModel groupVM = new GroupViewModel();
            list = list[0].Split(',');

            if (currentUser.Id == group.OwnerId)
            {
                if (name != null)
                {
                    group.Name = name;
                    groupVM.Name = group.Name;
                }

                if (file != null)
                {
                    group.Image = SaveImage(file, group);
                    groupVM.Src = ReadImage(group.Image);
                }

                if (desc != null)
                {
                    group.About = desc;
                    groupVM.About = group.About;
                }

                if (list != null)
                {
                    List<User> newUsers = new List<User>();
                    List<User> oldUsers = new List<User>();
                    List<UserGroup> ListUG = new List<UserGroup>();
                    List<UserViewModel> userVM = new List<UserViewModel>();

                    ListUG.AddRange(_context.UserGroups.Where(x => x.GroupId == group.Id));

                    foreach (string item in list)
                        newUsers.Add(_context.Users.Where(x => x.UserName == item).SingleOrDefault());

                    foreach (UserGroup item in ListUG)
                        oldUsers.Add(_context.Users.Where(x => x.Id == item.UserId).SingleOrDefault());

                    foreach (User user in newUsers)
                    {
                        if (!oldUsers.Contains(user))
                        {
                            UserGroup newUG = new UserGroup
                            {
                                GroupId = group.Id,
                                UserId = user.Id
                            };

                            _context.UserGroups.Add(newUG);
                        }
                    }

                    foreach (User user in oldUsers)
                    {
                        if (!newUsers.Contains(user))
                        {
                            UserGroup ug = new UserGroup();
                            ug = _context.UserGroups.Where(x => x.GroupId == group.Id && x.UserId == user.Id).SingleOrDefault();

                            if (ug != null)
                                _context.UserGroups.Remove(ug);
                        }
                        else
                            userVM.Add(new UserViewModel { Username = user.UserName, Source = ReadImage(user.Image) });
                    }

                    groupVM.Users = userVM;
                }

                _context.SaveChanges();
            }

            return Json(groupVM);
        }

        [HttpPost]
        public JsonResult DeleteGroup(string id)
        {
            Group group = _context.Groups.Where(x => x.Id.Equals(id)).SingleOrDefault();
            User currentUser = _context.Users.Where(x => x.UserName == User.Identity.Name).SingleOrDefault();

            if (currentUser.Id == group.OwnerId)
            {
                List<Message> messages = new List<Message>();

                if (group != null)
                    messages = _context.Messages.Where(x => x.GroupId == group.Id).ToList();

                _context.Messages.RemoveRange(messages);
                _context.Groups.Remove(group);
                _context.Images.RemoveRange(_context.Images.Where(x => x.GroupId.Equals(id)).ToList());
                _context.SaveChanges();
            }

            return Json(true);
        }

        #endregion

        public JsonResult AddRemoveFriend(string username)
        {
            string currentUser = User.Identity.Name;
            var friends = _context.Friends.Where(x => (x.FirstUserId == username && x.SecondUserId == currentUser)
                          || (x.FirstUserId == currentUser && x.SecondUserId == username)).ToList();

            if (friends.Count == 0)
            {
                Friends friend = new Friends
                {
                    FirstUserId = currentUser,
                    SecondUserId = username,
                    IsConfirmed = false
                };

                _context.Friends.Add(friend);
            }
            else
            {
                foreach (var item in friends)
                {
                    if (item.IsConfirmed == true)
                        _context.Friends.Remove(item);

                    else if (item.IsConfirmed == false && item.SecondUserId == currentUser)
                        item.IsConfirmed = true;
                }
            }

            _context.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public async Task<JsonResult> SendMessage(IFormFile file, string text, string talk)
        {
            Group group = _context.Groups.Where(x => x.Name == talk).FirstOrDefault();
            User sender = _context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            List<UserGroup> listUG = _context.UserGroups.Where(x => x.Group == group).ToList();
            List<Connection> conns = new List<Connection>();
            List<string> connIds = new List<string>();
            Image image = new Image();

            if (group != null)
            {
                if (file != null)
                    image = SaveImage(file, null);

                group.LastMessage = text;
                group.LastOnline = DateTime.Now;

                foreach (var item in listUG)
                {
                    User user = _context.Users.Find(item.UserId);

                    if (user.UserName != User.Identity.Name)
                    {
                        var connectionsRange = _context.Connections.Where(x => x.Username == user.UserName).ToList();
                        conns.AddRange(connectionsRange);
                    }
                }

                foreach (Connection id in conns)
                    connIds.Add(id.ConnectionId);

                Message message = new Message()
                {
                    Group = group,
                    Date = DateTime.Now,
                    Text = text,
                    User = sender,
                    Image = image.Id
                };

                _context.Messages.Add(message);
                _context.SaveChanges();

                await _hubContext.Clients.Clients(connIds).SendAsync("ReceiveMessage", message.Id.ToString(), image.Id, group.Id.ToString());
            }

            return Json(true);
        }

        public JsonResult GetMessage(string messageId, string imageId)
        {
            Message message = _context.Messages.Where(x => x.Id == int.Parse(messageId)).SingleOrDefault();
            User user = _context.Users.Where(x => x.Id == message.UserId).SingleOrDefault();
            Image profileImage = _context.Images.Where(x => x.User == user).SingleOrDefault();

            MessageViewModel messageVM = new MessageViewModel
            {
                Text = message.Text,
                Username = user.UserName,
                Date = message.Date.ToString("G"),
                ProfileImage = ReadImage(profileImage)
            };

            if (imageId != null)
            {
                Image image = _context.Images.Where(x => x.Id == imageId).FirstOrDefault();
                messageVM.Image = ReadImage(image);
            }

            return Json(messageVM);
        }

        public JsonResult GetUserSettings()
        {
            //Settings settings = new Settings();
            User user = _context.Users.Where(x => x.UserName == User.Identity.Name).SingleOrDefault();
            Settings settings = _context.Settings.Where(x => x.UserId == user.Id).SingleOrDefault();

            if (settings == null)
                settings = new Settings(false);

            return Json(settings.DarkMode);
        }

        public JsonResult GetUser(string username)
        {
            User user = _context.Users.Where(x => x.UserName == username).SingleOrDefault();
            Image image = _context.Images.Where(x => x.UserId == user.Id).SingleOrDefault();
            string src = null;

            if (image != null)
                src = ReadImage(image);

            UserViewModel userVM = new UserViewModel
            {
                Username = user.UserName,
                Source = src
            };

            return Json(userVM);
        }

        public JsonResult GetUsers(string partOfName, int page)
        {
            List<UserViewModel> userVMs = new List<UserViewModel>();
            List<User> users = (from user
                                in _context.Users
                                where user.UserName.Contains(partOfName)
                                select user).Skip(10 * page).Take(10).ToList();

            users.Remove(users.Where(x => x.UserName == User.Identity.Name).SingleOrDefault());

            userVMs.AddRange(from item in users
                             let image = _context.Images.Where(x => x.UserId == item.Id).SingleOrDefault()
                             select new UserViewModel
                             {
                                 Username = item.UserName,
                                 Source = ReadImage(image),
                                 About = $"/Home/Profile?username={item.UserName}"
                             });

            return Json(userVMs);
        }

        public JsonResult GetNotifications()
        {
            User currentUser = _context.Users.Where(x => x.UserName == User.Identity.Name).SingleOrDefault();
            List<Notification> notifications = _context.Notifications.Where(x => x.UserId.Equals(currentUser.Id)).ToList();
            List<NotificationVM> notificationVMs = new List<NotificationVM>();

            foreach (var item in notifications)
            {
                User user = _context.Users.Where(x => x.Id.Equals(item.ToUserId)).SingleOrDefault();

                notificationVMs.Add(new NotificationVM
                {
                    Content = item.Content,
                    Username = user.UserName,
                    Date = item.Date.ToString()
                });
            }

            return Json(notificationVMs);
        }

        public JsonResult AddNotification(string username)
        {
            User user = _context.Users.Where(x => x.UserName == username).SingleOrDefault();
            User currentUser = _context.Users.Where(x => x.UserName == User.Identity.Name).SingleOrDefault();

            if (user != null)
            {
                Notification notification = new Notification
                {
                    Date = DateTime.Now,
                    UserId = currentUser.Id,
                    ToUserId = user.Id,
                    Content = $"/Home/Profile?username={username}"
                };

                _context.Notifications.Add(notification);
                _context.SaveChanges();

                List<Connection> conns = _context.Connections.Where(x => x.Username.Equals(user.UserName)).ToList();
                List<string> connids = new List<string>();

                foreach (var item in conns)
                {
                    connids.Add(item.ConnectionId);
                }

                string content = notification.Content;
                string date = notification.Date.ToString();

                _hubContext.Clients.Clients(connids).SendAsync("ReceiveNotification", content, date, username);
            }

            return Json(true);
        }

        #endregion

        #region Helper Methods 

        public List<UserViewModel> GetFriends()
        {
            bool isOnline = true;
            string currentUser = User.Identity.Name;
            User user = new User();
            Image image = new Image();
            List<UserViewModel> userVMs = new List<UserViewModel>();
            List<Friends> friends = _context.Friends.Where(x => (x.FirstUserId == User.Identity.Name || x.SecondUserId == User.Identity.Name) && x.IsConfirmed == true).ToList();

            foreach (var item in friends)
            {
                UserViewModel UserVM = new UserViewModel();

                if (item.FirstUserId == User.Identity.Name)
                {
                    isOnline = _context.Connections.Contains(_context.Connections.Where(x => x.Username == item.SecondUserId).FirstOrDefault());
                    user = _context.Users.Where(x => x.UserName == item.SecondUserId).FirstOrDefault();
                    UserVM.Username = item.SecondUserId;
                    image = _context.Images.Where(x => x.UserId == user.Id).SingleOrDefault();
                }
                else
                {
                    isOnline = _context.Connections.Contains(_context.Connections.Where(x => x.Username == item.FirstUserId).FirstOrDefault());
                    user = _context.Users.Where(x => x.UserName == item.FirstUserId).FirstOrDefault();
                    UserVM.Username = item.FirstUserId;
                    image = _context.Images.Where(x => x.UserId == user.Id).SingleOrDefault();
                }

                UserVM.IsFriend = true;
                UserVM.IsOnline = isOnline;
                UserVM.LastOnline = user.LastOnline;
                UserVM.Source = ReadImage(image);

                userVMs.Add(UserVM);
            }

            return userVMs;
        }

        public Image SaveImage(IFormFile file, Group group = null, User user = null)
        {
            Image image = new Image();
            string uniqueName = Guid.NewGuid().ToString() + "_" + file.FileName;
            image.Id = uniqueName;
            image.ImageName = file.FileName;

            if (file.Length > 0)
            {
                byte[] byteString = null;

                using (var fs = file.OpenReadStream())
                {
                    using var ms = new MemoryStream();
                    fs.CopyTo(ms);
                    byteString = ms.ToArray();
                }

                image.ByteString = byteString;
            }

            _context.SaveChanges();

            if (user != null)
            {
                Image imgToDelete = _context.Images.Where(x => x.UserId == user.Id).SingleOrDefault();
                if (imgToDelete != null) _context.Images.Remove(imgToDelete);
                image.UserId = user.Id;
            }

            else if (group != null)
            {
                List<Image> imgToDelete = _context.Images.Where(x => x.GroupId == group.Id).ToList();
                if (imgToDelete != null)
                    _context.Images.RemoveRange(imgToDelete);

                image.GroupId = group.Id;
            }

            _context.Images.Add(image);
            _context.SaveChanges();

            return image;
        }

        public string ReadImage(Image image)
        {
            if (image != null)
            {
                string base64 = Convert.ToBase64String(image.ByteString);
                string src = string.Format($"data:image/gif;base64,{base64}");

                return src;
            }
            else
                return null;
        }

        #endregion
    }
}