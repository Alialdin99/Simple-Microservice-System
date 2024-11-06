using Play.Common;
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Play.Inventory.Service.Entities
{
    public class CatalogItem: IEntity
    {
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id{ get; set; }

        public string Name{ get; set; }

        public string Description{get; set; }
        
    }
}