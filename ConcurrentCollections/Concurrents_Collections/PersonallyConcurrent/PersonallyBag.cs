using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrent_Collections.PersonallyConcurrent
{
    internal class PersonallyBag<T>
    {
        object _locker;
        List<WorkNode<T>> _listNodes = new List<WorkNode<T>>();
        SpaceWorkNode<T> workNodes;

        volatile int _count;
        volatile int _workNodes;
        public int Count { get { return _count; } }
        public int CountWorkSpaces { get { return _workNodes; } }
        public PersonallyBag()
        {
            Initialization();
        }
        void Initialization()
        {
            _count = 0;
            _workNodes = 0;
            _locker = new object();
        }
        public IEnumerable<T> Values()
        {
            SpaceWorkNode<T> localspace = workNodes;
            List<T> list = new List<T>();
            if (localspace != null)
                while (true)
                {
                    if (localspace.Nodes == null)
                        return list;
                    lock (localspace.Nodes.ObjectLock)
                    {
                        for (int i = 0; i <= localspace.Nodes.Values.Count; i++)
                        {
                            list.Add(localspace.Nodes.Values.Dequeue());
                        }
                    }
                    localspace.Nodes = localspace.Nodes.Next;
                }
            else return list;
        }
        public bool TryTake(out T item)
        {
            Thread thread = Thread.CurrentThread;
            WorkNode<T> node = TakeValueThreadHasWorkNode(thread);
            if (node == null)
            {
                item = default!;
                return false;
            }
            lock (node.ObjectLock)
            {
            flag:
                if (node.Values.Count != 0)
                {
                    item = node.Values.Dequeue();
                    Interlocked.Decrement(ref _count);
                    if (node.Values.Count == 0)
                        RemoveEmptyWorkNode(node);
                    return true;
                }
                else
                {
                    RemoveEmptyWorkNode(node);
                    node = IsNotEmptyWorkNode(node);
                    if (node != null)
                    {
                        goto flag;
                        //item = node.Values.Dequeue();
                        //if (node.Values.Count == 0)
                        //    RemoveEmptyWorkNode(node);
                        //return true;
                    }
                    else
                    {
                        item = default!;
                        return false;
                    }
                }
            }
        }
        WorkNode<T> TakeValueThreadHasWorkNode(Thread thread)
        {
            if (workNodes == null)
                return null;
            WorkNode<T> past = null;
            WorkNode<T> workNode = null;
            if (workNodes == null && workNodes.Nodes == null)
            {
                return null;
            }
            workNode = workNodes.Nodes;
            while (true)
            {
                if (workNode == null)
                {
                    return past;
                }
                if (workNode.Thread.Equals(thread) ||
                    !workNode.Thread.IsAlive)
                {
                    lock (_locker)
                    {
                        workNode.Thread = thread;
                    }
                    return workNode;
                }
                past = workNode;
                workNode = workNode.Next;
            }
        }
        void RemoveEmptyWorkNode(WorkNode<T> node)
        {
            lock (_locker)
            {
                if (node.Next == null && node.Past == null)
                {
                    workNodes = null;
                }
                else if (node.Next != null && node.Past == null)
                {
                    WorkNode<T> next = node.Next;
                    workNodes.Nodes = next;
                }
                else
                {
                    WorkNode<T> past = node.Past;
                    if (node.Next != null)
                    {
                        past.Next = node.Next;
                    }
                    else
                    {
                        past.Next = null;
                    }
                }
                _listNodes.Remove(node);
            }

            Interlocked.Decrement(ref _workNodes);
        }
        WorkNode<T> IsNotEmptyWorkNode(WorkNode<T> node)
        {
            lock (_locker)
            {
                if (node.Past != null)
                {
                    return node.Next;
                }
                else if (node.Next != null)
                {
                    return node.Next;
                }
                else
                {
                    return null;
                }
            }
        }
        public bool TryAdd(T item)
        {
            Thread thread = Thread.CurrentThread;
            WorkNode<T> node = ThreadHasWorkNode(thread);
            if (node == null)
                return false;
            lock (node.ObjectLock)
            {
                node.Values.Enqueue(item);
                Interlocked.Increment(ref _count);
            }
            return true;
        }
        WorkNode<T> ThreadHasWorkNode(Thread thread)
        {
            WorkNode<T> past = null;
            WorkNode<T> workNode = null;
            if (workNodes == null)
            {

                lock (_locker)
                {
                    workNodes = new SpaceWorkNode<T>();
                    Interlocked.Increment(ref _workNodes);
                    WorkNode<T> node = new WorkNode<T>()
                    {
                        Thread = thread,
                        Values = new Queue<T>()
                    };
                    workNodes.Nodes = node;
                    _listNodes.Add(node);
                    return node;
                }
            }
            workNode = workNodes.Nodes;
            while (true)
            {
                if (workNode == null)
                {
                    lock (_locker)
                    {
                        Interlocked.Increment(ref _workNodes);
                        WorkNode<T> node = new WorkNode<T>()
                        {
                            Thread = thread,
                            Values = new Queue<T>()
                        };
                        if (past != null)
                        {
                            past.Next = node;
                            node.Past = past;
                        }
                        _listNodes.Add(node);
                        return node;
                    }
                }
                if (workNode.Thread.Equals(thread) ||
                    !workNode.Thread.IsAlive)
                {
                    return workNode;
                }
                past = workNode;
                workNode = workNode.Next;
            }
        }
    }
}
