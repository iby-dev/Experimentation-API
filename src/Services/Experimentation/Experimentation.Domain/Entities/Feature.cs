using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Experimentation.Domain.Entities
{
    public class Feature : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int FriendlyId { get; set; }
        public string Name { get; set; }
        public List<string> BucketList { get; set; } = new List<string>();
    }
}