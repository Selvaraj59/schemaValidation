using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Runtime.Serialization;

namespace SchemaValidatingService
{
    [DataContract(Namespace = "http://SchemaValidation")]
    public class Call
    {
        [DataMember]
        string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }

    [DataContract(Namespace = "http://SchemaValidation")]
    public class CallResponse
    {
        [DataMember]
        string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }

    [ServiceContract(Namespace = "http://SchemaValidation")]
    public interface ISchemaValidatingService
    {
        [OperationContract()]
        CallResponse Vanakkam(Call call);
        [OperationContract()]
        Message Vanakkam2(Message msg);
    }

    public class SchemaValidatingService : ISchemaValidatingService
    {
        public SchemaValidatingService()
        {
            
        }
        public CallResponse Vanakkam(Call call)
        {
            CallResponse response = new CallResponse();
            response.Text = "Hello Client!";
            return response;
        }

        public Message Vanakkam2(Message greeting)
        {
            string ns = "http://SchemaValidation";

            XmlDocument xmlFactory = new XmlDocument();
            XmlElement responseBodyElement = xmlFactory.CreateElement("VanakkamResponse", ns);
            XmlElement responseContentElement = xmlFactory.CreateElement("VanakkamResultX", ns);
            XmlElement responseParameterElement = xmlFactory.CreateElement("text", ns);
            responseParameterElement.InnerText = "Vanakkam TestClient!";
            responseContentElement.AppendChild(responseParameterElement);
            responseBodyElement.AppendChild(responseContentElement);

            return Message.CreateMessage(greeting.Version, "http://SchemaValidation/ISchemaValidatingService/Vanakkam2Response", new XmlNodeReader(responseBodyElement));
        }

    }

}
