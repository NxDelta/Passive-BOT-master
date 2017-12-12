﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PassiveBOT.Configuration;

namespace PassiveBOT.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CheckDj : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command,
            IServiceProvider services)
        {
            var id = context.Guild.Id;
            var role = GuildConfig.Load(id).DjRoleId;
            if (role == 0)
                return Task.FromResult(PreconditionResult.FromSuccess());

            if (((IGuildUser) context.User).RoleIds.Contains(role))
                return Task.FromResult(PreconditionResult.FromSuccess());

            return Task.FromResult(PreconditionResult.FromError("User is Not DJ"));
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CheckModerator : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command,
            IServiceProvider services)
        {
            var id = context.Guild.Id;
            var role = GuildConfig.Load(id).ModeratorRoleId;
            if (role == 0)
            {
                if (((IGuildUser) context.User).GuildPermissions.Administrator)
                    return Task.FromResult(PreconditionResult.FromSuccess());
            }
            else
            {
                if (((IGuildUser) context.User).RoleIds.Contains(role))
                    return Task.FromResult(PreconditionResult.FromSuccess());
            }


            return Task.FromResult(PreconditionResult.FromError("User is Not A Moderator or an Admin!"));
        }
    }
}