using Aruba.Identity.Domain.Models;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Aruba.Identity.Infrastructure.Users.Repository;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(IMongoDatabase database) =>
        _collection = database.GetCollection<User>(nameof(User));

    public Task<User> GetByIdAsync(string id) =>
        _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public Task<User> GetByEmailAsync(string email) =>
        _collection.Find(x => x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();

    public Task<List<User>> GetAllAsync() =>
        _collection.Find(_ => true).ToListAsync();

    public Task InsertAsync(User entity) =>
        _collection.InsertOneAsync(entity);

    public Task UpdateAsync(User entity) =>
        _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);

    public Task DeleteAsync(string id) =>
        _collection.DeleteOneAsync(x => x.Id == id);

    public Task<List<User>> FindAsync(Expression<Func<User, bool>> predicate) =>
        _collection.Find(predicate).ToListAsync();
}