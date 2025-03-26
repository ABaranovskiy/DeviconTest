namespace WebAPI.Dto;

public class UpdateResponseDto
{
    public bool Success { get; init; }
    public DateTime LastUpdate { get; init; }

    public UpdateResponseDto(bool success, DateTime lastUpdate)
    {
        Success = success;
        LastUpdate = lastUpdate;
    }
}
    
