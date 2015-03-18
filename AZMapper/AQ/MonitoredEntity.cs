using System;
using System.Collections.Generic;

namespace AZMapper.AQ
{
    public class MonitoredEntity : IDisposable
    {
        public WeakReference Ref { get; private set; }
        private List<MonitorItemValue> _items;
        private bool _disposed;

        private MonitoredEntity() { }

        public MonitoredEntity(object entity)
        {
            Ref = new WeakReference(entity, false);
            SetItemValues(entity);
            _disposed = false;
        }

        private void SetItemValues(object entity)
        {
            var getters = ObjectGetterStorage.Instance.GetGetters(entity.GetType());
            _items = new List<MonitorItemValue>(getters.Count);
            foreach (var getter in getters)
            {
                object val = getter.Value.Getter(entity);
                _items.Add(new MonitorItemValue() { Name = getter.Key, Value = (val == null) ? 0 : val.GetHashCode() });
            }
        }

        public List<string> CompareValues(object entity)
        {
            var getters = ObjectGetterStorage.Instance.GetGetters(entity.GetType());
            int hashCode;
            var list = new List<string>();

            foreach (var getter in getters)
            {
                object val = getter.Value.Getter(entity);
                hashCode = (val == null) ? 0 : val.GetHashCode();

                var item = _items.Find((x) => { return x.Name == getter.Key; });
                if (item.Value != hashCode)
                {
                    list.Add(getter.Key);
                }
            }

            return list;
        }

        public void UpdateValues(object source)
        {
            if (source != Ref.Target)
                throw new MapperException("Object reference mismatch");

            SetItemValues(source);
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_items != null)
                        _items.Clear();
                }

                Ref = null;
                _items = null;
                _disposed = true;
            }
        }

        #endregion

        private struct MonitorItemValue
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }
}
