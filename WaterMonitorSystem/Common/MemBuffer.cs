using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common
{
    #region 基本操作类
    /// <summary>
    /// 内存缓存对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MemBuffer<T>
    {
        ConcurrentDictionary<string, T> _list = null;

        public MemBuffer()
        {
            _list = new ConcurrentDictionary<string, T>();
        }

        /// <summary>
        /// 列表
        /// </summary>
        public ConcurrentDictionary<string, T> Item
        {
            get { return _list; }
        }

        /// <summary>
        /// 列表长度
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            lock (_list)
            {
                _list.Clear();
            }
        }

        /// <summary>
        /// 添加对象进入buffer
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="buffer">Value</param>
        public void Add(string key, T buffer)
        {
            lock (_list)
            {
                if (_list.ContainsKey(key))
                {
                    _list[key] = buffer;
                }
                else
                {
                    _list.TryAdd(key, buffer);
                }
            }
        }
        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="key">Key</param>
        public void Remove(string key)
        {
            T buffer;

            lock (_list)
            {

                _list.TryRemove(key, out buffer);
            }
        }

        /// <summary>
        /// 获取指定键值
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>如果有 true,无 false</returns>
        public bool TryGetValue(string key, out T value)
        {
            return _list.TryGetValue(key, out value);

        }

        /// <summary>
        /// 获取指定键值
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>如果有 true,无 false</returns>
        public bool TryGetValue(Guid key, out T value)
        {
            return _list.TryGetValue(key.ToString(), out value);
        }

        /// <summary>
        /// 查找指定Key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>存在返回对象，不存在 null</returns>
        public bool Keys(string key, out T buffer)
        {
            return _list.TryGetValue(key, out buffer);
        }
    }

    /// <summary>
    /// 内部队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MemQueue<T>
    {
        private Queue<T> _queue = null;

        public MemQueue()
        {
            _queue = new Queue<T>();
        }

        /// <summary>
        /// 获取队列中的数据数量
        /// </summary>
        public int Count { get { return _queue.Count; } }

        /// <summary>
        /// 清空除列
        /// </summary>
        public void Clear()
        {
            lock (_queue)
            {
                _queue.Clear();
            }
        }

        /// <summary>
        /// 添加对像至队列
        /// </summary>
        /// <param name="target"></param>
        public void Add(T target)
        {

            lock (_queue)
            {
                _queue.Enqueue(target);
            }

        }

        /// <summary>
        /// 取得最先进入队列的数据
        /// </summary>
        /// <returns>最先进入的数据</returns>
        public T Get()
        {
            T retValue = default(T);


            if (_queue.Count > 0)
            {
                lock (_queue)
                {
                    retValue = _queue.Dequeue();
                }
            }

            return retValue;
        }
    }
    #endregion
}
