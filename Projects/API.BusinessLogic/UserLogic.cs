﻿using System;
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
                HighestScore = 0,
                LatestScore = 0,
                LatestUpDateTime = DateTime.Now
            };

            await _userCollection.InsertOneAsync(user);
        }
    }
}