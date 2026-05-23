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
using VanadiumStrike.Core;
using VanadiumStrike.Config;
using System.Reflection;

namespace VanadiumStrike;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "Vanadium Nuker V12";
        
        ForceClear();
        
        ConsoleHelper.PrintBanner();
        
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}Bot Token: {ColorConfig.Reset}");
        string token = Console.ReadLine()?.Trim() ?? "";
        
        Console.Write($"{ColorConfig.Red}# {ColorConfig.White}Guild ID: {ColorConfig.Reset}");
        string guildIdStr = Console.ReadLine()?.Trim() ?? "";
        
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(guildIdStr) || !ulong.TryParse(guildIdStr, out ulong guildId))
        {
            ConsoleHelper.PrintError("Invalid input!");
            return;
        }
        
        ForceClear();
        
        var bot = new VanadiumBot(token, guildId);
        await bot.StartAsync();
    }
    public static void ShowAssemblyInfo()
{
    var assembly = Assembly.GetExecutingAssembly();
    var version = assembly.GetName().Version;
    var title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "Vanadium Strike";
    var company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "RussianHarvey & Tobakk";
    var copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "Copyright © 2026";
    
    ConsoleHelper.PrintInfo($"{title} v{version}");
    ConsoleHelper.PrintInfo($"{company}");
    ConsoleHelper.PrintInfo($"{copyright}");
}
    
    public static void ForceClear()
    {
        Console.Write("\u001b[2J\u001b[3J\u001b[H");
        Console.Out.Flush();
    }
}
