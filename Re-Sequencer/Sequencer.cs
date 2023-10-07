using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Re_Sequencer
{
    public class Sequencer
    {
        public Dictionary<int, string> Sequence { get; set; }
        public int StartingIndex { get; set; }
        public Sequencer() { 
            StartingIndex = 1;
            Sequence = new Dictionary<int, string>();
        }

        public void SendInSequence(IModel channel, int coorId, string message)
        {
            if (StartingIndex == coorId) {
                SendMessage(message, channel);
                StartingIndex++;
            } else
            {
                Sequence.Add(coorId, message);
            }
            for (int i = 0; i < Sequence.Count; i++)
            {
                foreach(var item in Sequence)
                {
                    if (item.Key == StartingIndex)
                    {
                        SendMessage(item.Value, channel);
                        StartingIndex++;
                    }
                }
            }
        }

        private void SendMessage(string message, IModel channel)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "Agg",
                                 routingKey: "InSequence",
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine($"Sent combined message: '{message}'");
        }
    }
}
