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
            //schemaSet.Add("http://SchemaValidation", "messages.xsd");

            return new SchemaValidationBehavior(schemaSet);
        }
    }
}
