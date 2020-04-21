namespace Elasticsearch.Index.Entities
{
    using System.Runtime.Serialization;

    [DataContract]
    public abstract class BaseEntity
    {
        [DataMember(Name = "identity")]
        public int Identity { get; set; }
    }
}