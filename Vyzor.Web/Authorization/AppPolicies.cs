
namespace Vyzor.Web.Authorization;

public static class AppPolicies
{
    public const string AdminOnly = "AdminOnly";

    public const string DoctorOnly = "DoctorOnly";

    public const string PatientOnly = "PatientOnly";

    public const string DoctorOrAdmin = "DoctorOrAdmin";

    public const string PatientOrAdmin = "PatientOrAdmin";
}
