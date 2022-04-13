using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Reflection;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace Models
{
    public class FileManager
    {
        public DirectoryInfo Folder { get; private set; }
        public FileManager(string path)
        {
            Folder = new System.IO.DirectoryInfo(path);
            if (Folder.Exists == false)
                Folder.Create();
        }

        public string GetFileName(object id)
        {
            return Folder.FullName + '/' + id;
        }

        public FileInfo GetProjectFileInfo(object id)
        {
            return new FileInfo(this.GetFileName(id));
        }
        public JObject ReadCore(FileInfo fi)
        {
            using (var s = fi.OpenRead())
            {
                return Bson.Read(s);
            }
        }

        public void WriteCore(object id, object value)
        {
            using (var s = this.GetProjectFileInfo(id).Create())
            {
                var o = JObject.FromObject(value);
                if (o.Property("Id") != null) { o.Remove("Id"); }

                Bson.Write(s, value);
            }
        }
        public DateTime GetLastUpdate(object id)
        {
            var fi = GetProjectFileInfo(id);
            return fi.LastWriteTime;
        }
    }
    public class Collection
    {

        #region FILE MANAGER

        public FileManager FileManager { get; private set; }
        #endregion

        public string Name { get; private set; }
        public DataBase DataBase { get; private set; }
        public Collection(string name, DataBase dataBase)
        {
            Name = name;
            DataBase = dataBase;
            FileManager = new FileManager(dataBase.PhysicalPath + '/' + name);
        }
        /// <summary>
        /// Duyệt qua tất cả các ObjectId
        /// </summary>
        /// <param name="action"></param>
        public void Traversal(Action<string> action)
        {
            foreach (var name in Directory.GetFiles(FileManager.Folder.FullName))
            {
                action(Path.GetFileNameWithoutExtension(name));
            }
        }
        /// <summary>
        /// Duyệt qua tất cả các ObjectId và Object
        /// </summary>
        /// <param name="action"></param>
        public void Traversal(Action<string, JObject> action)
        {
            foreach (var fi in FileManager.Folder.GetFiles())
            {
                var doc = FileManager.ReadCore(fi);
                action?.Invoke(fi.Name, doc);
            }
        }
        /// <summary>
        /// Duyệt qua tất cả các Document
        /// </summary>
        /// <param name="action"></param>
        public void Traversal<T>(Action<T> action)
        {
            this.Traversal((i, o) => {
                action(o.ToObject<T>());
            });
        }
        public JObject FindById(string id)
        {
            var fi = FileManager.GetProjectFileInfo(id);
            if (fi.Exists)
            {
                return FileManager.ReadCore(fi);
            }
            return null;
        }
        public T FindById<T>(string id)
        {
            var o = this.FindById(id);
            if (o == null)
            {
                return default(T);
            }
            var e = o.ToObject<T>();
            return e;
        }
        public void FindAndUpdate<T>(string id, Action<T> preAction)
        {
            var item = this.FindById(id).ToObject<T>();
            if (item == null)
            {
                return;
            }
            preAction?.Invoke(item);
            this.Update(id, item);
        }
        public void FindAndUpdate(string id, Func<JObject, bool> canUpdate)
        {
            var item = this.FindById(id);
            if (item == null)
            {
                return;
            }
            if (canUpdate(item))
            {
                this.Update(id, item);
            }
        }
        public virtual void FindAndDelete(string id, Action<JObject> preAction)
        {
            FindAndDelete(id, x =>
            {
                preAction?.Invoke(x);
                return true;
            });
        }
        public virtual void FindAndDelete(string id, Func<JObject, bool> canDelete)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var item = FindById(id);
                if (item == null)
                {
                    return;
                }
                if (canDelete(item))
                {
                    Delete(id);
                }
            }
        }
        public virtual bool Contains(string id)
        {
            return FileManager.GetProjectFileInfo(id).Exists;
        }
        public virtual void Insert(object item)
        {
            Insert(new ObjectId().ToString(), item);
        }
        public virtual void Insert(string id, object item)
        {
            FileManager.WriteCore(id, item);
        }

        public void Update(object item)
        {
            Update(((IDocument)item).ObjectId, item);
        }
        public virtual void Update(string id, object item)
        {
            FileManager.WriteCore(id, item);
        }
        public virtual void Delete(string id)
        {
            var fi = FileManager.GetProjectFileInfo(id);
            if (fi.Exists)
                fi.Delete();
        }
        public virtual List<JObject> ToList()
        {
            var lst = new List<JObject>();
            Traversal((i, e) => lst.Add(e));
            return lst;
        }
        public virtual List<T> ToList<T>()
        {
            var lst = new List<T>();
            Traversal<T>(e => lst.Add(e));
            return lst;
        }
        public List<JObject> ToList(Func<JObject, bool> check)
        {
            var lst = new List<JObject>();
            foreach (var fi in FileManager.Folder.GetFiles())
            {
                var item = FileManager.ReadCore(fi);
                if (check(item))
                {
                    lst.Add(item);
                }
            }
            return lst;
        }

        public Dictionary<string, T> ToDictionary<T>(Func<string, T, bool> condition)
        {
            var map = new Dictionary<string, T>();
            Traversal((i, e) =>
            {
                var a = e.ToObject<T>();
                if (condition(i, a))
                    map.Add(i, a);
            });
            return map;
        }
        public Dictionary<string, T> ToDictionary<T>()
        {
            var map = new Dictionary<string, T>();
            Traversal((i, e) =>
            {
                var a = e.ToObject<T>();
                map.Add(i, a);
            });
            return map;
        }

        public DataBase ToDatabase()
        {
            return new DataBase(this.DataBase.ConnectionString, this.Name);
        }
        public virtual IEnumerable<T> Select<T>(Func<T, bool> where)
        {
            var lst = new List<T>();
            Traversal((i, o) => {
                var e = o.ToObject<T>();
                if (where(e)) { lst.Add(e); }
            });
            return lst;
        }
        public int Count()
        {
            return Directory.GetFiles(FileManager.Folder.FullName).Length;
        }
        public void Clear()
        {
            foreach (var fi in FileManager.Folder.GetFiles())
            {
                fi.Delete();
            }
        }
    }
    public class Collection<T> : Collection
    {
        public Collection(string name, DataBase dataBase)
            : base(name, dataBase)
        {

        }
        public Collection(DataBase dataBase)
            : base(typeof(T).Name, dataBase)
        {

        }

        new public List<T> ToList()
        {
            return base.ToList<T>();
        }
        new public T FindById(string id)
        {
            return base.FindById<T>(id);
        }
    }
}
