namespace DataAccessLibrary.Interfaces;

public interface IUnitOfWork
{
    ILikesRepository LikesRepository { get; }
    IMemberRepository MemberRepository { get; }
    IMessageRepository MessageRepository { get; }
    IAdminRepository AdminRepository { get; }
    IAccountRepository AccountRepository { get; }
    UserManager<AppUser> UserManager { get; }
    RoleManager<AppRole> RoleManager { get; }
    SignInManager<AppUser> SignInManager { get; }
    DataContext Db { get; }

    Task<bool> Complete();
    bool HasChanges();
}