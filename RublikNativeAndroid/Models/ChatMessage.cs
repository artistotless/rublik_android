using System;
using LiteNetLib;
using LiteNetLib.Utils;
namespace RublikNativeAndroid.Models
{
    public class ChatMessage : Contracts.IHasId
    {
        public string text { get; set; }
        public int authorId { get; set; }
        public int destinationId { get; set; }

        public string timeStamp { get; set; }

        public ChatMessage(int destinationId, string text, int authorId, DateTime dateTime)
        {
            this.destinationId = destinationId;
            this.text = text;
            this.authorId = authorId;
            this.timeStamp = dateTime.ToString();
        }

        public ChatMessage(int destinationId, string text)
        {
            this.destinationId = destinationId;
            this.text = text;
        }

        public ChatMessage(NetPacketReader reader)
        {
            authorId = reader.GetInt();
            destinationId = reader.GetInt();
            timeStamp = reader.GetString();
            text = reader.GetString();
        }

        public NetDataWriter GetNetDataWriter()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(destinationId);
            writer.Put(text);
            return writer;
        }

        public static NetDataWriter GetNetDataWriterFromReader(NetPacketReader reader)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(reader.GetInt());
            writer.Put(reader.GetInt());
            writer.Put(reader.GetString());
            writer.Put(reader.GetString());
            return writer;
        }

        public long GetId() => authorId;

    }
}

