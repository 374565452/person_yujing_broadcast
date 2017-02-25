using System.Collections.Generic;

namespace Server.Core.ServerCore
{
    /// <summary>
    /// 保存已经连接到服务器上的userToken信息
    /// </summary>
    public class AsyncSocketUserTokenList
    {
        private List<AsyncSocketUserToken> userTokenList;

        public List<AsyncSocketUserToken> UserTokenList
        {
            get
            {
                return userTokenList;
            }
        }

        public AsyncSocketUserTokenList()
        {
            this.userTokenList = new List<AsyncSocketUserToken>();
        }

        public int count()
        {
            lock (userTokenList)
            {
                return this.userTokenList.Count;
            }
        }

        public void add(AsyncSocketUserToken userToken)
        {
            lock (userTokenList)
            {
                this.userTokenList.Add(userToken);
            }
        }

        public void remove(AsyncSocketUserToken userToken)
        {
            lock (userTokenList)
            {
                this.userTokenList.Remove(userToken);
            }
        }

        public void copyList(ref AsyncSocketUserToken[] array)
        {
            lock (userTokenList)
            {
                array = new AsyncSocketUserToken[this.userTokenList.Count];
                this.userTokenList.CopyTo(array);
            }
        }
    }
}
