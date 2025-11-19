using System.Text.Json.Serialization;

namespace Crewly.Data;

public enum UserRole
{
    None,
    Executor,
    Client,
}

public static class UserStateGroup
{
    public static bool IsStart(UserState state) => StartState.Contains(state);
    public static bool IsRegistration(UserState userState) => RegistrationState.Contains(userState);
    public static bool IsWaitForVerification(UserState userState) => WaitForVerification.Contains(userState);
    public static bool IsMenuAccess(UserState userState) => MenuAccess.Contains(userState);
    public static bool IsUserProfileMenu(UserState userState) => UserProfileMenu.Contains(userState);
    public static bool IsTaskCreate(UserState userState) => TaskState.Contains(userState);
    public static bool IsTaskEditing(UserState userState) => TaskEdit.Contains(userState);

    private static readonly HashSet<UserState> TaskState =
    [
        UserState.TaskTitle,
        UserState.TaskSpecification,
        UserState.TaskTags,
        UserState.TaskBudget,
        UserState.TaskDeadline,
        UserState.TaskAttachments,
        UserState.TaskCreationCompleted
    ];

    private static readonly HashSet<UserState> TaskEdit =
    [
        UserState.PreparationEditTask,
        UserState.EditTask,
    ];
    
    private static readonly HashSet<UserState> MenuAccess = 
    [
        UserState.Menu,
    ];

    private static readonly HashSet<UserState> UserProfileMenu =
    [
        UserState.ClientProfileMenu,
        UserState.ExecutorProfileMenu
    ];
    
    private static readonly HashSet<UserState> WaitForVerification =
    [
        UserState.WaitForVerification,
    ];
        
    private static readonly  HashSet<UserState> RegistrationState =
    [
        UserState.ExecutorRegistrationStart,
        UserState.ExecutorName,
        UserState.ExecutorBio,
        UserState.ExecutorSpecializations,
        UserState.ExecutorBid,
        UserState.ExecutorExperience,
        UserState.ExecutorPortfolio,
        UserState.ExecutorAvailability,
        UserState.ExecutorAvatar,
        UserState.ExecutorContacts,

        UserState.ClientRegistrationStart,
        UserState.ClientName,
        UserState.ClientBio,
        UserState.ClientType,
        UserState.ClientBudgetary,
        UserState.ClientBrandGuide,
        UserState.ClientLanguage,
        UserState.ClientLocation,
        UserState.ClientAvatar
    ];

    private static readonly HashSet<UserState> StartState =
    [
        UserState.Start,
    ];
}


//DATA

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(UserData), "base")]
[JsonDerivedType(typeof(ExecutorData), "executor")]
[JsonDerivedType(typeof(ClientData), "client")]
public class UserData
{
    public long UserId { get; set; }
    public UserRole Role { get; set; } = UserRole.None;
    public UserState State { get; set; } = UserState.Start;
}

public class ExecutorData : UserData
{
    public string? Name             { get; set; } = "";   
    public string? Bio              { get; set; } = "";
    public string? Specializations  { get; set; } = "";
    public string? Bid              { get; set; } = "";
    public string? Experience       { get; set; } = "";
    public string? Portfolio        { get; set; } = "";
    public string? Availability     { get; set; } = "";
    public string? Avatar           { get; set; } = "";
    public string? Contacts         { get; set; } = "";
}

public class ClientData : UserData
{
    public string? Name             { get; set; } = "";
    public string? Bio              { get; set; } = "";
    public string? Type             { get; set; } = "";
    public string? Budgetary        { get; set; } = "";
    public string? BrandGuide       { get; set; } = "";
    public string? Language         { get; set; } = "";
    public string? Avatar           { get; set; } = "";
    public string? Location         { get; set; } = "";
}

public class TaskData
{
    public long? OwnerId         { get; set; }
    public Guid TaskId           { get; set; } = Guid.NewGuid();
    public string? Title         { get; set; } = "";
    public string? Specification { get; set; } = "";
    public string? Tags          { get; set; } = "";
    public string? Budget        { get; set; } = "";
    public string? Deadline      { get; set; } = "";
    public string? Attachments   { get; set; } = "";
}

public class TaskEditData
{
    public long? OwnerId       { get; set; }
    public Guid taskId { get; set; }
    public string? field { get; set; } = "";
}