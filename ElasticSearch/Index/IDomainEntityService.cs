namespace Elasticsearch.Index
{
    using System.Collections.Generic;

    using Elasticsearch.Index.Entities;

    public interface IDomainEntityService
    {
        List<Person> GetPeople();
    }
}