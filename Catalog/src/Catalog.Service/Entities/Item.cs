

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Play.Common;

namespace Catalog.Service.Entities
{
    public class Item:IEntity
    {
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public Item(Guid id, string name, string description, decimal price, DateTimeOffset createdDate)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            CreatedDate = createdDate;
        }
    }
}