using Common;
using DataAccess.Interfaces;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class PubsubRepository : IPubsubRepository
    {
        private string projId;
        public PubsubRepository(string projectId)
        {
            projId = projectId;
        }

        public async Task<string> Publish(File msg)
        {
            TopicName topic = new TopicName(projId, "boqli0topic");

            PublisherClient client = PublisherClient.Create(topic);

            string mail_serialized = JsonConvert.SerializeObject(msg);
            PubsubMessage message = new PubsubMessage
            {
                Data = ByteString.CopyFromUtf8(mail_serialized)

            };
            return await client.PublishAsync(message);
           
        }
    }
}
