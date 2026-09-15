namespace Vyzor.Application.DTO.Chat;

public class SupportChatResultDTO
{
    public ChatMessageDTO Message { get; set; } = null!;

    public ChatMessageDTO? SupportMessage { get; set; }

    public string? MatchedSpecialization { get; set; }

    public List<DoctorRecommendationDTO> Doctors { get; set; } = new();
}