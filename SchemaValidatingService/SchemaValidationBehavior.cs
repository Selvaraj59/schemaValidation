using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Description;
using System.Xml.Schema;


namespace SchemaValidatingService
{
    public class SchemaValidationBehavior : IEndpointBehavior
    {
        XmlSchemaSet schemaSet; 

        public SchemaValidationBehavior(XmlSchemaSet schemaSet)
        {
            this.schemaSet = schemaSet;
        }
   
        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            SchemaValidationMessageInspector inspector = new SchemaValidationMessageInspector(schemaSet, true);
            clientRuntime.MessageInspectors.Add(inspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            SchemaValidationMessageInspector inspector = new SchemaValidationMessageInspector(schemaSet, false);
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion
    }
}
