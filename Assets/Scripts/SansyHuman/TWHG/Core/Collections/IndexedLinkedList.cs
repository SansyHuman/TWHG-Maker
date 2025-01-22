using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.TWHG.Core.Collections
{
    /// <summary>
    /// Collection that the linked list nodes are indexed by hash.
    /// </summary>
    public class IndexedLinkedList<T, TKey> : ICollection<T>, IReadOnlyCollection<T>, IDictionary<TKey, T>,
        IReadOnlyDictionary<TKey, T>, IDictionary
    {
        private readonly Dictionary<TKey, LinkedListNode<T>> _index = new();
        private readonly LinkedList<T> _list = new();
        private int _version = 0;
        
        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }
        
        void IDictionary.Add(object key, object value)
        {
            if (key is TKey tkey && _index.ContainsKey(tkey))
            {
                throw new ArgumentException("An item with the same key has already been added.");
            }
            
            ((IDictionary)this)[key] = value;
        }
        
        void ICollection<KeyValuePair<TKey, T>>.Add(KeyValuePair<TKey, T> item)
        {
            this[item.Key] = item.Value;
        }
        
        void IDictionary<TKey, T>.Add(TKey key, T value)
        {
            if (_index.ContainsKey(key))
            {
                throw new ArgumentException("key already exists.");
            }
            
            this[key] = value;
        }
        
        public void CopyTo(Array array, int index)
        {
            ((ICollection)_list).CopyTo(array, index);
        }
        
        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }
        
        void ICollection<KeyValuePair<TKey, T>>.CopyTo(KeyValuePair<TKey, T>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (Count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The size of the array is not enough to copy all the items in the collection.");
            }

            foreach (var kvp in _index)
            {
                array[arrayIndex++] = new KeyValuePair<TKey, T>(kvp.Key, kvp.Value.Value);
            }
        }
                
        public void Clear()
        {
            _list.Clear();
            _index.Clear();
            _version++;
        }
        
        void IDictionary.Clear()
        {
            ((ICollection<T>)this).Clear();
        }
        
        void ICollection<KeyValuePair<TKey, T>>.Clear()
        {
            ((ICollection<T>)this).Clear();
        }

        public int Count => _list.Count;

        int ICollection<T>.Count => ((ICollection)this).Count;
        
        int ICollection<KeyValuePair<TKey, T>>.Count => ((ICollection)this).Count;
        
        int IReadOnlyCollection<T>.Count => ((ICollection)this).Count;
        
        int IReadOnlyCollection<KeyValuePair<TKey, T>>.Count => ((ICollection)this).Count;

        public bool IsSynchronized => false;
        public object SyncRoot => this;

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
        
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.EnumeratorType.DictEntry);
        }
        
        IEnumerator<KeyValuePair<TKey, T>> IEnumerable<KeyValuePair<TKey, T>>.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.EnumeratorType.KeyValuePair);
        }

        [Serializable]
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, T>>, IDictionaryEnumerator
        {
            private IndexedLinkedList<T, TKey> _list;
            private IEnumerator<TKey> _keyEnumerator;
            private bool _valid;
            private int _version;
            private KeyValuePair<TKey, T> _current;
            private EnumeratorType _enumeratorType;

            public enum EnumeratorType
            {
                DictEntry,
                KeyValuePair
            }

            public Enumerator(IndexedLinkedList<T, TKey> list, EnumeratorType enumeratorType)
            {
                _list = list;
                _keyEnumerator = list.Keys.GetEnumerator();
                _version = list._version;
                _valid = false;
                _enumeratorType = enumeratorType;
                _current = new KeyValuePair<TKey, T>();
            }

            public KeyValuePair<TKey, T> Current => _current;

            object IEnumerator.Current
            {
                get
                {
                    if (!_valid)
                    {
                        throw new InvalidOperationException("Enumerator is not valid.");
                    }

                    if (_enumeratorType == EnumeratorType.DictEntry)
                    {
                        return new DictionaryEntry(_current.Key, _current.Value);
                    }
                    else
                    {
                        return _current;
                    }
                }
            }
            
            public bool MoveNext()
            {
                if (_version != _list._version)
                {
                    _valid = false;
                    throw new InvalidOperationException("Collection was modified while enumerating the list.");
                }

                bool hasNext = _keyEnumerator.MoveNext();
                if (hasNext)
                {
                    _valid = true;
                    _current = new KeyValuePair<TKey, T>(_keyEnumerator.Current, _list.GetNode(_keyEnumerator.Current).Value);
                }
                else
                {
                    _valid = false;
                    _current = new KeyValuePair<TKey, T>();
                }

                return hasNext;
            }

            public void Reset()
            {
                if (_version != _list._version)
                {
                    _valid = false;
                    throw new InvalidOperationException("Collection was modified while enumerating the list.");
                }

                _keyEnumerator.Reset();
                _valid = false;
            }

            public void Dispose()
            {
                
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if (!_valid)
                    {
                        throw new InvalidOperationException("Enumerator is not valid.");
                    }
                    
                    return new DictionaryEntry(_current.Key, _current.Value);
                }
            }

            object IDictionaryEnumerator.Key
            {
                get
                {
                    if (!_valid)
                    {
                        throw new InvalidOperationException("Enumerator is not valid.");
                    }

                    return _current.Key;
                }
            }

            object IDictionaryEnumerator.Value
            {
                get
                {
                    if (!_valid)
                    {
                        throw new InvalidOperationException("Enumerator is not valid.");
                    }
                    
                    return _current.Value;
                }
            }
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public bool Contains(object key)
        {
            if (key is TKey tkey)
            {
                return _index.ContainsKey(tkey);
            }

            return false;
        }
        
        bool ICollection<KeyValuePair<TKey, T>>.Contains(KeyValuePair<TKey, T> item)
        {
            return _index.ContainsKey(item.Key) && _index[item.Key].Value.Equals(item.Value);
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

                _version++;
                return true;
            }

            return false;
        }
        
        void IDictionary.Remove(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            
            if (key is TKey tkey)
            {
                ((IDictionary<TKey, T>)this).Remove(tkey);
            }
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
                    _version++;
                    return true;
                }
            }

            return false;
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
            _version++;
            return true;
        }

        bool ICollection<T>.IsReadOnly => false;

        bool IDictionary.IsReadOnly => ((ICollection<T>)this).IsReadOnly;
        
        bool ICollection<KeyValuePair<TKey, T>>.IsReadOnly => ((ICollection<T>)this).IsReadOnly;

        public bool IsFixedSize => false;
        
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

                _version++;
            }
        }
        
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
                    return this[tkey];
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
                    this[tkey] = tvalue;
                }
                else
                {
                    throw new NotSupportedException("Key is not of type " + typeof(TKey));
                }
            }
        }
        
        public IEnumerable<T> Values => _list;
        
        ICollection IDictionary.Values => _list;
        
        ICollection<T> IDictionary<TKey, T>.Values => _list;
        
        public IEnumerable<TKey> Keys => _index.Keys;
        
        ICollection IDictionary.Keys => _index.Keys;
        
        ICollection<TKey> IDictionary<TKey, T>.Keys => _index.Keys;

        public bool ContainsKey(TKey key)
        {
            return _index.ContainsKey(key);
        }
        
        bool IReadOnlyDictionary<TKey, T>.ContainsKey(TKey key)
        {
            return ((IDictionary<TKey, T>)this).ContainsKey(key);
        }
        
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
        
        bool IDictionary<TKey, T>.TryGetValue(TKey key, out T value)
        {
            return ((IReadOnlyDictionary<TKey, T>)this).TryGetValue(key, out value);
        }

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
            _index.Add(key, newNode);
            _version++;
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
            _index.Add(key, newNode);
            _version++;
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
            _index.Add(key, node);
            _version++;
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
            _index.Add(key, node);
            _version++;
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
            _version++;
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
                _version++;
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
