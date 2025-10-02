namespace Crewly.Data;

public enum UserRole
{
    None,
    Executor,
    Client,
}

public static class UserStateGroup
{
    public static bool IsRegistration(UserState userState) => RegistrationState.Contains(userState);
    
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
}

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