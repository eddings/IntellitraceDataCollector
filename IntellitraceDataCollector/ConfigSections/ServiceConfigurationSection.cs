
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace IntelliTraceDataCollector
{
    public class ServiceConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("Services", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ServiceCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ServiceCollection Services
        {
            get
            {
                return (ServiceCollection)base["Services"];
            }
        }
    }

    public class ServiceConfig : ConfigurationElement
    {
        public ServiceConfig() { }

        public ServiceConfig( string name)
        {
            Name = name;
            
        }

        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }
    }

    public class ServiceCollection : ConfigurationElementCollection
    {
        public ServiceCollection()
        {
            Console.WriteLine("ServiceCollection Constructor");
        }

        public ServiceConfig this[int index]
        {
            get { return (ServiceConfig)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(ServiceConfig serviceConfig)
        {
            BaseAdd(serviceConfig);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceConfig)element).Name;
        }

        public void Remove(ServiceConfig serviceConfig)
        {
            BaseRemove(serviceConfig.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }
    }
}
