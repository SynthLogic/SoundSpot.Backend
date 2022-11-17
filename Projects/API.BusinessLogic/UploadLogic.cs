using System;
using System.Threading.Tasks;
using API.Contexts.Interfaces;
using API.Models;
using MongoDB.Driver;

namespace API.BusinessLogic
{
    public class UploadLogic
    {
        private readonly IMongoDbContext _dbContext;
        private readonly IMongoCollection<Instrument> _instrumentCollection;

        public UploadLogic(IMongoDbContext dbContext)
        {
            _dbContext = dbContext;
            if (!_dbContext.TryGetDatabase(out var db))
            {
                return;
            }

            _instrumentCollection = db.GetCollection<Instrument>(Instrument.CollectionName);

        }

        public async Task UploadFile(byte[] fileContents, string name, string family)
        {
            var content = Convert.ToBase64String(fileContents);

            var instrument = new Instrument
            {
                Content = content,
                Name = name,
                Family = family ?? ""
            };

            await _instrumentCollection.InsertOneAsync(instrument);
        }
    }
}