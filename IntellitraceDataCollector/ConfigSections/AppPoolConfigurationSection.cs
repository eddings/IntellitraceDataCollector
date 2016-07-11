using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace IntelliTraceDataCollector
{
    public class AppPoolConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("AppPools", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(AppPoolCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public AppPoolCollection AppPools
        {
            get
            {
                return (AppPoolCollection)base["AppPools"];
            }
        }
    }

    public class AppPoolConfig : ConfigurationElement
    {
        public AppPoolConfig() { }

        public AppPoolConfig(string name)
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

    public class AppPoolCollection : ConfigurationElementCollection
    {
        public AppPoolCollection()
        {
            Console.WriteLine("AppPoolCollection Constructor");
        }

        public AppPoolConfig this[int index]
        {
            get { return (AppPoolConfig)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(AppPoolConfig appPoolConfig)
        {
            BaseAdd(appPoolConfig);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AppPoolConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AppPoolConfig)element).Name;
        }

        public void Remove(AppPoolConfig appPoolConfig)
        {
            BaseRemove(appPoolConfig.Name);
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
