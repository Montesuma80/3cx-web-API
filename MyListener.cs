using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCX.Configuration;

namespace WebAPI
{
    public class PsArgsEventListener : IDisposable
    {
        public static readonly Dictionary<System.Type, List<string>> TypeToDataClass = new Dictionary<System.Type, List<string>>
            {
            { typeof(DN), new List<string>{"DN", "REGISTRATION", "VMBOXINFO", "FWDPROFILE"} },
            { typeof(Gateway), new List<string>{"GATEWAY" } },
            { typeof(Parameter), new List<string> { "PARAMETER" } },
            { typeof(OutboundRule), new List<string> { "OUTBOUNDRULE" } },
            { typeof(ActiveConnection), new List<string> { "CONNECTION" } },
            { typeof(PhoneSystem), new List<string> { "CFGSERVER"} },
            { typeof(Group), new List<string> { "GRP" } },
            { typeof(DeviceInfo), new List<string> { "DEVINFO" } },
            { typeof(PhoneBookEntry), new List<string> { "PHONEBOOK" } },
            { typeof(Tenant), new List<string> { "TENANT" } },
            { typeof(Statistics), new List<string> { "*S_", "STATISTICS" } }, //special starts with
            { typeof(BlackListEntry), new List<string> { "BLACKLIST" } }
            };

        public static readonly Dictionary<string, System.Type> DataClassToType = new Dictionary<string, System.Type>();
        public static readonly Dictionary<string, System.Type> WildCardDataClassToType = new Dictionary<string, System.Type>();

        protected Action<NotificationEventArgs> _insert = null;
        protected Action<NotificationEventArgs> _update = null;
        protected Action<NotificationEventArgs> _delete = null;
        protected Func<NotificationEventArgs, bool> _filter = null;
        protected Func<int, bool> _wait = null;
        static PsArgsEventListener() //build reverse Map of types
        {
            foreach (var a in TypeToDataClass)
            {
                foreach (var b in a.Value)
                {
                    if (b.StartsWith("*"))
                        WildCardDataClassToType[b.Substring(1)] = a.Key;
                    else
                        DataClassToType[b] = a.Key;

                }
            }

        }

        protected PsArgsEventListener()
        {
            PhoneSystem.Root.Updated += Root_Updated;
            PhoneSystem.Root.Inserted += Root_Inserted;
            PhoneSystem.Root.Deleted += Root_Deleted;
        }

        protected void SetArgsHandler(
            Action<NotificationEventArgs> update,
            Action<NotificationEventArgs> insert,
            Action<NotificationEventArgs> delete,
            Func<NotificationEventArgs, bool> filter,
            Func<int, bool> Wait = null)
        {
            lock (this)
            {
                _update = update;
                _insert = insert;
                _delete = delete;
                _filter = filter;
            }
        }
        public bool Wait(int milliseconds)
        {
            return _wait?.Invoke(milliseconds) != false;
        }
        public virtual void Dispose()
        {
            lock (this)
            {
                PhoneSystem.Root.Updated -= Root_Updated;
                PhoneSystem.Root.Inserted -= Root_Inserted;
                PhoneSystem.Root.Deleted -= Root_Deleted;
            }
        }

        private void Root_Deleted(object sender, NotificationEventArgs e)
        {
            if (_filter?.Invoke(e) != false)
                _delete?.Invoke(e);
        }

        private void Root_Inserted(object sender, NotificationEventArgs e)
        {
            if (_filter?.Invoke(e) != false)
                _insert?.Invoke(e);
        }

        private void Root_Updated(object sender, NotificationEventArgs e)
        {
            if (_filter?.Invoke(e) != false)
                _update?.Invoke(e);
        }
    }

    //typed listener
    //some of the types are shared across dataclasses
    //f.e. VMBOX, DN, REGISTRATION class is delivering DN object
    //so by type - requires check of the DataClass.
    //data class can be null. In this case = data class will not be verified
    //DataClass==null any DataClass with specific object type., filter==null - any DataClass object of specified type including null
    //DataClass==null, filter!=null - any DataClass object of specified type excluding null
    //DataClass==<specific>, filter==null - specific DataClass object of specified type excluding null
    //DataClass==<specific>, filter!=null - specific DataClass object of specified type excluding null
    public class PsTypeEventListener<T> : PsArgsEventListener where T : class
    {
        private string DataClass;

        internal PsTypeEventListener()
        {
            List<string> validDataClasses;
            try
            {
                //must be found if defined
                TypeToDataClass.TryGetValue(TypeToDataClass.Where(x => x.Key.IsAssignableFrom(typeof(T))).First().Key, out validDataClasses);
            }
            catch
            {
                throw new InvalidCastException($"{typeof(T).Name} is not supported");
            }
            if (validDataClasses.Count > 1)
            {
                //more then one data class defined. data class should be specified explicitly
                throw new InvalidCastException($"DataClass should be specified for PsTypeEventListener<{typeof(T).Name}>: {string.Join(", ", validDataClasses.Select(x => "'" + x + "'").ToArray())}");
            }
        }

        internal PsTypeEventListener(string DataClass)
        {
            System.Type validTypeForDataClass;
            if (!DataClassToType.TryGetValue(DataClass, out validTypeForDataClass))
            {
                try
                {
                    validTypeForDataClass = WildCardDataClassToType.Where(x => DataClass.StartsWith(x.Key)).First().Value;
                }
                catch
                {
                    throw new InvalidCastException($"'{DataClass}' is not supported");
                }
            }
            this.DataClass = DataClass;
            if (!validTypeForDataClass.IsAssignableFrom(typeof(T)))
            {
                throw new InvalidCastException($"'{DataClass}' notification must be handled using {validTypeForDataClass.Name} but requested is {typeof(T).Name}");
            }
        }

        private bool IsMyClass(string dataClass = null)
        {
            return DataClass == null || DataClass.Equals(dataClass);
        }

        private bool MyFilter(NotificationEventArgs args, Func<T, bool> filter)
        {
            if (IsMyClass(args.EntityName))
            {
                //allow even null object
                return filter == null ? true : (args.ConfObject as T != null) && filter(args.ConfObject as T);
            }
            return false;
        }

        public void SetTypeHandler(
            Action<T> update,
            Action<T> insert,
            Action<T> delete,
            Func<T, bool> filter = null,
            Func<int, bool> Wait = null)
        {
            SetArgsHandler(
                update != null ? (x) => update?.Invoke(x.ConfObject as T) : (Action<NotificationEventArgs>)null,
                insert != null ? (x) => insert?.Invoke(x.ConfObject as T) : (Action<NotificationEventArgs>)null,
                delete != null ? (x) => delete?.Invoke(x.ConfObject as T) : (Action<NotificationEventArgs>)null,
                (x) => MyFilter(x, filter),
                _wait = Wait
                );
        }
    }
}
