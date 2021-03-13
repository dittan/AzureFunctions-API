using Dapper;
using Functions.Core.Entities;
using Functions.Core.Interfaces.Repositorys;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Functions.Infra.Repositorys
{
    public class UserRepository : IUserRepository
    {
        private readonly string _sqlConnectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
        private readonly ILogger _log;

        public UserRepository(ILogger<UserRepository> log)
        {
            _log = log;
        }

        public async Task<int> CreateUser(Users user)
        {
            const string sql = @"INSERT INTO Users (FirstName, LastName, DateOfBirth, Email) VALUES(@FirstName, @LastName, @DateOfBirth, @Email) 
                                SELECT CAST(SCOPE_IDENTITY() as int)";
            var parameters = new { FirstName = user.FirstName, LastName = user.LastName, DateOfBirth = user.DateOfBirth, Email = user.Email };

            using (var dbConnection = new SqlConnection(_sqlConnectionString))
            {
                int id = await dbConnection.QuerySingleAsync<int>(sql, parameters);
                return id;
            }
        }

        public async Task<bool> DeleteUser(int id)
        {
            const string sql = @"DELETE FROM Users WHERE Id = @Id";
            var parameters = new { Id = id };

            using (var dbConnection = new SqlConnection(_sqlConnectionString))
            {
                int affectedRow = await dbConnection.ExecuteAsync(sql, parameters);

                return affectedRow > 0;
            }
        }

        public async Task<Users> GetUserById(int id)
        {
            const string sql = @"SELECT [Id],[FirstName],[LastName],[DateOfBirth],[Email] FROM [Users] WHERE Id = @Id;";
            var parameters = new { Id = id };

            using(var dbConnection = new SqlConnection(_sqlConnectionString))
            {
                Users user = await dbConnection.QueryFirstOrDefaultAsync<Users>(sql, parameters);
                return user;
            }
        }

        public async Task<bool> UpdateUser(Users user)
        {
            const string sql = @"UPDATE Users SET FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth, Email = @Email WHERE Id = @Id";
            var parameters = new { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, DateOfBirth = user.DateOfBirth, Email = user.Email };

            using (var dbConnection = new SqlConnection(_sqlConnectionString))
            {
                int affectedRow = await dbConnection.ExecuteAsync(sql, parameters);

                return affectedRow > 0;
            }
        }
    }
}
