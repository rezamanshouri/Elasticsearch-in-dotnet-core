namespace Elasticsearch.Index.Entities
{
    using System.Runtime.Serialization;

    [DataContract]
    public abstract class Person : BaseEntity
    {
        [DataMember(Name = "address")]
        public string Address { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}