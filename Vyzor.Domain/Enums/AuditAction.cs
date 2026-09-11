using System;
using System.Collections.Generic;
using System.Text;
namespace Vyzor.Domain.Enums;

public enum AuditAction
{
    Login = 1,
    Logout = 2,
    Register = 3,

    DoctorCreated = 10,
    DoctorUpdated = 11,
    DoctorDeleted = 12,

    SpecializationCreated = 20,
    SpecializationUpdated = 21,
    SpecializationDeleted = 22,

    AppointmentCreated = 30,
    AppointmentUpdated = 31,
    AppointmentCancelled = 32,
    AppointmentCompleted = 33,

    ReviewCreated = 40,
    ReviewUpdated = 41,
    ReviewDeleted = 42,

    SubscriptionPlanCreated = 50,
    SubscriptionPlanUpdated = 51,
    SubscriptionPlanDeleted = 52,
    SubscriptionRenewed = 53,

    UserRoleChanged = 60
}