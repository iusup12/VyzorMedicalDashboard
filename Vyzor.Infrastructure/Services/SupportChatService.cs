using Microsoft.EntityFrameworkCore;
using Vyzor.Application.DTO;
using Vyzor.Application.DTO.Chat;
using Vyzor.Application.Interfaces;
using Vyzor.Domain.Entities;
using Vyzor.Infrastructure.Data;

namespace Vyzor.Infrastructure.Services;

public class SupportChatService : ISupportChatService
{
    private readonly AppDbContext _context;

    public SupportChatService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int?> GetChatIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken);

        if (patient == null)
            return null;

        return await _context.SupportChats
            .AsNoTracking()
            .Where(x =>
                x.PatientId == patient.Id &&
                !x.IsClosed)
            .OrderByDescending(x => x.LastMessageAtUtc)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int?> GetOrCreateChatIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken);

        if (patient == null)
            return null;

        var chat = await _context.SupportChats
            .Where(x =>
                x.PatientId == patient.Id &&
                !x.IsClosed)
            .OrderByDescending(x => x.LastMessageAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (chat != null)
            return chat.Id;

        chat = new SupportChat
        {
            PatientId = patient.Id,
            CreatedAtUtc = DateTime.UtcNow,
            LastMessageAtUtc = null,
            IsClosed = false
        };

        _context.SupportChats.Add(chat);

        await _context.SaveChangesAsync(cancellationToken);

        return chat.Id;
    }

    public async Task<IReadOnlyList<ChatMessageDTO>> GetMessagesAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var chatId = await GetChatIdAsync(
            userId,
            cancellationToken);

        if (!chatId.HasValue)
            return Array.Empty<ChatMessageDTO>();

        return await _context.SupportChatMessages
            .AsNoTracking()
            .Where(x => x.SupportChatId == chatId.Value)
            .OrderBy(x => x.CreatedAtUtc)
            .Select(x => new ChatMessageDTO
            {
                Id = x.Id,
                SupportChatId = x.SupportChatId,
                SenderUserId = x.SenderUserId,
                Message = x.Message,
                CreatedAtUtc = x.CreatedAtUtc,
                IsFromSupport = x.IsFromSupport
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<SupportChatResultDTO?> SendPatientMessageAsync(
        string userId,
        string message,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message))
            return null;

        message = message.Trim();

        var chatId = await GetOrCreateChatIdAsync(
            userId,
            cancellationToken);

        if (!chatId.HasValue)
            return null;

        var chat = await _context.SupportChats
            .FirstOrDefaultAsync(
                x => x.Id == chatId.Value,
                cancellationToken);

        if (chat == null)
            return null;

        var patientMessage = new SupportChatMessage
        {
            SupportChatId = chat.Id,
            SenderUserId = userId,
            Message = message,
            CreatedAtUtc = DateTime.UtcNow,
            IsFromSupport = false
        };

        _context.SupportChatMessages.Add(patientMessage);

        chat.LastMessageAtUtc = patientMessage.CreatedAtUtc;

        await _context.SaveChangesAsync(cancellationToken);

        var result = new SupportChatResultDTO
        {
            Message = new ChatMessageDTO
            {
                Id = patientMessage.Id,
                SupportChatId = patientMessage.SupportChatId,
                SenderUserId = patientMessage.SenderUserId,
                Message = patientMessage.Message,
                CreatedAtUtc = patientMessage.CreatedAtUtc,
                IsFromSupport = false
            }
        };

        var specializations = await _context.Specializations
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(x => new
            {
                x.Id,
                x.Name
            })
            .ToListAsync(cancellationToken);

        var normalizedMessage = Normalize(message);

        var matchedSpecialization = specializations
            .FirstOrDefault(x =>
                IsSpecializationMatch(
                    normalizedMessage,
                    x.Name));

        string supportText;

        if (matchedSpecialization == null)
        {
            supportText =
                "Здравствуйте! Я могу помочь подобрать врача. " +
                "Напишите, какой специалист вам нужен, например: " +
                "кардиолог, невролог, терапевт или дерматолог.";
        }
        else
        {
            result.MatchedSpecialization =
                matchedSpecialization.Name;

            result.Doctors = await _context.Doctors
                .AsNoTracking()
                .Where(x =>
                    x.SpecializationId == matchedSpecialization.Id)
                .OrderByDescending(x => x.Rating)
                .ThenBy(x => x.FullName)
                .Take(5)
                .Select(x => new DoctorRecommendationDTO
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    SpecializationName =
                        x.Specialization != null
                            ? x.Specialization.Name
                            : matchedSpecialization.Name,
                    ImageUrl = x.ImageUrl,
                    AppointmentPrice = x.AppointmentPrice,
                    Rating = x.Rating
                })
                .ToListAsync(cancellationToken);

            if (result.Doctors.Count > 0)
            {
                supportText =
                    $"Я нашёл врачей по направлению «{matchedSpecialization.Name}». " +
                    "Выберите подходящего специалиста из списка ниже.";
            }
            else
            {
                supportText =
                    $"Я нашёл направление «{matchedSpecialization.Name}», " +
                    "но сейчас подходящих врачей нет.";
            }
        }

        var supportMessage = new SupportChatMessage
        {
            SupportChatId = chat.Id,
            SenderUserId = "support",
            Message = supportText,
            CreatedAtUtc = DateTime.UtcNow,
            IsFromSupport = true
        };

        _context.SupportChatMessages.Add(supportMessage);

        chat.LastMessageAtUtc = supportMessage.CreatedAtUtc;

        await _context.SaveChangesAsync(cancellationToken);

        result.SupportMessage = new ChatMessageDTO
        {
            Id = supportMessage.Id,
            SupportChatId = supportMessage.SupportChatId,
            SenderUserId = supportMessage.SenderUserId,
            Message = supportMessage.Message,
            CreatedAtUtc = supportMessage.CreatedAtUtc,
            IsFromSupport = supportMessage.IsFromSupport
        };

        return result;
    }

    private static bool IsSpecializationMatch(
     string message,
     string specialization)
    {
        var text = Normalize(message);
        var name = Normalize(specialization);

        if (text.Contains(name))
            return true;

        var keywords = new Dictionary<string, string[]>
        {
            ["therapist"] = new[]
            {
            "терапевт",
            "терапия",
            "терапевту",
            "терапевта",
            "терапевтом",
            "therapist"
        },

            ["cardiologist"] = new[]
            {
            "кардиолог",
            "кардиология",
            "кардиологу",
            "кардиолога",
            "кардиологом",
            "сердце",
            "сердца",
            "сердечный",
            "сердечная",
            "cardiologist"
        },

            ["dentist"] = new[]
            {
            "стоматолог",
            "стоматология",
            "стоматологу",
            "стоматолога",
            "стоматологом",
            "зуб",
            "зубы",
            "зубная",
            "dentist"
        },

            ["neurologist"] = new[]
            {
            "невролог",
            "неврология",
            "неврологу",
            "невролога",
            "неврологом",
            "нерв",
            "головная боль",
            "мигрень",
            "neurologist"
        },

            ["pediatrician"] = new[]
            {
            "педиатр",
            "педиатрия",
            "педиатру",
            "педиатра",
            "ребенок",
            "ребёнок",
            "детский",
            "дети",
            "pediatrician"
        }
        };

        foreach (var pair in keywords)
        {
            if (!name.Contains(pair.Key))
                continue;

            foreach (var keyword in pair.Value)
            {
                if (text.Contains(keyword))
                    return true;
            }
        }

        return false;
    }

    private static string Normalize(string value)
    {
        return value
            .Trim()
            .ToLowerInvariant()
            .Replace("ё", "е");
    }
}