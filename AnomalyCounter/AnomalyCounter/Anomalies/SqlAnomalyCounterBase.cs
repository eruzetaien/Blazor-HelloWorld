using AnomalyCounter.Data;
using Microsoft.EntityFrameworkCore;

namespace AnomalyCounter.Anomalies;

public abstract class SqlAnomalyCounterBase : IAnomalyCounter
{
    private readonly IServiceScopeFactory _scopeFactory;
    private CancellationTokenSource? _cts;

    public abstract string Name { get; set; }
    public abstract string StoredProcedure { get; }
    public bool Enabled { get; set; } = true;
    public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromSeconds(5);
    public int CurrentValue { get; set; }

    public event Action? Updated;

    protected SqlAnomalyCounterBase(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync()
    {
        if (_cts != null) return;
        _cts = new CancellationTokenSource();
        _ = Task.Run(() => RunAsync(_cts.Token));
    }

    private async Task RunAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested && Enabled)
        {
            Console.WriteLine($"[{Name}] Refreshing...");
            await RefreshAsync();
            await Task.Delay(RefreshInterval, token);
        }
    }

    public async Task RefreshAsync()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await using var conn = db.Database.GetDbConnection();
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = StoredProcedure;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            var result = await cmd.ExecuteScalarAsync();
            if (result != null && int.TryParse(result.ToString(), out var value))
            {
                CurrentValue = value;
                Updated?.Invoke();
                Console.WriteLine($"Value : {value}");
            }
        }   
        catch (Exception ex)
        {
            Console.WriteLine($"[{Name}] Error: {ex.Message}");
        }
    }

    public Task StopAsync()
    {
        _cts?.Cancel();
        _cts = null;
        return Task.CompletedTask;
    }

    public void UpdateSettings(TimeSpan interval, bool enabled)
    {
        RefreshInterval = interval;
        Enabled = enabled;
    }
}
