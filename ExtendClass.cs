using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DICOMLib
{
    public static class ExtendClass
    {  
        public static byte[] ReadToEnd(this BinaryReader br)
        {
            byte[] b = null;
            int i = 0;
            while (br.PeekChar() > -1)
                b[i++] = br.ReadByte();
            return b;
        }
        //binaryReader extend method
        public static byte[] Reverse(this byte[] b)
        {
            Array.Reverse(b);
            return b;
        }
        public static byte[] ReverseForBigEndian(this byte[] b,int start , int end)//此为自己所写
        {
            int length =System.Math.Abs(end - start) + 1;
            ArrayList a = ArrayList.Adapter(b);
            a.Reverse(start<end?start:end , length);
            return (byte[])a.ToArray(typeof(byte));
        }
        public static UInt16 ReadUInt16BE(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt16(binRdr.ReadBytes(sizeof(UInt16))
            .Reverse(), 0);
           
        }
        public static Int16 ReadInt16BE(this BinaryReader binRdr)
        {
            return BitConverter.ToInt16(binRdr.ReadBytes(sizeof(Int16))
            .Reverse(), 0);
        }

        //extend class
       
    }
    public class BinaryReaderBE : BinaryReader
    {
        private byte[] a16 = new byte[2];
        private byte[] a32 = new byte[4];
        private byte[] a64 = new byte[8];
        public BinaryReaderBE(System.IO.Stream stream)
        : base(stream) { }
        public override UInt64 ReadUInt64()//新增UInt64
        {
            a64 = base.ReadBytes(8);
            Array.Reverse(a64);
            return BitConverter.ToUInt64(a64, 0);
        }
        public override uint ReadUInt32()
        {
            a32 = base.ReadBytes(4);
            Array.Reverse(a32);
            return BitConverter.ToUInt32(a32, 0);
        }
        public override UInt16 ReadUInt16()//ppt未添加override,UInt16好
        {
            a16 = base.ReadBytes(2);            
            Array.Reverse(a16);
            return BitConverter.ToUInt16(a16, 0);
        }

    }
}