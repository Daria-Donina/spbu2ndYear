using System;
using System.Threading;

namespace MyBlockingPriorityQueue
{
    public class BlockingPriorityQueue<T>
    {
        /// <summary>
        /// An element of the queue.
        /// </summary>
        private class Node
        {
            public T Value { get; set; }
            public Node Next { get; set; }
            public int Priority { get; set; }

            public Node(T value, int priority, Node next)
            {
                Value = value;
                Priority = priority;
                Next = next;
            }
        }

        /// <summary>
        /// Number of elements in the queue.
        /// </summary>
        public int Length { get; private set; }
        private Node head = null;
        private Node tail = null;
        private object locker = new object();

        /// <summary>
        /// Checks if the queue is empty.
        /// </summary>
        /// <returns>True if the list is empty and false if it's not.</returns>
        public bool IsEmpty() => head == null;

        /// <summary>
        /// Adds new element to the queue.
        /// </summary>
        /// <param name="value">An element to add.</param>
        /// <param name="priority">Priority of the element.</param>
        public void Enqueue(T value, int priority)
        {
            lock (locker)
            {
                if (IsEmpty())
                {
                    head = new Node(value, priority, head);
                    ++Length;
                    tail = head;

                    // Подаем сигнал ждущему Dequeue о добавлении элемента
                    Monitor.PulseAll(locker);
                    return;
                }

                tail.Next = new Node(value, priority, tail.Next);
                ++Length;
                tail = tail.Next;
            }
        }

        private Node FindMax()
        {
            var node = head.Next;
            var maxPriority = head.Priority;
            var maxNode = head;

            for (int i = 1; i < Length; ++i)
            {
                if (node.Priority > maxPriority)
                {
                    maxPriority = node.Priority;
                    maxNode = node;
                }

                node = node.Next;
            }

            return maxNode;
        }

        private Node FindPrev(Node nodeToFind)
        {
            if (nodeToFind == head)
            {
                throw new ArgumentOutOfRangeException("Head element does not have a previous");
            }

            var node = head;

            for (int i = 0; i < Length; ++i)
            {
                if (node.Next == nodeToFind)
                {
                    break;
                }
                node = node.Next;
            }

            return node;
        }

        /// <summary>
        /// Removes element with maximum priority.
        /// </summary>
        /// <returns>Value of the removed element.</returns>
        public T Dequeue()
        {
            lock (locker)
            {
                if (IsEmpty())
                {
                    // Ждем, пока добавится новый элемент
                    Monitor.Wait(locker);
                }

                if (Length == 1)
                {
                    var value = head.Value;
                    head = null;
                    tail = null;
                    Length = 0;
                    return value;
                }
                else
                {
                    var nodeToRemove = FindMax();
                    var value = nodeToRemove.Value;

                    if (nodeToRemove == head)
                    {
                        head = head.Next;
                        --Length;
                        return value;
                    }

                    var prev = FindPrev(nodeToRemove);

                    if (nodeToRemove == tail)
                    {
                        tail = prev;
                        --Length;
                        return value;
                    }

                    prev = prev.Next.Next;
                    --Length;
                    return value;
                }
            }
        }

        /// <summary>
        /// Removes all elements from the queue.
        /// </summary>
        public void Clear()
        {
            head = null;
            Length = 0;
        }
    }
}
