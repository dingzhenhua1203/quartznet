/* 
* Copyright 2004-2005 OpenSymphony 
* 
* Licensed under the Apache License, Version 2.0 (the "License"); you may not 
* use this file except in compliance with the License. You may obtain a copy 
* of the License at 
* 
*   http://www.apache.org/licenses/LICENSE-2.0 
*   
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
* WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
* License for the specific language governing permissions and limitations 
* under the License.
* 
*/

/*
* Previously Copyright (c) 2001-2004 James House
*/
using System;
using System.Collections;

using Quartz.Collection;

namespace Quartz.Util
{
	/// <summary>
	/// An implementation of <code>Map</code> that wraps another <code>Map</code>
	/// and flags itself 'dirty' when it is modified.
	/// </summary>
	/// <author>James House</author>
	[Serializable]
	public class DirtyFlagMap : IDictionary, ICloneable
	{
		public virtual bool Mutable
		{
			get { return !locked; }

			set
			{
				locked = !value;
				if (locked)
				{
					// TODO should be readonly
				}
				else
				{
					map = new Hashtable(map);
				}
			}
		}

		/// <summary>
		/// Determine whether the <code>Map</code> is flagged dirty.
		/// </summary>
		public virtual bool Dirty
		{
			get { return dirty; }
		}

		/// <summary> <p>
		/// Get a direct handle to the underlying Map.
		/// </p>
		/// </summary>
		public virtual IDictionary WrappedMap
		{
			get { return map; }
		}

		public virtual object this[object key]
		{
			get { return map[key]; }
			set
			{
				map[key] = value;
				dirty = true;
			}
		}

		public virtual int Count
		{
			get { return map.Count; }
		}

		public virtual ICollection Values
		{
			get { return map.Values; }
		}

		[NonSerialized] private bool locked = false;
		private bool dirty = false;
		private Hashtable map;

		/// <summary> <p>
		/// Create a DirtyFlagMap that 'wraps' the given <code>Map</code>.
		/// </p>
		/// </summary>
		public DirtyFlagMap(IDictionary mapToWrap)
		{
			if (mapToWrap == null)
			{
				throw new ArgumentException("mapToWrap cannot be null!");
			}

			map = new Hashtable(mapToWrap);
		}

		/// <summary>
		/// Create a DirtyFlagMap that 'wraps' a <code>Hashtable</code>.
		/// </summary>
		public DirtyFlagMap()
		{
			map = new Hashtable();
		}

		/// <summary> <p>
		/// Create a DirtyFlagMap that 'wraps' a <code>HashMap</code> that has the
		/// given initial capacity.
		/// </p>
		/// 
		/// </summary>
		public DirtyFlagMap(int initialCapacity)
		{
			map = new Hashtable(initialCapacity);
		}

		/// <summary> <p>
		/// Create a DirtyFlagMap that 'wraps' a <code>HashMap</code> that has the
		/// given initial capacity and load factor.
		/// </p>
		/// 
		/// </summary>
		public DirtyFlagMap(int initialCapacity, float loadFactor)
		{
			map = new Hashtable(initialCapacity, loadFactor);
		}

		/// <summary> <p>
		/// Clear the 'dirty' flag (set dirty flag to <code>false</code>).
		/// </p>
		/// </summary>
		public virtual void ClearDirtyFlag()
		{
			dirty = false;
		}

		public virtual void Clear()
		{
			dirty = true;
			map.Clear();
		}

		public virtual bool Contains(object key)
		{
			return map.Contains(key);
		}

		public virtual bool ContainsValue(object obj)
		{
			return map.ContainsValue(obj);
		}

		public virtual ISet EntrySet()
		{
			return new HashSet(map);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is DirtyFlagMap))
			{
				return false;
			}

			Hashtable targetAux = new Hashtable((IDictionary) obj);

			if (Count == targetAux.Count)
			{
				IEnumerator sourceEnum = Keys.GetEnumerator();
				while (sourceEnum.MoveNext())
				{
					if (targetAux.Contains(sourceEnum.Current))
					{
						targetAux.Remove(sourceEnum.Current);
					}
					else
					{
						return false;
					}
				}
			}
			else
			{
				return false;
			}
			if (targetAux.Count == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return map.GetHashCode() ^ locked.GetHashCode() ^ dirty.GetHashCode();
		}

		public virtual bool isEmpty()
		{
			return (map.Count == 0);
		}

		public virtual ISet KeySet()
		{
			return new HashSet(map.Keys);
		}

		public virtual object Put(object key, object val)
		{
			dirty = true;
			object tempObject;
			tempObject = map[key];
			map[key] = val;
			return tempObject;
		}

		public virtual void PutAll(IDictionary t)
		{
			if (t != null && t.Count > 0)
			{
				dirty = true;

				ArrayList keys = new ArrayList(t.Keys);
				ArrayList values = new ArrayList(t.Values);

				for (int i = 0; i < keys.Count; i++)
				{
					this[keys[i]] = values[i];
				}
			}
		}

		public virtual void Remove(object key)
		{
			object tempObject;
			tempObject = map[key];
			map.Remove(key);
			object obj = tempObject;

			if (obj != null)
			{
				dirty = true;
			}
		}

		public virtual object Clone()
		{
			DirtyFlagMap copy;
			try
			{
				copy = (DirtyFlagMap) MemberwiseClone();
				copy.map = (Hashtable) map.Clone();
			}
			catch (Exception)
			{
				throw new Exception("Not Cloneable.");
			}

			return copy;
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return map.GetEnumerator();
		}

		public virtual void Add(object key, object value)
		{
			map.Add(key, value);
			dirty = true;
		}

		public virtual void CopyTo(Array array, int index)
		{
			object[] keys = new object[Count];
			object[] values = new object[Count];
			if (Keys != null)
			{
				Keys.CopyTo(keys, index);
			}
			if (Values != null)
			{
				Values.CopyTo(values, index);
			}
			for (int i = index; i < Count; i++)
			{
				if (keys[i] != null || values[i] != null)
				{
					array.SetValue(new DictionaryEntry(keys[i], values[i]), i);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return map.GetEnumerator();
		}

		public virtual ICollection Keys
		{
			get { return map.Keys; }
		}

		public virtual bool IsReadOnly
		{
			get { return locked; }
		}

		public virtual Boolean IsFixedSize
		{
			get { return false; }
		}
		
		private object syncRoot = new object();
		
		public virtual object SyncRoot
		{
			get { return syncRoot; }
		}

		public virtual bool IsSynchronized
		{
			get { return false; }
		}
	}
}