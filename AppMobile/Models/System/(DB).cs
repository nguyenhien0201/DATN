using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Models
{
    public static class DataExtension
    {
        public static string GetDataKey(this string s)
        {
            return s.ToUpper();
        }
    }
    public partial class DB
    {
        public const int Delete = -1;
        public const int Update = 0;
        public const int Insert = 1;

        static DataBase  _mainDB;
        static public DataBase Main => _mainDB;
        static public void Mapping(DataBase db) { _mainDB = db; }
        static public Collection GetCollection<T>() { return _mainDB.GetCollection<T>(); }
    }
    public abstract class DataList<T> : List<T>
    {
        Collection _db;
        public bool Busy { get; protected set; }
        protected virtual Collection GetDataCollection()
        {
            return DB.Main.GetCollection<T>();
        }
        public Collection Data
        {
            get
            {
                if (_db == null)
                {
                    _db = GetDataCollection();
                }
                return _db;
            }
        }
        protected abstract string GetItemKey(T item);
        public DataList<T> Load()
        {
            Busy = true;
            this.Clear();
            Data.Traversal<T>(a => base.Add(a));

            Busy = false;
            return this;
        }
        public T Find(string key)
        {
            key = key.GetDataKey();
            return base.Find(x => GetItemKey(x).GetDataKey() == key);
        }
    }
}
#region DATA MAP
namespace Models
{
    public abstract class DataMap<T> : Dictionary<string, T>
    {
        Collection _db;
        public bool Busy { get; protected set; }
        protected virtual Collection GetDataCollection()
        {
            return DB.Main.GetCollection<T>();
        }
        public Collection Data
        {
            get
            {
                if (_db == null)
                {
                    _db = GetDataCollection();
                }
                return _db;
            }
        }
        public DataMap<T> Load()
        {
            Busy = true;

            this.Clear();
            Data.Traversal((i, a) => this.Add(i.GetDataKey(), a.ToObject<T>()));

            Busy = false;
            return this;
        }
        public List<T> Load(IEnumerable<string> keys, Action<string, T> loadCallback)
        {
            var lst = new List<T>();
            foreach (var key in keys)
            {
                var id = key.GetDataKey();
                T item = default(T);

                if (!this.TryGetValue(id, out item))
                {
                    JObject o = Data.FindById(id);
                    if (o != null)
                    {
                        item = o.ToObject<T>();
                        this.Add(id, item);
                    }
                }
                if (item != null)
                {
                    lst.Add(item);
                    loadCallback?.Invoke(id, item);
                }
            }
            return lst;
        }
        public T Find(string key)
        {
            T value = default(T);
            if (string.IsNullOrEmpty(key) == false)
            {
                key = key.GetDataKey();
                TryGetValue(key, out value);
            }

            return value;
        }
        public void Insert(string id, object item, Action<string, T> insertCallback)
        {
            if (id == null)
            {
                id = new ObjectId().ToString().GetDataKey();
                Data.Insert(id, item);
            }
            else
            {
                Data.Update(id, item);

                id = id.GetDataKey();
                if (this.ContainsKey(id))
                {
                    this.Remove(id);
                }
            }

            var e = JObject.FromObject(item).ToObject<T>();
            insertCallback?.Invoke(id, e);
        }
        public void Update(string id, T item, Action<string, T> updateCallback)
        {
            id = id.GetDataKey();
            if (this.ContainsKey(id))
            {
                if (item == null)
                {
                    item = this[id];
                }
                else
                {
                    this[id] = item;
                }

                Data.Update(id, item);
                updateCallback?.Invoke(id, item);
            }
        }
        public void Delete(string id, Action<string, T> deleteCallback)
        {
            Data.FindAndDelete(id, e => {
                this.Remove(id.GetDataKey());
                deleteCallback?.Invoke(id, e.ToObject<T>());
            });
        }
        public virtual void Insert(string id, object item)
        {
            this.Insert(id, item, null);
        }
        public virtual void Delete(string id)
        {
            this.Delete(id, null);
        }
        public virtual void DeleteAll()
        {
            Clear();
            Data.Clear();
        }
        public void Update(string id, T item)
        {
            this.Update(id, item, null);
        }
        public virtual void UpdateOrInsert(string id, T item)
        {
            if (id == null)
            {
                id = new ObjectId().ToString();
            }
            id = id.GetDataKey();
            if (base.ContainsKey(id))
            {
                this[id] = item;
            }
            else
            {
                base.Add(id, item);
            }
            Data.Update(id, item);
        }
        public KeyValuePair<string, T>[] Select(Func<T, bool> where)
        {
            var lst = new List<KeyValuePair<string, T>>();
            foreach (var p in this)
            {
                if (where == null || where(p.Value)) { lst.Add(p); }
            }
            return lst.ToArray();
        }
        public KeyValuePair<string, T>[] Select<TKey>(Func<T, bool> where, Func<T, TKey> order)
        {
            return Select(where).OrderBy(x => order(x.Value)).ToArray();
        }
        protected List<DataContext> GetDataContextCore(IEnumerable<KeyValuePair<string, T>> src)
        {
            var lst = new List<DataContext>();
            foreach (var p in src)
            {
                var context = p.Value as DataContext;
                if (context == null)
                {
                    context = DataContext.FromObject(p.Value);
                }
                context.ObjectId = p.Key;
                lst.Add(context);
            }
            return lst;
        }
        public IEnumerable<DataContext> GetDataContext()
        {
            return this.GetDataContextCore(this);
        }
        public IEnumerable<DataContext> GetDataContext(Func<T, bool> where)
        {
            return this.GetDataContextCore(this.Select(where));
        }
        public IEnumerable<DataContext> GetDataContext<TKey>(Func<T, bool> where, Func<T, TKey> order)
        {
            return this.GetDataContextCore(this.Select<TKey>(where, order));
        }

        public event Action<int, T> DataChanged;
        protected virtual void RaiseDataChanged(int a, T i)
        {
            DataChanged?.Invoke(a, i);
        }
        public virtual void UpdateData(DataContext value)
        {
            var action = value.Pop<int>("#action");
            T item = default(T);
            if (action == -2)
            {
                DeleteAll();
                RaiseDataChanged(action, item);
            }
            var id = value.Pop<string>("#id");
            if (action == 0)
            {
                Insert(id, value, (s, i) => RaiseDataChanged(action, i));
                return;
            }
            if (action == -1)
            {
                Delete(id, (s, i) => RaiseDataChanged(action, i));
                return;
            }

            item = JObject.FromObject(value).ToObject<T>();
            Update(id, item, null);
            RaiseDataChanged(action, item);
        }
    }
}
#endregion