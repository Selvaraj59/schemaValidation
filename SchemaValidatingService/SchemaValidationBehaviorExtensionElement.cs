using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Configuration;
using System.Configuration;
using System.Xml.Schema;
using System.Xml;

namespace SchemaValidatingService
{
    public class SchemaValidationBehaviorExtensionElement : BehaviorExtensionElement
    {
        public SchemaValidationBehaviorExtensionElement()
        {

        }

        public override Type BehaviorType 
        { 
            get
            {
                return typeof(SchemaValidationBehavior);
            } 
        }

        protected override object CreateBehavior()
        {
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            foreach (SchemaConfigElement schemaCfg in this.Schemas)
            {
                Uri baseSchema = new Uri(AppDomain.CurrentDomain.BaseDirectory);
                string location = new Uri(baseSchema,schemaCfg.Location).ToString();
                XmlSchema schema = XmlSchema.Read(
                    new XmlTextReader(location), null
                    );
                schemaSet.Add(schema);
            }
            return new SchemaValidationBehavior(schemaSet);
        }

        [ConfigurationProperty("schemas", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(SchemasCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public SchemasCollection Schemas
        {
            get
            {
                SchemasCollection SchemasCollection =
                (SchemasCollection)base["schemas"];
                return SchemasCollection;
            }
        }
    }

    public class SchemasCollection : ConfigurationElementCollection
    {
        public SchemasCollection()
        {
            
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SchemaConfigElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((SchemaConfigElement)element).Location;
        }

        public SchemaConfigElement this[int index]
        {
            get
            {
                return (SchemaConfigElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public SchemaConfigElement this[string Name]
        {
            get
            {
                return (SchemaConfigElement)BaseGet(Name);
            }
        }

        public int IndexOf(SchemaConfigElement Schema)
        {
            return BaseIndexOf(Schema);
        }

        public void Add(SchemaConfigElement Schema)
        {
            BaseAdd(Schema);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(SchemaConfigElement Schema)
        {
            if (BaseIndexOf(Schema) >= 0)
                BaseRemove(Schema.Location);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }
    }

    public class SchemaConfigElement : ConfigurationElement
    {
        public SchemaConfigElement(String location)
        {
            this.Location = location;
        }

        public SchemaConfigElement()
        {
        }

        [ConfigurationProperty("location", DefaultValue = null, IsRequired = true, IsKey = false)]
        public string Location
        {
            get
            {
                return (string)this["location"];
            }
            set
            {
                this["location"] = value;
            }
        }
    }

}
