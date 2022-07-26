public class MessageRepository : IMessageRepository
{
	private readonly DataContext _db;

	public MessageRepository(DataContext context)
	{
		_db = context;
	}

	public void AddMessage(Message message)
	{
		_db.Messages.Add(message);
	}

	public void DeleteMessage(Message message)
	{
		_db.Messages.Remove(message);
	}

	public async Task<Message> GetMessageAsync(int id)
	{
		return await _db.Messages.FindAsync(id);
	}

	public async Task<PaginationResponseModel<PaginationList<MessageModel>>> GetMessagesForMember()
	{
		await Task.Delay(1);
		throw new NotImplementedException();
	}

	public async Task<PaginationResponseModel<PaginationList<MessageModel>>> GetMessageThread()
	{
		await Task.Delay(1);
		throw new NotImplementedException();
	}

	public async Task<bool> SaveAllAsync()
	{
		return await _db.SaveChangesAsync() > 0;
	}
}
