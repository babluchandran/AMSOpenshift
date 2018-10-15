using AMSMicroService2.Models;
using Apache.NMS;
using Apache.NMS.Util;
using Newtonsoft.Json;
using System;

namespace AMSMicroService2.Utility
{
    public class CommonUtility
    {
        //public static void PushToActiveMQ(T opldObject, int queueNumber)
        //{
        //    Uri connecturi = new Uri("activemq:tcp://localhost:61616");

        //    // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
        //    IConnectionFactory factory = new NMSConnectionFactory(connecturi);

        //    using (IConnection connection = factory.CreateConnection())
        //    using (ISession session = connection.CreateSession())
        //    {
        //        IDestination destination = SessionUtil.GetDestination(session, GETQueueName(queueNumber));

        //        // Create the producer
        //        using (IMessageProducer producer = session.CreateProducer(destination))
        //        {
        //            // Start the connection so that messages will be processed.
        //            connection.Start();
        //            producer.DeliveryMode = MsgDeliveryMode.Persistent;

        //            var messageToQueue = JsonConvert.SerializeObject(opldObject);

        //            //var ss = "Test Message " + DateTime.Now;
        //            ITextMessage request = session.CreateTextMessage(messageToQueue);
        //            request.NMSCorrelationID = "abc";
        //            //request.Properties["NMSXGroupID"] = "cheese";
        //            //request.Properties["myHeader"] = "Cheddar";

        //            producer.Send(request);
        //        }
        //    }
        //}

        public static OPLD PullFromActiveMQ()
        {
            try
            {
                Uri connecturi = new Uri("activemq:tcp://localhost:61616");

                // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
                IConnectionFactory factory = new NMSConnectionFactory(connecturi);

                using (IConnection connection = factory.CreateConnection())
                using (ISession session = connection.CreateSession())
                {
                    IDestination destination = SessionUtil.GetDestination(session, "queue://OPLDProcess");

                    // Create a consumer and producer
                    using (IMessageConsumer consumer = session.CreateConsumer(destination))
                    using (IMessageProducer producer = session.CreateProducer(destination))
                    {
                        // Start the connection so that messages will be processed.
                        connection.Start();
                        producer.DeliveryMode = MsgDeliveryMode.Persistent;
                        
                        // Consume a message
                        ITextMessage message = consumer.Receive() as ITextMessage;
                        if (message == null)
                        {
                            return default(OPLD);
                        }
                        else
                        {
                            OPLD opld = Newtonsoft.Json.JsonConvert.DeserializeObject<OPLD>(message.Text);

                            return opld;
                        }
                    }
                }
            }
            catch {
                return default(OPLD);
            }
            
        }

        public static string GETQueueName(int queueNumber)
        {
            string queueName = string.Empty;

            if (queueNumber == 1)
            {
                queueName = "queue://OPLDProcess";
            }

            if (queueNumber == 2)
            {
                queueName = "queue://CorrelationProcess";
            }

            if (queueNumber == 3)
            {
                queueName = "queue://ServicePointCreation";
            }

            return queueName;
        }
    }
}
