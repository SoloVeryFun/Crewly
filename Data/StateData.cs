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
    ExecutorPortfolio,
    ExecutorAvailability,
    ExecutorAvatar,
    ExecutorContacts,
    ExecutorRegistrationCompleted,
    
    //Client
    ClientRegistrationStart,
    ClientName,
    ClientBio,
    ClientType,
    ClientBudgetary,
    ClientBrandGuide,
    ClientLanguage,
    ClientLocation,
    ClientAvatar,
    ClientRegistrationCompleted,    
    
    //Wait for verification
    WaitForVerification,
    
    Main,
}
