/*
 * ============================================
 * Vanadium Nuker V12 - Discord Server Nuker
 * ============================================
 * 
 * Copyright (c) 2026 RussianHarvey & Tobakk
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
 * 
 * ============================================
 * Discord: @russianharvey | @_ux8
 * GitHub: https://github.com/Uxz7
 * Version: 2.0.0-ULTRA
 * ============================================
 */
namespace VanadiumStrike.Config;

public static class ApiEndpoints
{
    private const string BaseUrl = "https://discord.com/api/v10";
    
    public static class Guilds
    {
        public static string GetGuild(ulong guildId) => $"{BaseUrl}/guilds/{guildId}";
        public static string GetGuildChannels(ulong guildId) => $"{BaseUrl}/guilds/{guildId}/channels";
        public static string GetGuildMembers(ulong guildId) => $"{BaseUrl}/guilds/{guildId}/members";
        public static string GetGuildBans(ulong guildId) => $"{BaseUrl}/guilds/{guildId}/bans";
        public static string BanMember(ulong guildId, ulong userId) => $"{BaseUrl}/guilds/{guildId}/bans/{userId}";
        public static string KickMember(ulong guildId, ulong userId) => $"{BaseUrl}/guilds/{guildId}/members/{userId}";
        public static string GetGuildRoles(ulong guildId) => $"{BaseUrl}/guilds/{guildId}/roles";
        public static string GetGuildEmojis(ulong guildId) => $"{BaseUrl}/guilds/{guildId}/emojis";
        public static string GetGuildStickers(ulong guildId) => $"{BaseUrl}/guilds/{guildId}/stickers";
        public static string GetGuildInvites(ulong guildId) => $"{BaseUrl}/guilds/{guildId}/invites";
        public static string GetVanityUrl(ulong guildId) => $"{BaseUrl}/guilds/{guildId}/vanity-url";
        public static string CreateRole(ulong guildId) => $"{BaseUrl}/guilds/{guildId}/roles";
    }
    
    public static class Channels
    {
        public static string GetChannel(ulong channelId) => $"{BaseUrl}/channels/{channelId}";
        public static string SendMessage(ulong channelId) => $"{BaseUrl}/channels/{channelId}/messages";
        public static string CreateWebhook(ulong channelId) => $"{BaseUrl}/channels/{channelId}/webhooks";
        public static string DeleteChannel(ulong channelId) => $"{BaseUrl}/channels/{channelId}";
        public static string UpdateChannel(ulong channelId) => $"{BaseUrl}/channels/{channelId}";
    }
    
    public static class Users
    {
        public static string CreateDm() => $"{BaseUrl}/users/@me/channels";
    }
}
