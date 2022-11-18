using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Contexts.Interfaces;
using API.Models;
using MongoDB.Driver;

namespace API.BusinessLogic
{
    public class InstrumentLogic
    {
        private readonly IMongoDbContext _dbContext;
        private readonly IMongoCollection<Instrument> _instrumentCollection;

        public InstrumentLogic(IMongoDbContext dbContext)
        {
            _dbContext = dbContext;
            if (!_dbContext.TryGetDatabase(out var db))
            {
                return;
            }

            _instrumentCollection = db.GetCollection<Instrument>(Instrument.CollectionName);

        }

        public async Task UploadFile(byte[] fileContents, string name, string category)
        {
            var content = Convert.ToBase64String(fileContents);

            var instrument = new Instrument
            {
                Content = content,
                Name = name,
                Category = category ?? ""
            };

            await _instrumentCollection.InsertOneAsync(instrument);
        }

        public async Task<List<Instrument>> GetAllFiles()
        {
            var result = await (await _instrumentCollection.FindAsync(_ => true)).ToListAsync();

            return result.Any() ? result : new List<Instrument>();
        }

        public async Task<Instrument> GetFile(string name)
        {
            var filter = Builders<Instrument>.Filter.Eq(f => f.Name, name);

            var result = await (await _instrumentCollection.FindAsync(filter)).FirstOrDefaultAsync();

            return result;
        }
    }
}