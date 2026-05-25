namespace DatingApp.DataAccess.Interfaces;

public interface IUnitOfWork
{
    ILikesRepository LikesRepository { get; }
    IMemberRepository MemberRepository { get; }
    IMessageRepository MessageRepository { get; }
    IAdminRepository AdminRepository { get; }
    IAccountRepository AccountRepository { get; }
    UserManager<AppUser> UserManager { get; }
    RoleManager<AppRole> RoleManager { get; }
    DataContext Db { get; }

    Task<bool> CompleteAsync();
    bool HasChanges();
}