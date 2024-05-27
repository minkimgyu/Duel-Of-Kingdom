using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Linq;

namespace WPP.Network
{
    class ByteBuffer
    {
        public List<byte> buffer;
        private byte[] _tempBuffer;
        public int readIndex { get; set; }

        public ByteBuffer()
        {
            buffer = new List<byte>();
            readIndex = 0;
        }

        public int Count()
        {
            return buffer.Count - readIndex;
        }

        public void Clear()
        {
            buffer.Clear();
            return;
        }

        public byte[] ToArray()
        {
            return buffer.ToArray();
        }

        public void WriteBytes(byte[] input)
        {
            buffer.AddRange(input);
            return;
        }
        public void WriteShort(short input)
        {
            buffer.AddRange(BitConverter.GetBytes(input));
            return;
        }

        public void WriteInteger(int input)
        {
            buffer.AddRange(BitConverter.GetBytes(input));
            return;
        }

        public void WriteLong(long input)
        {
            buffer.AddRange(BitConverter.GetBytes(input));
        }

        public void WriteFloat(float input)
        {
            buffer.AddRange(BitConverter.GetBytes(input));
            return;
        }

        public void WriteString(string input)
        {
            buffer.AddRange(BitConverter.GetBytes(input.Length));
            buffer.AddRange(Encoding.ASCII.GetBytes(input));
            return;
        }

        public void WriteEndPoint(IPEndPoint ep)
        {
            string endPointString = $"{ep.Address}:{ep.Port}";
            buffer.AddRange(BitConverter.GetBytes(endPointString.Length));
            buffer.AddRange(Encoding.ASCII.GetBytes(endPointString));
            return;
        }

        public void WriteVector3(Vector3 position)
        {
            buffer.AddRange(BitConverter.GetBytes(position.x));
            buffer.AddRange(BitConverter.GetBytes(position.y));
            buffer.AddRange(BitConverter.GetBytes(position.z));
            return;
        }

        public void WriteQuaternion(Quaternion rotation)
        {
            buffer.AddRange(BitConverter.GetBytes(rotation.x));
            buffer.AddRange(BitConverter.GetBytes(rotation.y));
            buffer.AddRange(BitConverter.GetBytes(rotation.z));
            buffer.AddRange(BitConverter.GetBytes(rotation.w));
            return;
        }

        public void WriteDateTime(DateTime dateTime)
        {
            buffer.AddRange(BitConverter.GetBytes(dateTime.Ticks));
            return;
        }

        public byte[] ReadBytes(int numOfBytesToRead, bool moveHead)
        {
            _tempBuffer = buffer.ToArray();
            byte[] returnBytes = new byte[numOfBytesToRead];
            Buffer.BlockCopy(_tempBuffer, readIndex, returnBytes, 0, numOfBytesToRead);
            if (moveHead)
                readIndex += numOfBytesToRead;
            return returnBytes;
        }

        public short ReadShort(bool moveHead)
        {
            _tempBuffer = buffer.ToArray();
            short ret = BitConverter.ToInt16(_tempBuffer, readIndex);
            if (moveHead)
                readIndex += sizeof(short);
            return ret;
        }
        public int ReadInteger(bool moveHead)
        {
            _tempBuffer = buffer.ToArray();
            int ret = BitConverter.ToInt32(_tempBuffer, readIndex);
            if (moveHead)
                readIndex += sizeof(int);
            return ret;
        }

        public long ReadLong(bool moveHead)
        {
            _tempBuffer = buffer.ToArray();
            long ret = BitConverter.ToInt64(_tempBuffer, readIndex);
            if (moveHead)
                readIndex += sizeof(long);
            return ret;
        }

        public float ReadFloat(bool moveHead)
        {
            _tempBuffer = buffer.ToArray();
            float ret = BitConverter.ToSingle(_tempBuffer, readIndex);
            if (moveHead)
                readIndex += sizeof(float);
            return ret;
        }

        public string ReadString(bool moveHead)
        {
            _tempBuffer = buffer.ToArray();
            int length = ReadInteger(true);
            string ret = Encoding.ASCII.GetString(_tempBuffer, readIndex, length);
            if (moveHead)
                readIndex += length;
            return ret;
        }

        public IPEndPoint ReadEndPoint(bool moveHead)
        {
            _tempBuffer = buffer.ToArray();
            int length = ReadInteger(true);

            string ret = Encoding.ASCII.GetString(_tempBuffer, readIndex, length);

            if (moveHead)
                readIndex += length;

            string[] parts = ret.Split(':');
            if (parts.Length == 2 && IPAddress.TryParse(parts[0], out IPAddress ipAddress) && int.TryParse(parts[1], out int port))
            {
                IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
                return endPoint;
            }
            return null;
        }

        public Vector3 ReadVector3(bool moveHead)
        {
            _tempBuffer = buffer.ToArray();

            float x = BitConverter.ToSingle(_tempBuffer, readIndex);
            if (moveHead)
                readIndex += sizeof(float);
            float y = BitConverter.ToSingle(_tempBuffer, readIndex);
            if (moveHead)
                readIndex += sizeof(float);
            float z = BitConverter.ToSingle(_tempBuffer, readIndex);
            if (moveHead)
                readIndex += sizeof(float);
            Vector3 ret = new Vector3(x, y, z);
            return ret;
        }

        public Quaternion ReadQuaternion(bool moveHead)
        {
            _tempBuffer = buffer.ToArray();
            float x = BitConverter.ToSingle(_tempBuffer, readIndex);
            if (moveHead)
                readIndex += sizeof(float);
            float y = BitConverter.ToSingle(_tempBuffer, readIndex);
            if (moveHead)
                readIndex += sizeof(float);
            float z = BitConverter.ToSingle(_tempBuffer, readIndex);
            if (moveHead)
                readIndex += sizeof(float);
            float w = BitConverter.ToSingle(_tempBuffer, readIndex);
            if (moveHead)
                readIndex += sizeof(float);
            Quaternion ret = new Quaternion(x, y, z, w);
            return ret;
        }

        public DateTime ReadDateTime(bool moveHead)
        {
            _tempBuffer = buffer.ToArray();
            long ticks = BitConverter.ToInt64(_tempBuffer.ToArray(), readIndex);
            if (moveHead)
                readIndex += sizeof(long);
            return new DateTime(ticks);
        }
    }
}