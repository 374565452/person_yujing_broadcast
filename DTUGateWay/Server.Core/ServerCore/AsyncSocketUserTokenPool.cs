using System;
using System.Collections.Generic;

namespace Server.Core.ServerCore
{
    public class AsyncSocketUserTokenPool
    {
        private Stack<AsyncSocketUserToken> userTokenPool;

        public AsyncSocketUserTokenPool(int capacity)
        {
            userTokenPool = new Stack<AsyncSocketUserToken>(capacity);
        }

        public void push(AsyncSocketUserToken item)
        {
            if (item == null)
            {
                throw new ArgumentException("item added to a AsyncSocketUserToken is not null!");
            }
            lock (userTokenPool)
            {
                this.userTokenPool.Push(item);
            }
        }

        public AsyncSocketUserToken pop()
        {
            lock (userTokenPool)
            {
                return this.userTokenPool.Pop();
            }
        }

        public int Count
        {
            get { return this.userTokenPool.Count; }
        }
    }
}
