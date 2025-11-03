namespace AnomalyCounter.Anomalies;

public class AnomalyCounter2 : SqlAnomalyCounterBase
{
    public AnomalyCounter2(IServiceScopeFactory scopeFactory)
        : base(scopeFactory) { }

    public override string Name { get; set; } = "Anomaly 2";

    public override string StoredProcedure => "sp_GetAnomaly2";
}