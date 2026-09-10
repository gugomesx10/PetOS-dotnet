using System.Diagnostics;

namespace PetOS.Observability;

public static class PetOsTelemetry
{
    public const string SourceName = "PetOS";

    public static readonly ActivitySource ActivitySource =
        new(SourceName);
}