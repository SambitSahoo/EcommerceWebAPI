using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Core.Entities
{
    [BsonIgnoreExtraElements]
    public class ProductBrand : BaseEntity
    {
        [BsonElement("Name")]
        public string Name { get; set; }
    }
}