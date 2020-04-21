namespace Elasticsearch.Utilities.Lock
{
    using System;

    public interface ILockWork
    {
        bool DoWork(Action action);
    }
}