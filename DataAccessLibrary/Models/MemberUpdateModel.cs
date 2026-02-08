namespace DataAccessLibrary.Models;

using DataAccessLibrary.Entities;

public class MemberUpdateModel
{
    public string Username { get; set; }

    [MaxLength(1000)]
    public string Introduction { get; set; }

    [MaxLength(1000)]
    public string LookingFor { get; set; }

    [MaxLength(1000)]
    public string Interests { get; set; }

    [MaxLength(100)]
    public string City { get; set; }

    [MaxLength(100)]
    public string State { get; set; }

    public void ApplyTo(AppUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        user.Introduction = Introduction;
        user.LookingFor = LookingFor;
        user.Interests = Interests;
        user.City = City;
        user.State = State;
    }

    public static MemberUpdateModel FromMemberModel(MemberModel member)
    {
        ArgumentNullException.ThrowIfNull(member);

        return new MemberUpdateModel
        {
            Username = member.Username,
            Introduction = member.Introduction,
            LookingFor = member.LookingFor,
            Interests = member.Interests,
            City = member.City,
            State = member.State
        };
    }
}
