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
namespace VanadiumStrike.Core;

public class ThreadPoolManager : IDisposable
{
    private readonly SemaphoreSlim _semaphore;
    public ThreadPoolManager(int maxConcurrency = 500) => _semaphore = new SemaphoreSlim(maxConcurrency);
    
    public async Task ExecuteBatchAsync(IEnumerable<Task> tasks)
    {
        var taskList = tasks.ToList();
        var options = new ParallelOptions { MaxDegreeOfParallelism = 500 };
        await Parallel.ForEachAsync(taskList, options, async (task, _) =>
        {
            await _semaphore.WaitAsync();
            try { await task; }
            finally { _semaphore.Release(); }
        });
    }
    
    public void Dispose() => _semaphore.Dispose();
}