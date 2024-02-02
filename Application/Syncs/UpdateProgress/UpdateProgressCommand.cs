namespace Sync.Application.Syncs.UpdateProgress;

public record UpdateProgressCommand(
    decimal Percentage,
    string Progress,
    string Device,
    string DeviceId,
    string Document
);