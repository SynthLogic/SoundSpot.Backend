using System;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Models
{
    public class User
    {
        [BsonIgnore]
        public const string CollectionName = "users";
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int HighestScore { get; set; }
        public int LatestScore { get; set; }
        public DateTime LatestUpDateTime { get; set; }

        public static string CalculateHash(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return "";
            return CalculateHash(Encoding.UTF8.GetBytes(password));
        }

        private static string CalculateHash(byte[] password)
        {
            using (var md5Hash = MD5.Create())
            {
                return BitConverter.ToString(md5Hash.ComputeHash(password)).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}