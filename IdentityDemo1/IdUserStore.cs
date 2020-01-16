using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityDemo1
{
    public class IdUserStore : IUserStore<IdUser>,  IUserPasswordStore<IdUser>
    {
        public async Task<IdentityResult> CreateAsync(IdUser user, CancellationToken cancellationToken)
        {
            using(var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    @"Insert into IdUsers(Id, UserName, NormalizedUserName, PasswordHash)
                        Values (@id, @userName, @normalizedUserName, @passwordHash)",
                    new
                    {
                        id = user.Id,
                        userName = user.UserName,
                        normalizedUserName = user.NormalizedUserName,
                        passwordHash = user.PasswordHash
                    }
                    );
            }
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IdUser user, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    @"Delete IdUsers Where Id = @id",
                    new
                    {
                        id = user.Id
                    }
                    );
            }
            return IdentityResult.Success;
        }

        public void Dispose()
        {
            
        }

        public async Task<IdUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<IdUser>(
                    @"Select * From IdUsers Where Id = @id",
                    new
                    {
                        id = userId
                    }
                    );
            }
        }

        public async Task<IdUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<IdUser>(
                    @"Select * From IdUsers Where NormalizedUserName = @name",
                    new
                    {
                        name = normalizedUserName
                    }
                    );
            }
        }

        public Task<string> GetNormalizedUserNameAsync(IdUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(IdUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(IdUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(IdUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(IdUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }


        public static DbConnection GetOpenConnection()
        {
            var connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;database=IdentityDemo1;trusted_connection=yes;";
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        } 
        public async Task<IdentityResult> UpdateAsync(IdUser user, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    @"Update IdUsers Set UserName = @userName
                                    , NormalizedUserName = @normalizedUserName
                                    , PasswordHash = @passwordHash
                        Where Id = @id",
                    new
                    {
                        id = user.Id,
                        userName = user.UserName,
                        normalizedUserName = user.NormalizedUserName,
                        passwordHash = user.PasswordHash
                    }
                    );
            }
            return IdentityResult.Success;
        }

        public Task SetPasswordHashAsync(IdUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(IdUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(IdUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }
    }
}
