using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public partial class DataContext : Dictionary<string, object>
    {
        public static DataContext FromObject(object obj)
        {
            var json = JObject.FromObject(obj);
            return json.ToObject<DataContext>();
        }
        public static DataContext Parse(string text)
        {
            var json = JObject.Parse(text);
            return json.ToObject<DataContext>();
        }
        public T ChangeType<T>() where T: DataContext, new()
        {
            var dst = new T();
            dst.Copy(this);

            return dst;
        }
        protected virtual object GetValueCore(string name, bool remove)
        {
            object value;
            if (base.TryGetValue(name, out value))
            {
                if (remove)
                {
                    base.Remove(name);
                }
            }
            return value;
        }
        public void Push(string name, object value)
        {
            if (value == null) return;
            if (base.ContainsKey(name))
            {
                base[name] = value;
            }
            else
            {
                base.Add(name, value);
            }
        }
        public T Pop<T>(string name)
        {
            return (T)(GetValueCore(name, true) ?? default(T));
        }
        public T ToObject<T>()
        {
            return JObject.FromObject(this).ToObject<T>();
        }
        #region GET ITEMS VALUES
        public T GetObject<T>(string name)
        {
            object v;
            if (!TryGetValue(name, out v))
            {
                return default(T);
            }

            var obj = v is string ? JObject.Parse((string)v) : JObject.FromObject(v);
            return obj.ToObject<T>();
        }
        public T GetArray<T>(string name)
        {
            object v;
            if (!TryGetValue(name, out v))
            {
                return default(T);
            }

            var obj = v is string ? JArray.Parse((string)v) : JArray.FromObject(v);
            return obj.ToObject<T>();
        }
        public T GetValue<T>(string name)
        {
            object v = default(T);
            if (TryGetValue(name, out v))
            {
                v = Convert.ChangeType(v, typeof(T));
            }
            return (T)v;
        }
        public virtual string GetString(string name) => GetValue<string>(name);
        #endregion
        public void SetObject(string name, object value)
        {
            Push(name, JObject.FromObject(value).ToString());
        }
        public virtual void SetString(string name, string value) => Push(name, value);
        public string Join(string seperator, params string[] names)
        {
            var s = string.Empty;
            foreach (var name in names)
            {
                var v = GetValueCore(name, false);
                if (v != null)
                {
                    if (s.Length > 0) s += seperator;
                    s += v;
                }
            }
            return s;
        }
        public DataContext Copy(DataContext src, params string[] names)
        {
            if (names.Length == 0)
            {
                names = src.Keys.ToArray();
            }
            foreach (var name in names)
            {
                if (this.ContainsKey(name) == false)
                {
                    var v = src[name];
                    if (v != null)
                    {
                        base.Add(name, v);
                    }
                }
            }
            return this;
        }
        public DataContext Move(DataContext dst, params string[] names)
        {
            foreach (var name in names)
            {
                var v = this.GetValueCore(name, true);
                dst.Push(name, v);
            }
            return dst;
        }
        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }
        new public object this[string name]
        {
            get
            {
                return GetValueCore(name, false);
            }
            set
            {
                Push(name, value);
            }
        }
        public virtual string GetObjectIdName() => "ObjectId";
        public virtual string ObjectId
        {
            get => (string)this[GetObjectIdName()];
            set => Push(GetObjectIdName(), value.ToString());
        }
    }
    public class DataList : List<DataContext> { }
    public class DataContextFilter
    {
        public string Key { get; set; }
        object _value;
        public event EventHandler ValueChanged;
        public object Value 
        {
            get => _value; 
            set
            {
                if (_value == value) return;

                _results = null;
                _distincts = null;
                _value = value;

                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        IEnumerable<DataContext> _results;
        public IEnumerable<DataContext> Results 
        { 
            get
            {
                if (_results == null)
                {
                    if (_value == null || Source == null)
                    {
                        return _results = new DataContext[] { };
                    }

                    var lst = new List<DataContext>();
                    foreach (var e in Source)
                    {
                        if (_value.Equals(e[Key]))
                            lst.Add(e);
                    }
                    _results = lst;
                }
                return _results;
            }
        }
        public IEnumerable<DataContext> _source;
        public IEnumerable<DataContext> Source 
        {
            get => _source;
            set
            {
                if (_source != value)
                {
                    _results = null;
                    _distincts = null;

                    _source = value;
                }
            }
        }
        List<object> _distincts;
        public List<object> DistinctValues
        {
            get
            {
                if (_distincts == null)
                {
                    var map = new Dictionary<object, DataContext>();
                    foreach (var item in Source)
                    {
                        var v = item[Key];
                        if (v != null && map.ContainsKey(v) == false)
                        {
                            map.Add(v, item);
                        }
                    }
                    _distincts = new List<object>(map.Keys);
                }
                return _distincts;
            }
        }
    }
    public class DataContextFilterList : LinkedList<DataContextFilter>
    {
        public void Append(DataContextFilter e)
        {
            e.Value = null;
            base.AddLast(e);
        }
    }
    public class DataContextFilterPath
    {
        public DataContextFilterList Filters { get; private set; } = new DataContextFilterList();
        public DataContextFilterList Selected { get; private set; } = new DataContextFilterList();
        public DataContextFilter Current => Selected.Last.Value;
        
        public void Forward(DataContextFilter e)
        {
            var node = e == null ? Filters.First : Filters.Find(e);
            if (node != null)
            {
                Selected.AddLast(node.Value);
                Filters.Remove(node);
            }

            RaiseSelectedChanged();
        }
        public void Forward(object value)
        {
            _busy = true;
            Current.Value = value;
            Forward(null);
        }
        public void Back(DataContextFilter e)
        {
            var node = Selected.Find(e).Next;
            while (node != null)
            {
                var next = node.Next;
                Filters.Append(node.Value);

                Selected.Remove(node);
                
                node = next;
            }
            RaiseSelectedChanged();
        }
        public DataContextFilterPath(params string[] keys)
        {
            foreach (var key in keys)
            {
                var filter = new DataContextFilter { Key = key };
                filter.ValueChanged += (s, e) => { 
                    foreach (var fi in Filters)
                    {
                        fi.Source = filter.Results;
                    }
                    RaiseCurrentValueChanged();
                };
                Filters.AddLast(filter);
            }
        }
        public void Start(IEnumerable<DataContext> source, string key)
        {
            var current = Filters.First(x => x.Key == key);
            current.Source = source;

            Forward(current);
        }
        public DataContextFilterPath Reset()
        {
            var node = Selected.Last;
            while (node != null)
            {
                Filters.AddFirst(node.Value);
                node = node.Previous;
            }
            Selected.Clear();
            
            _busy = true;
            Forward(Filters.First.Value);
            
            return this;
        }
        public IEnumerable<DataContext> GetResult()
        {
            if (Current.Value != null)
            {
                return Current.Results;
            }
            return Current.Source;
        }
        public event EventHandler SelectedChanged;
        protected virtual void RaiseSelectedChanged()
        {
            _busy = false;
            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler CurrentValueChanged;
        bool _busy;
        protected virtual void RaiseCurrentValueChanged()
        {
            if (_busy == false)
            {
                CurrentValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

