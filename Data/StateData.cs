namespace Crewly.Data;

public enum UserState
{
    Start,
    
    //Executor
    ExecutorRegistrationStart,
    ExecutorName,
    ExecutorBio,
    ExecutorSpecializations,
    ExecutorBid,
    ExecutorExperience,
    ExecutorAvailability,
    ExecutorAvatar,
    ExecutorPortfolio,
    ExecutorContacts,
    ExecutorRegistrationCompleted,
    
    //Client
    ClientRegistrationStart,
    ClientName,
    ClientBio,
    ClientType,
    ClientBudgetary,
    ClientLanguage,
    ClientAvatar,
    ClientBrandGuide,
    ClientLocation,
    ClientRegistrationCompleted,    
    
    //Wait for verification
    WaitForVerification,
    
    //
    Menu,
}
