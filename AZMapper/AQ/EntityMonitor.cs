using System;
using System.Collections.Generic;

namespace AZMapper.AQ
{
    public class EntityMonitor
    {
        private static readonly EntityMonitor _instance = new EntityMonitor();
        private Dictionary<Type, List<MonitoredEntity>> _entities;

        public static EntityMonitor Instance
        {
            get { return _instance; }
        }

        private EntityMonitor()
        {
            _entities = new Dictionary<Type, List<MonitoredEntity>>(20);
        }

        public void Add(object entity)
        {
            if (entity == null)
                throw new MapperException("Object cannot be null");

            var type = entity.GetType();

            if (!type.IsClass)
                throw new MapperException("Object cannot be a value type or interface");

            List<MonitoredEntity> list = null;

            if (_entities.ContainsKey(type))
            {
                list = _entities[type];
            }
            else
            {
                list = new List<MonitoredEntity>();
                _entities.Add(type, list);
            }

            //bool exists = false;

            //for (int i = 0; i < list.Count; i++)
            //{
            //    object o = list[i]._ref.Target;
            //    if (o != null && o.Equals(entity))
            //    {
            //        exists = true;
            //        break;
            //    }
            //}

            //if (!exists)
            {
                list.Add(new MonitoredEntity(entity));
            }
        }

        public void Remove(object entity)
        {
            var monitoredIdentity = Find(entity);
            if (monitoredIdentity != null)
            {
                var type = entity.GetType();
                var list = _entities[type];
                list.Remove(monitoredIdentity);
                monitoredIdentity.Dispose();
            }
        }

        public void Update(object entity)
        {
            var monitoredIdentity = Find(entity);
            if (monitoredIdentity == null)
                throw new MapperException("Entity not found");

            monitoredIdentity.UpdateValues(entity);
        }

        public MonitoredEntity Find(object entity)
        {
            var type = entity.GetType();
            if (_entities.ContainsKey(type))
            {
                var list = _entities[type];

                for (int i = 0; i < list.Count; i++)
                {
                    object o = list[i].Ref.Target;
                    if (o != null && o.Equals(entity))
                    {
                        return list[i];
                    }
                }
            }

            return null;
        }

        public List<string> GetDiff(object entity)
        {
            var monitoredEntity = Find(entity);
            if (monitoredEntity != null)
            {
                return monitoredEntity.CompareValues(entity);
            }

            return new List<string>();
        }

        public void RemoveCollected(Type type = null)
        {
            if (type == null)
            {
                foreach (var key in _entities)
                {
                    key.Value.RemoveAll((x) => { return !x.Ref.IsAlive; });
                }
            }
            else
            {
                _entities[type].RemoveAll((x) => { return !x.Ref.IsAlive; });
            }
        }

        public void Clear()
        {
            _entities.Clear();
        }
    }
}
