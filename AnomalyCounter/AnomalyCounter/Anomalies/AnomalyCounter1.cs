namespace AnomalyCounter.Anomalies;

public class AnomalyCounter1 : SqlAnomalyCounterBase
{
    public AnomalyCounter1(IServiceScopeFactory scopeFactory)
        : base(scopeFactory) { }

    public override string Name { get; set; } = "Anomaly 1";
    public override string StoredProcedure => "sp_GetAnomaly1";
}