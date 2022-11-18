using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Contexts.Interfaces;
using API.Models;
using MongoDB.Driver;

namespace API.BusinessLogic
{
    public class FileLogic
    {
        private readonly IMongoDbContext _dbContext;
        private readonly IMongoCollection<File> _instrumentCollection;

        public FileLogic(IMongoDbContext dbContext)
        {
            _dbContext = dbContext;
            if (!_dbContext.TryGetDatabase(out var db)) return;
            _instrumentCollection = db.GetCollection<File>(File.CollectionName);
        }

        public async Task UploadFile(byte[] fileContents, string contentType, long fileSize, string name, string category)
        {
            var content = Convert.ToBase64String(fileContents);

            var instrument = new File
            {
                Content = content,
                ContentType = contentType,
                FileSize = fileSize,
                Name = name,
                Category = category ?? ""
            };

            await _instrumentCollection.InsertOneAsync(instrument);
        }

        public async Task<List<File>> GetAllFiles()
        {
            var result = await (await _instrumentCollection.FindAsync(_ => true)).ToListAsync();

            return result.Any() ? result : new List<File>();
        }

        public async Task<File> GetFile(string contentType, string name)
        {
            var filter = Builders<File>.Filter.Eq(f => f.Name, name) &
                Builders<File>.Filter.Eq(f => f.ContentType, contentType);

            var result = await (await _instrumentCollection.FindAsync(filter, new FindOptions<File>
            {
                Collation = new Collation("en", strength: CollationStrength.Secondary)
            })).FirstOrDefaultAsync();

            return result;
        }
    }
}