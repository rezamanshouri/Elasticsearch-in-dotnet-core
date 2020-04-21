namespace Elasticsearch.Utilities.Lock
{
    using System;
    using System.Threading;

    public class LockWork : ILockWork
    {
        private readonly object Lock = new object();

        public bool DoWork(Action action)
        {
            var lockTaken = false;
            try
            {
                Monitor.TryEnter(this.Lock, ref lockTaken);
                if (lockTaken)
                {
                    action();
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(this.Lock);
                }
            }

            return true;
        }
    }
}