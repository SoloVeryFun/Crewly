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
    
    ExecutorProfileMenu,
    
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
    
    ClientProfileMenu,
    
    //Client creat a task
    TaskTitle,
    TaskSpecification,
    TaskTags,
    TaskBudget,
    TaskDeadline,
    TaskAttachments,
    TaskCreationCompleted,
    
    PreparationEditTask,
    EditTask,
    
    
    //Wait for verification
    WaitForVerification,
    
    //
    Menu,
}
