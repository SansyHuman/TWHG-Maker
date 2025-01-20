using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.TWHG.Core.Collections
{
    /// <summary>
    /// Collection that the linked list nodes are indexed by hash.
    /// </summary>
    public class IndexedLinkedList<T, TKey> : System.Collections.Generic.ICollection<T>,
        System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IReadOnlyCollection<T>,
        System.Collections.ICollection, System.Collections.Generic.IDictionary<TKey, T>,
        System.Collections.Generic.IReadOnlyDictionary<TKey, T>, System.Collections.IDictionary
    {
        private readonly Dictionary<TKey, LinkedListNode<T>> _index = new();
        private readonly LinkedList<T> _list = new();

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_list).CopyTo(array, index);
        }

        public int Count => _list.Count;

        public bool IsSynchronized => false;
        public object SyncRoot => this;

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            _list.Clear();
            _index.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (_list.Remove(item))
            {
                foreach (KeyValuePair<TKey, LinkedListNode<T>> kvp in _index)
                {
                    if (kvp.Value.Value.Equals(item))
                    {
                        _index.Remove(kvp.Key);
                        break;
                    }
                }
                return true;
            }

            return false;
        }

        int ICollection<T>.Count => _list.Count;

        bool ICollection<T>.IsReadOnly => false;

        public bool Contains(object key)
        {
            if (key is TKey tkey)
            {
                return _index.ContainsKey(tkey);
            }

            return false;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        void IDictionary.Remove(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            
            if (key is TKey tkey && _index.ContainsKey(tkey))
            {
                _index.Remove(tkey, out var removeNode);
                _list.Remove(removeNode);
            }
        }

        public bool IsFixedSize => false;

        bool IDictionary.IsReadOnly => false;

        object IDictionary.this[object key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                
                if (key is TKey tkey)
                {
                    return _index[tkey];
                }

                throw new NotSupportedException("Key is not of type " + typeof(TKey));
            }

            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                
                if (key is TKey tkey && value is T tvalue)
                {
                    if (_index.ContainsKey(tkey))
                    {
                        var node = _index[tkey];
                        node.Value = tvalue;
                    }
                    else
                    {
                        var newNode = _list.AddLast(tvalue);
                        _index.Add(tkey, newNode);
                    }
                }
                else
                {
                    throw new NotSupportedException("Key is not of type " + typeof(TKey));
                }
            }
        }

        void IDictionary.Add(object key, object value)
        {
            ((IDictionary)this)[key] = value;
        }

        void IDictionary.Clear()
        {
            _list.Clear();
            _index.Clear();
        }

        ICollection IDictionary.Values => _list;

        ICollection IDictionary.Keys => _index.Keys;

        IEnumerator<KeyValuePair<TKey, T>> IEnumerable<KeyValuePair<TKey, T>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<TKey, T>>.Add(KeyValuePair<TKey, T> item)
        {
            this[item.Key] = item.Value;
        }

        bool ICollection<KeyValuePair<TKey, T>>.Contains(KeyValuePair<TKey, T> item)
        {
            return _index.ContainsKey(item.Key) && _index[item.Key].Equals(item.Value);
        }

        void ICollection<KeyValuePair<TKey, T>>.CopyTo(KeyValuePair<TKey, T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<TKey, T>>.Remove(KeyValuePair<TKey, T> item)
        {
            if (_index.ContainsKey(item.Key))
            {
                var node = _index[item.Key];
                if (node.Value.Equals(item.Value))
                {
                    _list.Remove(node);
                    _index.Remove(item.Key);
                    return true;
                }
            }

            return false;
        }

        void ICollection<KeyValuePair<TKey, T>>.Clear()
        {
            _list.Clear();
            _index.Clear();
        }

        int ICollection<KeyValuePair<TKey, T>>.Count => _list.Count;

        bool ICollection<KeyValuePair<TKey, T>>.IsReadOnly => false;

        void IDictionary<TKey, T>.Add(TKey key, T value)
        {
            if (_index.ContainsKey(key))
            {
                throw new ArgumentException("key already exists.");
            }
            
            this[key] = value;
        }

        public bool ContainsKey(TKey key)
        {
            return _index.ContainsKey(key);
        }

        bool IDictionary<TKey, T>.Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            
            if (!_index.ContainsKey(key))
            {
                return false;
            }

            _index.Remove(key, out var node);
            _list.Remove(node);
            return true;
        }

        bool IDictionary<TKey, T>.TryGetValue(TKey key, out T value)
        {
            return ((IReadOnlyDictionary<TKey, T>)_index).TryGetValue(key, out value);
        }

        public T this[TKey key]
        {
            get => _index[key].Value;
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                
                if (_index.ContainsKey(key))
                {
                    _index[key].Value = value;
                }
                else
                {
                    var newNode = _list.AddLast(value);
                    _index.Add(key, newNode);
                }
            }
        }

        ICollection<TKey> IDictionary<TKey, T>.Keys => _index.Keys;

        ICollection<T> IDictionary<TKey, T>.Values => _list;

        int IReadOnlyCollection<T>.Count => _list.Count;

        public bool TryGetValue(TKey key, out T value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            
            bool exists = _index.TryGetValue(key, out var node);
            if (exists)
            {
                value = node.Value;
            }
            else
            {
                value = default;
            }
            
            return exists;
        }

        bool IReadOnlyDictionary<TKey, T>.ContainsKey(TKey key)
        {
            return _index.ContainsKey(key);
        }

        public IEnumerable<TKey> Keys => _index.Keys;

        public IEnumerable<T> Values => _list;

        int IReadOnlyCollection<KeyValuePair<TKey, T>>.Count => _list.Count;

        /// <summary>
        /// Adds the specified new node after the specified existing node in the LinkedList.
        /// </summary>
        /// <param name="node">The LinkedListNode after which to insert newNode.</param>
        /// <param name="key">The key to index the newNode.</param>
        /// <param name="newNode">The new LinkedListNode to add to the LinkedList.</param>
        public void AddAfter(LinkedListNode<T> node, TKey key, LinkedListNode<T> newNode)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (newNode == null)
            {
                throw new ArgumentNullException(nameof(newNode));
            }

            if (node.List != _list)
            {
                throw new InvalidOperationException("node is not in this list.");
            }

            if (newNode.List != _list && newNode.List != null)
            {
                throw new InvalidOperationException("newNode is not in this list.");
            }

            if (_index.ContainsKey(key))
            {
                throw new ArgumentException("key already exists.");
            }
            
            _list.AddAfter(node, newNode);
            _index[key] = newNode;
        }

        /// <summary>
        /// Adds the specified value after the specified existing node in the LinkedList.
        /// </summary>
        /// <param name="node">The LinkedListNode after which to insert newNode.</param>
        /// <param name="key">The key to index the newNode.</param>
        /// <param name="value">The new value to add to the LinkedList.</param>
        /// <returns>New LinkedListNode contains value.</returns>
        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, TKey key, T value)
        {
            var newNode = new LinkedListNode<T>(value);
            AddAfter(node, key, newNode);
            return newNode;
        }

        /// <summary>
        /// Adds the specified new node before the specified existing node in the LinkedList.
        /// </summary>
        /// <param name="node">The LinkedListNode before which to insert newNode.</param>
        /// <param name="key">The key to index the newNode.</param>
        /// <param name="newNode">The new LinkedListNode to add to the LinkedList.</param>
        public void AddBefore(LinkedListNode<T> node, TKey key, LinkedListNode<T> newNode)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
            
            if (newNode == null)
            {
                throw new ArgumentNullException(nameof(newNode));
            }

            if (node.List != _list)
            {
                throw new InvalidOperationException("node is not in this list.");
            }

            if (newNode.List != _list && newNode.List != null)
            {
                throw new InvalidOperationException("newNode is not in this list.");
            }

            if (_index.ContainsKey(key))
            {
                throw new ArgumentException("key already exists.");
            }
            
            _list.AddBefore(node, newNode);
            _index[key] = newNode;
        }

        /// <summary>
        /// Adds the specified value before the specified existing node in the LinkedList.
        /// </summary>
        /// <param name="node">The LinkedListNode before which to insert newNode.</param>
        /// <param name="key">The key to index the newNode.</param>
        /// <param name="value">The new value to add to the LinkedList.</param>
        /// <returns>New LinkedListNode contains value.</returns>
        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, TKey key, T value)
        {
            var newNode = new LinkedListNode<T>(value);
            AddBefore(node, key, newNode);
            return newNode;
        }

        /// <summary>
        /// Adds the specified new node at the start of the LinkedList.
        /// </summary>
        /// <param name="key">The key to index the node.</param>
        /// <param name="node">The new LinkedListNode to add to the LinkedList.</param>
        public void AddFirst(TKey key, LinkedListNode<T> node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
            
            if (node.List != _list && node.List != null)
            {
                throw new InvalidOperationException("node is not in this list.");
            }
            
            if (_index.ContainsKey(key))
            {
                throw new ArgumentException("key already exists.");
            }

            _list.AddFirst(node);
            _index[key] = node;
        }

        /// <summary>
        /// Adds the specified value at the start of the LinkedList.
        /// </summary>
        /// <param name="key">The key to index the node.</param>
        /// <param name="value">The new value to add to the LinkedList.</param>
        /// <returns>New LinkedListNode contains value.</returns>
        public LinkedListNode<T> AddFirst(TKey key, T value)
        {
            var newNode = new LinkedListNode<T>(value);
            AddFirst(key, newNode);
            return newNode;
        }

        /// <summary>
        /// Adds the specified new node at the end of the LinkedList.
        /// </summary>
        /// <param name="key">The key to index the node.</param>
        /// <param name="node">The new LinkedListNode to add to the LinkedList.</param>
        public void AddLast(TKey key, LinkedListNode<T> node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
            
            if (node.List != _list && node.List != null)
            {
                throw new InvalidOperationException("node is not in this list.");
            }
            
            if (_index.ContainsKey(key))
            {
                throw new ArgumentException("key already exists.");
            }

            _list.AddLast(node);
            _index[key] = node;
        }

        /// <summary>
        /// Adds the specified value at the end of the LinkedList.
        /// </summary>
        /// <param name="key">The key to index the node.</param>
        /// <param name="value">The new value to add to the LinkedList.</param>
        /// <returns>New LinkedListNode contains value.</returns>
        public LinkedListNode<T> AddLast(TKey key, T value)
        {
            var newNode = new LinkedListNode<T>(value);
            AddLast(key, newNode);
            return newNode;
        }

        /// <summary>
        /// Removes the specified node from the LinkedList.
        /// </summary>
        /// <param name="node">The LinkedListNode to remove from the LinkedList.</param>
        public void Remove(LinkedListNode<T> node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.List != _list)
            {
                throw new InvalidOperationException("node is not in this list.");
            }
            
            foreach (KeyValuePair<TKey, LinkedListNode<T>> kvp in _index)
            {
                if (kvp.Value == node)
                {
                    _index.Remove(kvp.Key);
                    break;
                }
            }
            _list.Remove(node);
        }

        /// <summary>
        /// Removes the node indexed by the key from the LinkedList.
        /// </summary>
        /// <param name="key">The key of the node to remove from the LinkedList.</param>
        /// <returns>true if the node successfully removed. Else, false.</returns>
        public bool RemoveByKey(TKey key)
        {
            bool removed = ((IDictionary<TKey, LinkedListNode<T>>)_index).Remove(key, out var node);
            if (removed)
            {
                _list.Remove(node);
            }

            return removed;
        }

        /// <summary>
        /// Removes the first node from the LinkedList.
        /// </summary>
        public void RemoveFirst()
        {
            if (_list.Count == 0)
            {
                throw new InvalidOperationException("list is empty.");
            }
            
            Remove(_list.First);
        }

        /// <summary>
        /// Removes the last node from the LinkedList.
        /// </summary>
        public void RemoveLast()
        {
            if (_list.Count == 0)
            {
                throw new InvalidOperationException("list is empty.");
            }
            
            Remove(_list.Last);
        }
        
        /// <summary>
        /// Gets the first node of the LinkedList.
        /// </summary>
        public LinkedListNode<T> First => _list.First;
        
        /// <summary>
        /// Gets the last node of the LinkedList.
        /// </summary>
        public LinkedListNode<T> Last => _list.Last;

        /// <summary>
        /// Gets the node indexed by the key.
        /// </summary>
        /// <param name="key">Key to find.</param>
        /// <returns>LinkedListNode indexed by the key.</returns>
        public LinkedListNode<T> GetNode(TKey key)
        {
            if (_index.ContainsKey(key))
            {
                return _index[key];
            }

            return null;
        }
    }
}
