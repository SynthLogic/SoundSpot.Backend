using System;
using System.Threading.Tasks;
using API.Contexts.Interfaces;
using API.Models;
using MongoDB.Driver;

namespace API.BusinessLogic
{
    public class UserLogic
    {
        private readonly IMongoDbContext _dbContext;
        private readonly IMongoCollection<User> _userCollection;

        public UserLogic(IMongoDbContext dbContext)
        {
            _dbContext = dbContext;
            if (!_dbContext.TryGetDatabase(out var db)) return;
            _userCollection = db.GetCollection<User>(User.CollectionName);
        }

        public async Task CreateUser(string username, string email, string password)
        {
            var user = new User
            {
                Username = username,
                Email = email,
                Password = User.CalculateHash(password),
                ProfilePicture = null,
                HighestScore = 0,
                LatestScore = 0,
                LatestUpDateTime = DateTime.Now
            };

            await _userCollection.InsertOneAsync(user);
        }

        public async Task<User> GetUser(string email, string password)
        {
            var filter = Builders<User>.Filter.Eq(f => f.Email, email) &
                         Builders<User>.Filter.Eq(f => f.Password, User.CalculateHash(password));

            var user = await (await _userCollection.FindAsync(filter)).FirstOrDefaultAsync();

            return user;
        }

        public async Task<bool> UpdateUser(string email, string username, User updatedData)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(f => f.Email, email) &
                             Builders<User>.Filter.Eq(f => f.Username, username);

                var user = await (await _userCollection.FindAsync(filter)).FirstOrDefaultAsync();

                var properties = typeof(User).GetProperties();

                foreach (var property in properties)
                {
                    if (!property.CanRead && property.CanWrite) continue;
                    if (property.GetValue(updatedData) is null) continue;

                    if (property.PropertyType.Name == "Int32")
                    {
                        if ((int) property.GetValue(updatedData) == 0 || 
                            (int) property.GetValue(updatedData) == -1)
                            continue;
                    }
                    if (string.Equals(property.Name, "Password", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var password = property.GetValue(updatedData);
                        var hashedPassword = User.CalculateHash(password.ToString());
                        property.SetValue(user, hashedPassword);
                    }
                    else if (string.Equals(property.Name, "LatestUpDateTime", StringComparison.CurrentCultureIgnoreCase))
                    {
                        property.SetValue(user, DateTime.Now);
                    }
                    else
                    {
                        property.SetValue(user, property.GetValue(updatedData));
                    }
                }

                await _userCollection.ReplaceOneAsync(filter, user);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}