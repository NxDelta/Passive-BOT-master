﻿using System;
using System.Collections.Generic;
using System.IO;
using Discord;
using Newtonsoft.Json;
using PassiveBOT.Commands;

namespace PassiveBOT.Configuration
{
    public class GuildConfig
    {
        [JsonIgnore] public static readonly string Appdir = AppContext.BaseDirectory;


        public ulong GuildId { get; set; } //
        public string GuildName { get; set; } //
        public string Prefix { get; set; } = Configuration.Load.Pre; //

        public ulong DjRoleId { get; set; } // restrict the music module to a specific role
        public ulong MutedRole { get; set; } = 0;
        public ulong ModeratorRoleId { get; set; } = 0;

        //TRANSLATION
        public bool Premium { get; set; } = false;

        public DateTime TimeOffset { get; set; }
        public int Characters { get; set; } = 0;


        public List<ulong> RoleList { get; set; } =
            new List<ulong>(); // a list of roles that users can join via command

        //public string Roles { get; set; } //Unnecessary data as this was converted to the above list.

        public string Rss { get; set; } = "0"; // rss feed url
        public ulong RssChannel { get; set; } // channel to post custom rss feeds to
        public List<Twitch> TwitchChannels { get; set; } = new List<Twitch>();
        public ulong TwitchPostChannel { get; set; } = 0;


        public List<Tags.Tagging> Dict { get; set; } = new List<Tags.Tagging>(); // tags module

        public List<string> Blacklist { get; set; } = new List<string>(); // keyword blacklist
        public string BlacklistMessage { get; set; } = "";
        public bool Invite { get; set; } = false; // blacklist for discord invites
        public List<ulong> InviteExcempt { get; set; } = new List<ulong>();
        public bool MentionAll { get; set; } = false; //blacklist for @everyone and @here 
        public List<ulong> MentionallExcempt { get; set; } = new List<ulong>();

        public bool ErrorLog { get; set; } // allows for responses with errors 

        public bool GoodbyeEvent { get; set; } = false;
        public string GoodbyeMessage { get; set; } = "Has Left the Server :(";
        public ulong GoodByeChannel { get; set; } = 0;
        public bool WelcomeEvent { get; set; } // toggles welcome messages for new users
        public string WelcomeMessage { get; set; } = "Welcome to Our Server!!!"; // the welcome message
        public ulong WelcomeChannel { get; set; } // welcome messages in a channel
        public bool EventLogging { get; set; } = false;
        public ulong EventChannel { get; set; } = 0;

        public GiveAway Comp { get; set; } = new GiveAway();


        public ulong Starboard { get; set; } = 0;

        public List<Warns> Warnings { get; set; } = new List<Warns>();
        public List<Kicks> Kicking { get; set; } = new List<Kicks>();
        public List<Bans> Banning { get; set; } = new List<Bans>();


        public static void SaveServer(GuildConfig config, IGuild guild)
        {
            var file = Path.Combine(Appdir, $"setup/server/{guild.Id}.json");
            var output = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(file, output);
        }

        public static GuildConfig Load(ulong id)
        {
            var file = Path.Combine(Appdir, $"setup/server/{id}.json");
            return JsonConvert.DeserializeObject<GuildConfig>(File.ReadAllText(file));
        }

        public static void Setup(IGuild guild)
        {
            if (File.Exists(Path.Combine(Appdir, $"setup/server/{guild.Id}.json"))) return;
            var cfg = new GuildConfig
            {
                GuildId = guild.Id,
                GuildName = guild.Name
            };

            SaveServer(cfg, guild);
        }

        public static GuildConfig GetServer(IGuild guild)
        {
            if (!File.Exists(Path.Combine(Appdir, $"setup/server/{guild.Id}.json")))
            {
                var cfg = new GuildConfig
                {
                    GuildId = guild.Id,
                    GuildName = guild.Name
                };

                SaveServer(cfg, guild);
            }

            var file = Path.Combine(Appdir, $"setup/server/{guild.Id}.json");
            return JsonConvert.DeserializeObject<GuildConfig>(File.ReadAllText(file));
        }

        public static string SetWMessage(IGuild guild, string input)
        {
            var file = Path.Combine(Appdir, $"setup/server/{guild.Id}.json");
            if (File.Exists(file))
            {
                var jsonObj = GetServer(guild);
                jsonObj.WelcomeMessage = input;
                jsonObj.WelcomeEvent = true;
                SaveServer(jsonObj, guild);
            }
            else
            {
                return "please run the setup command before using configuration commands";
            }
            return null;
        }

        public static string SetWChannel(IGuild guild, ulong channel)
        {
            var file = Path.Combine(Appdir, $"setup/server/{guild.Id}.json");
            if (File.Exists(file))
            {
                var jsonObj = GetServer(guild);
                jsonObj.WelcomeChannel = channel;
                SaveServer(jsonObj, guild);
            }
            else
            {
                return "please run the setup command before using configuration commands";
            }
            return null;
        }

        public static string SetError(IGuild guild, bool status)
        {
            var file = Path.Combine(Appdir, $"setup/server/{guild.Id}.json");
            if (File.Exists(file))
            {
                var jsonObj = GetServer(guild);
                jsonObj.ErrorLog = status;
                SaveServer(jsonObj, guild);
            }
            else
            {
                return "please run the setup command before using configuration commands";
            }
            return null;
        }


        public static string SetWelcomeStatus(IGuild guild, bool status)
        {
            var file = Path.Combine(Appdir, $"setup/server/{guild.Id}.json");
            if (File.Exists(file))
            {
                var jsonObj = GetServer(guild);
                jsonObj.WelcomeEvent = status;
                SaveServer(jsonObj, guild);
            }
            else
            {
                return "please run the setup command before using configuration commands";
            }
            return null;
        }

        public static string SetDj(IGuild guild, ulong role)
        {
            var file = Path.Combine(Appdir, $"setup/server/{guild.Id}.json");
            if (File.Exists(file))
            {
                var jsonObj = GetServer(guild);
                jsonObj.DjRoleId = role;
                SaveServer(jsonObj, guild);
            }
            else
            {
                return "please run the setup command before using configuration commands";
            }
            return null;
        }

        public static string RssSet(IGuild guild, ulong chan, string url, bool add)
        {
            var file = Path.Combine(Appdir, $"setup/server/{guild.Id}.json");
            if (File.Exists(file))
            {
                var jsonObj = GetServer(guild);

                if (add)
                {
                    jsonObj.Rss = url;
                    jsonObj.RssChannel = chan;
                }
                else
                {
                    jsonObj.Rss = "0";
                }

                SaveServer(jsonObj, guild);
            }
            else
            {
                return "please run the setup command before using configuration commands";
            }
            return null;
        }

        public class Twitch
        {
            public string Username { get; set; }
            public bool LastCheckedStatus { get; set; } = false;
        }

        public class GiveAway
        {
            public string Message { get; set; }
            public List<ulong> Users { get; set; } = new List<ulong>();
            public ulong Creator { get; set; }
        }

        public class Warns
        {
            public string User { get; set; }
            public string Reason { get; set; }
            public string Moderator { get; set; }
            public ulong UserId { get; set; }
        }

        public class Kicks
        {
            public string User { get; set; }
            public string Reason { get; set; }
            public string Moderator { get; set; }
            public ulong UserId { get; set; }
        }

        public class Bans
        {
            public string User { get; set; }
            public string Reason { get; set; }
            public string Moderator { get; set; }
            public ulong UserId { get; set; }
        }
    }
}