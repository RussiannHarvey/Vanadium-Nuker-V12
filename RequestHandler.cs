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
#nullable disable
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using VanadiumStrike.Config;

namespace VanadiumStrike.Core;

public class RequestHandler : IDisposable
{
    private readonly HttpClient _httpClient;
    
    public RequestHandler(string token)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", token);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "VanadiumStrike/1.0");
        _httpClient.Timeout = TimeSpan.FromSeconds(15);
    }
    
    public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string endpoint, object data = null)
    {
        try
        {
            string url = endpoint.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? endpoint : $"https://discord.com/api/v10/{endpoint}";
            int retryCount = 0;

            while (true)
            {
                using var request = new HttpRequestMessage(method, url);
                if (data != null)
                {
                    var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                    return response;

                var error = await response.Content.ReadAsStringAsync();
                ConsoleHelper.PrintDebug($"HTTP {response.StatusCode}: {error[..Math.Min(100, error.Length)]}");

                if (response.StatusCode == (HttpStatusCode)429 && retryCount == 0)
                {
                    var retryAfter = response.Headers.RetryAfter?.Delta?.TotalSeconds ?? 2;
                    ConsoleHelper.PrintWarning($"Rate limited on {endpoint}. Retrying after {retryAfter:N1}s...");
                    await Task.Delay(TimeSpan.FromSeconds(retryAfter + 1));
                    retryCount++;
                    continue;
                }

                return response;
            }
        }
        catch (TaskCanceledException)
        {
            ConsoleHelper.PrintError($"Timeout on {endpoint}");
            return null;
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError($"Request error: {ex.Message}");
            return null;
        }
    }
    
    public async Task<JsonElement?> GetJsonAsync(string endpoint)
    {
        var response = await SendAsync(HttpMethod.Get, endpoint);
        if (response != null && response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(content);
        }
        return null;
    }
    
    public void Dispose() => _httpClient.Dispose();
}
