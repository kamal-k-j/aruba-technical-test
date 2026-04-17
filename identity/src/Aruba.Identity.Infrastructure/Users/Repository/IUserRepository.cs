using Aruba.Identity.Domain.Models;
using System.Linq.Expressions;

namespace Aruba.Identity.Infrastructure.Users.Repository;

public interface IUserRepository
{
    Task<User> GetByIdAsync(string id);

    Task<User> GetByEmailAsync(string email);

    Task<List<User>> GetAllAsync();

    Task InsertAsync(User entity);

    Task UpdateAsync(User entity);

    Task DeleteAsync(string id);

    Task<List<User>> FindAsync(Expression<Func<User, bool>> predicate);
}