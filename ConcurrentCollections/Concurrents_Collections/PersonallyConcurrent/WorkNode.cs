using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrent_Collections.PersonallyConcurrent
{
    internal class WorkNode<T>
    {
        public Queue<T> Values { get; set; } = null!;
        public Thread Thread { get; set; } = null!;
        public WorkNode<T>? Next { get; set; } = null;
        public WorkNode<T>? Past { get; set; } = null;
        public object ObjectLock { get; set; } = new object();
    }
    internal class SpaceWorkNode<T>
    {
        public WorkNode<T>? Nodes { get; set; } = null;
    }
}
