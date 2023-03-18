namespace RSFBackup.Core.DTO.Files;

public class DeleteFileResponse
{
    public int DeletedObjectsCount { get; private set; }

    public DeleteFileResponse(int deletedObjectsCount)
    {
        DeletedObjectsCount = deletedObjectsCount;
    }
}
