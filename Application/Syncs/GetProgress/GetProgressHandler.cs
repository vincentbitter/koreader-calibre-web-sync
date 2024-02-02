namespace Sync.Application.Syncs.GetProgress;

class GetProgressHandler
{
    public GetProgressResponse? Handle(string hash)
    {
        // return new GetProgressResponse(0, 0, null, null, null, hash);
        // For now progress is not synced back
        return null;
    }
}