using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;

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

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public byte[] ToArray()
        {
            return buffer.ToArray();
        }

        public void WriteBytes(byte[] input)
        {
            buffer.AddRange(input);
        }
        public void WriteShort(short input)
        {
            buffer.AddRange(BitConverter.GetBytes(input));
        }

        public void WriteInteger(int input)
        {
            buffer.AddRange(BitConverter.GetBytes(input));
        }

        public void WriteLong(long input)
        {
            buffer.AddRange(BitConverter.GetBytes(input));
        }

        public void WriteFloat(float input)
        {
            buffer.AddRange(BitConverter.GetBytes(input));
        }

        public void WriteString(string input)
        {
            buffer.AddRange(BitConverter.GetBytes(input.Length));
            buffer.AddRange(Encoding.ASCII.GetBytes(input));
        }

        public void WriteEndPoint(IPEndPoint ep)
        {
            string endPointString = $"{ep.Address}:{ep.Port}";
            buffer.AddRange(BitConverter.GetBytes(endPointString.Length));
            buffer.AddRange(Encoding.ASCII.GetBytes(endPointString));
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
    }
}