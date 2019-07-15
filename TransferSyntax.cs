using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DICOMLib
{
    public static class TransferSyntaxs
    {
        static Dictionary<string, TransferSyntax> TSs = null;
        public static Dictionary<string, TransferSyntax> All
        {
            get
            {
                if (TSs == null)
                {
                    TSs = new Dictionary<string, TransferSyntax>( );
                    TransferSyntax ts = new ImplicitVRLittleEndian( );
                    TSs.Add(ts.uid, ts);
                    ts = new ExplicitVRLittleEndian( );
                    TSs.Add(ts.uid, ts);
                    ts = new ExplicitVRBigEndian( );
                    TSs.Add(ts.uid, ts);
                }
                return TSs;
            }
        }
    }

    public abstract class TransferSyntax
    {
        protected VRFactory vrfactory;
        protected DicomDictionary dictionary;

        /// <summary>
        /// 大端否
        /// </summary>
        public bool isBE;
        /// <summary>
        /// 显式否
        /// </summary>
        public bool isExplicit;
        /// <summary>
        /// 语法名
        /// </summary>
        public string name;
        /// <summary>
        /// 唯一编码
        /// </summary>
        public string uid;
        /// <summary>
        /// 值表示法解码器
        /// </summary>
        public string vrDecoder;
        protected MemoryStream ms;
        public TransferSyntax(bool isBE, bool isExplicit)
        {
            this.isBE = isBE;
            this.isExplicit = isExplicit;
            vrfactory = new VRFactory(isBE);
            dictionary = new DicomDictionary("..\\..\\..\\dicom.dic.txt");//ppt中的相对路径在bin中
            
        }

        ~TransferSyntax()
        {
            
        }

        public virtual DCMAbstractType Decode(byte[] data, ref uint idx)
        {
            //为节省资源，在构造函数中创建内存流
            MemoryStream ms = new MemoryStream();
            //流赋值
            ms.Write(data, 0, data.Length);
            ms.Position = idx;
            //reader选用
            BinaryReader reader =Select(ms);//此处ppt错误，定义为StreamReader。是困惑源。
            //element初始化
            DCMDataElement element = new DCMDataElement();
           
            //1.解码tag
            ParseTag(ref idx,ref element, reader);
            //2.解码VR
            ParseVR(ref element, reader);
            //3.解码值长度
            ParseVLength(ref element, reader, ms);
            //4.解码值
            GetValue(ref element, reader);
            
            //结束
            idx = (uint)ms.Position;//此处ppt错误 ms.Position为Long，不能直接赋值。
            reader.Close();
            ms.Close();//回收内存流空间
            return element;
        }

        public BinaryReader Select(MemoryStream ms)
        {
            BinaryReader reader;
            if (isBE)
            {
                reader = new BinaryReaderBE(ms);
             // Console.WriteLine("BEReader");
            }
            else
                reader = new BinaryReader(ms);
            //Console.WriteLine("reader");
            return reader;
        }
        public DCMDataElement ParseTag(ref uint idx,ref DCMDataElement element, BinaryReader reader)
        {  
              element.gtag = reader.ReadUInt16();
              element.etag = reader.ReadUInt16();
            //Console.WriteLine("tag");
            return element;
        }

        public DCMDataElement ParseVR(ref DCMDataElement element, BinaryReader reader)
        {
            if (element.gtag == 0xfffe)
            {
                element.vr = "Undefined";
                return element;
            }
            else if (isExplicit)
            {
                element.vr = Encoding.Default.GetString(reader.ReadBytes(2));
                LookupDictionary(ref element);
                //Console.WriteLine("vrE");
                return element;
            }
            else
            {
                LookupDictionary(ref element);
                return element;
            }
        }
        public void ParseVLength(ref DCMDataElement element, BinaryReader reader, MemoryStream ms)
        {
            bool IsSixByte = element.vr == "OB" || element.vr == "OF" || element.vr == "OW" || element.vr == "SQ" || element.vr == "UT" || element.vr == "UN";
            if (!isExplicit || element.gtag == 0xfffe)
            {
                element.length = reader.ReadUInt32();
                //Console.WriteLine("length_I");
            }
            else if (IsSixByte)
            {
                ms.Position += 2; //skip two bytes
                element.length = reader.ReadUInt32();
                //Console.WriteLine("length_six");
            }
            else
            {
                element.length = reader.ReadUInt16();
                //Console.WriteLine("length_other");
            }
        }
        public void GetValue(ref DCMDataElement element, BinaryReader reader)
        {
            if (element.length == 0xffffffff) //undefined length
                element.value = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
            else
                element.value = reader.ReadBytes((int)element.length);
            Console.WriteLine(BitConverter.ToString((byte[])element.value)); 
        }


        public void LookupDictionary(ref DCMDataElement element)//未将对象引用设置到对象的实例 原为保护方法
        {
            //查数据字典得到VR,Name,VM
            DicomDictionaryEntry entry = dictionary.GetEntry(element.gtag, element.etag);
            if (entry != null)
            {
                if (element.vr == "" || element.vr == null) element.vr = entry.VR;
                element.name = entry.Name;
                element.vm = entry.VM;
            }
            else if (element.vr == "" && element.etag == 0)
                element.vr = "UL";
            else
                Console.WriteLine("tag不存在");
            //得到VR对象实例
            element.vrparser = vrfactory.GetVR(element.vr);
            //Console.WriteLine("LookUpDic");
        }
    }

    public class ImplicitVRLittleEndian : TransferSyntax
    {
        public ImplicitVRLittleEndian() : base(false, false)
        {
            name = "ImplicitVRLittleEndian";
            uid = "1.2.840.10008.1.2";
        }
    }

    public class ExplicitVRLittleEndian : TransferSyntax
    {
        public ExplicitVRLittleEndian() : base(false, true)
        {
            name = "ExplicitVRLittleEndian";
            uid = "1.2.840.10008.1.2.1";
        }
    }

    public class ExplicitVRBigEndian : TransferSyntax
    {
        public ExplicitVRBigEndian() : base(true, true)
        {
            name = "ExplicitVRBigEndian";
            uid = "1.2.840.10008.1.2.2";
        }
    }
}
