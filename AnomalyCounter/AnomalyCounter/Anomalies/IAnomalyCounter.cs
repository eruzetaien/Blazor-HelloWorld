namespace AnomalyCounter.Anomalies;
public interface IAnomalyCounter
{
    string Name { get; set; }
    int CurrentValue { get; set; }
    bool Enabled { get; set; }
    TimeSpan RefreshInterval { get; set; }

    event Action? Updated;

    Task StartAsync();
    Task StopAsync();
    Task RefreshAsync();
    void UpdateSettings(TimeSpan interval, bool enabled);
}

