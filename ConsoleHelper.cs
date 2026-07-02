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
using System.Runtime.InteropServices;
using VanadiumStrike.Config;

namespace VanadiumStrike;

public static class ConsoleHelper
{
    private static readonly object _lock = new();
    public static bool SilentMode { get; set; }

    public static void Clear()
    {
        try
        {
            Console.Clear();
        }
        catch
        {
            // ignore
        }
    }

    public static void PrintColored(string message, string color)
    {
        if (SilentMode) return;
        lock (_lock)
        {
            Console.Write(color);
            Console.WriteLine(message);
            Console.Write(ColorConfig.Reset);
        }
    }

    private static string FormatLog(string level, string icon, string message)
    {
        var time = DateTime.Now.ToString("ddd dd/MM/yyyy HH:mm:ss");
        return $"[{level}] [{time}] {icon} {message}";
    }

    public static void PrintSuccess(string message) => PrintColored(FormatLog("INFO", "[+]", message), ColorConfig.Green);
    public static void PrintError(string message) => PrintColored(FormatLog("ERROR", "[-]", message), ColorConfig.Red);
    public static void PrintWarning(string message) => PrintColored(FormatLog("WARN", "[!]", message), ColorConfig.Yellow);
    public static void PrintInfo(string message) => PrintColored(FormatLog("INFO", "[*]", message), ColorConfig.Cyan);
    public static void PrintDebug(string message) => PrintColored($"[#] {message}", ColorConfig.Magenta);

    public static void PrintBanner()
    {
        Console.WriteLine($@"{ColorConfig.Red}         __     __                    _ _                 
{ColorConfig.Red}         \ \   / /_ _ _ __   __ _  __| (_)_   _ _ __ ___  
{ColorConfig.Red}          \ \ / / _` | '_ \ / _` |/ _` | | | | | '_ ` _ \ 
{ColorConfig.Red}           \ V / (_| | | | | (_| | (_| | | |_| | | | | | |
{ColorConfig.Red}            \_/ \__,_|_| |_|\__,_|\__,_|_|\__,_|_| |_| |_|
{ColorConfig.Red}        ─────────────────────────────────────────────────────
{ColorConfig.White}         {ColorConfig.Red}Made By {ColorConfig.White}Shisui & Tobakk |  {ColorConfig.Red}Fuck Skidz
{ColorConfig.Red}        ─────────────────────────────────────────────────────{ColorConfig.Reset}");
    }
}
