namespace Sync.Application.Syncs.GetProgress;

public record GetProgressResponse(
    decimal Percentage,
    int Progress,
    string Device,
    string DeviceId,
    long Timestamp,
    string Document
);