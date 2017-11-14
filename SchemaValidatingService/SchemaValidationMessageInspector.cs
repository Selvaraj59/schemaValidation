using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.Xml.Schema;
using System.Xml;
using System.ServiceModel.Channels;
using System.IO;

namespace SchemaValidatingService
{
    public class SchemaValidationMessageInspector : IClientMessageInspector, 
                                                      IDispatchMessageInspector
    {
        XmlSchemaSet schemaSet;
        bool isClientSide;
        [ThreadStatic]
        bool isRequest;
        
        public SchemaValidationMessageInspector(XmlSchemaSet schemaSet, bool isClientSide)
        {
            this.schemaSet = schemaSet;
            this.isClientSide = isClientSide;
        }

        void ValidateMessageBody(ref System.ServiceModel.Channels.Message message, bool isRequest)
        {
            if (!message.IsFault)
            {
                XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas();
                XmlReader bodyReader = message.GetReaderAtBodyContents().ReadSubtree();
                XmlReaderSettings wrapperSettings = new XmlReaderSettings();
                wrapperSettings.CloseInput = true;
                wrapperSettings.Schemas = schemaSet;
                wrapperSettings.ValidationFlags = XmlSchemaValidationFlags.None;
                wrapperSettings.ValidationType = ValidationType.Schema;
                XmlReader wrappedReader = XmlReader.Create(bodyReader, wrapperSettings);

                // pull body into a memory backed writer to validate
                this.isRequest = isRequest;
                MemoryStream memStream = new MemoryStream();
                XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateBinaryWriter(memStream);
                xdw.WriteNode(wrappedReader, false);
                xdw.Flush(); memStream.Position = 0;
                XmlDictionaryReader xdr = XmlDictionaryReader.CreateBinaryReader(memStream, quotas);

                // reconstruct the message with the validated body
                Message replacedMessage = Message.CreateMessage(message.Version, null, xdr);
                replacedMessage.Headers.CopyHeadersFrom(message.Headers);
                replacedMessage.Properties.CopyProperties(message.Properties);
                message = replacedMessage;
            }
        }      

#region IDispatchMessageInspector Members

        object IDispatchMessageInspector.AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            ValidateMessageBody(ref request, true);
            return null;
        }

        void IDispatchMessageInspector.BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            ValidateMessageBody(ref reply, false);
        }

        #endregion

        #region IClientMessageInspector Members

        void IClientMessageInspector.AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            ValidateMessageBody(ref reply, false);
        }

        object IClientMessageInspector.BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            ValidateMessageBody(ref request, true);
            return null;
        }
        #endregion
    }
}
