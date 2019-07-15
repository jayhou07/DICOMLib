using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DICOMLib
{
    public abstract class VR
    {
        public bool isBE;
        public bool isLongVR;

        public VR(bool isBE, bool isLongVR)
        {
            this.isBE = isBE;
            this.isLongVR = isLongVR;
        }

        public abstract T GetValue<T>(byte[] data, int startIndex);

        public abstract string ToString(byte[] data, int startIndex, string head);
    }

    public class UL : VR
    {
        public UL(bool isBE, bool isLongVR):base(isBE,isLongVR) { }
        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(UInt32))
            {
                byte[] val = data;
                int idx;
                if (isBE)
                {
                    val = data.ReverseForBigEndian(startIndex, 4);
                    idx = 0;
                }
                else
                    idx = startIndex;
                return (T)(object)BitConverter.ToUInt32(val, idx);
            }
            else
                throw new NotSupportedException();
        }

        public override string ToString(byte[] data, int startIndex, string head)
        {
            UInt32 value = GetValue<UInt32>(data, startIndex);
            return value.ToString();
        }
    }

    public class OW : VR
    {
        public OW(bool isBE, bool isLVR) : base(isBE, isLVR) { }       
        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(UInt16[]))
            {
                UInt16[] vals = new UInt16[(data.Length - startIndex) / 2];              
                byte[] val = data;
                int idx, i = 0;
                while (startIndex <data.Length)
                {
                    if (isBE)
                    {
                        val = data.ReverseForBigEndian(startIndex, 2);
                        idx = 0;
                    }
                    else
                        idx = startIndex;
                    vals[i++] = BitConverter.ToUInt16(val, idx);
                    startIndex += 2;
                }
                return (T)(object)vals;
            }
            else if (typeof(T) == typeof(Int16[]))
            {
                Int16[] vals = new Int16[(data.Length - startIndex) / 2];               
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    if (isBE)
                    {
                        val = data.ReverseForBigEndian(startIndex, 2);
                        idx = 0;
                    }
                    else
                        idx = startIndex;
                    vals[i++] = BitConverter.ToInt16(val, idx);
                    startIndex += 2;
                }
                return (T)(object)vals;
            }
            else 
                throw new NotSupportedException();
        }
        public override string ToString(byte[] data, int startIndex, string head)
        {
            Int16[] value = GetValue<Int16[]>(data, startIndex);
            string str = "";
            for(int i=0;i<10;i++)//   数组拼接
                str += value[i].ToString() + ", ";
            return str;
        }
    }



    public class ST : VR
    {
        public ST(bool isBE, bool isLongVR) : base(isBE, isLongVR){}

        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(byte[]))
            {
                byte[] vals = new byte[data.Length - startIndex];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    idx = startIndex;
                    vals[i++] = val[idx];
                    startIndex++;
                }
                return (T)(object)vals;
            }
            else
                //throw new NotSupportedException();
                return default(T);
        }
        public override string ToString(byte[] data, int startIndex, string head)
        {
            byte[] value = GetValue<byte[]>(data, startIndex);
            return Encoding.ASCII.GetString(value);
        }
    }

    public class LO : VR
    {
        public LO(bool isBE, bool isLongVR) : base(isBE, isLongVR) { }
        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(byte[]))
            {
                byte[] vals = new byte[data.Length - startIndex];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    idx = startIndex;
                    vals[i++] = val[idx];
                    startIndex++;
                }
                return (T)(object)vals;
            }
            else
                throw new NotSupportedException();
        }
        public override string ToString(byte[] data, int startIndex, string head)
        {
            byte[] value = GetValue<byte[]>(data, startIndex);
            return System.Text.Encoding.ASCII.GetString(value);
        }
    }

    public class UN : VR
    {
        public UN(bool isBE, bool isLongVR) : base(isBE, isLongVR)
        {

        }

        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(UInt16[]))
            {
                UInt16[] vals = new UInt16[(data.Length - startIndex) / 2];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    if (isBE)
                    {
                        val = data.ReverseForBigEndian(startIndex, 2);
                        idx = 0;
                    }
                    else
                        idx = startIndex;
                    vals[i++] = BitConverter.ToUInt16(val, idx);
                    startIndex += 2;
                }
                return (T)(object)vals;
            }
            else
                throw new NotSupportedException();

        }

        public override string ToString(byte[] data, int startIndex, string head)
        {
            UInt16[] value = GetValue<UInt16[]>(data, startIndex);
            return value.ToString();
        }
    }

    public class UT : VR
    {
        public UT(bool isBE, bool isLongVR) : base(isBE, isLongVR)
        {

        }

        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(UInt16[]))
            {
                UInt16[] vals = new UInt16[(data.Length - startIndex) / 2];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    if (isBE)
                    {
                        val = data.ReverseForBigEndian(startIndex, 2);
                        idx = 0;
                    }
                    else
                        idx = startIndex;
                    vals[i++] = BitConverter.ToUInt16(val, idx);
                    startIndex += 2;
                }
                return (T)(object)vals;
            }
            else
                throw new NotSupportedException();

        }

        public override string ToString(byte[] data, int startIndex, string head)
        {
            UInt16[] value = GetValue<UInt16[]>(data, startIndex);
            return value.ToString();
        }
    }

    public class SQ : VR
    {
        public SQ(bool isBE, bool isLongVR) : base(isBE, isLongVR)
        {

        }

        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(UInt16[]))
            {
                UInt16[] vals = new UInt16[(data.Length - startIndex) / 2];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    if (isBE)
                    {
                        val = data.ReverseForBigEndian(startIndex, 2);
                        idx = 0;
                    }
                    else
                        idx = startIndex;
                    vals[i++] = BitConverter.ToUInt16(val, idx);
                    startIndex += 2;
                }
                return (T)(object)vals;
            }
            else
                throw new NotSupportedException();

        }

        public override string ToString(byte[] data, int startIndex, string head)
        {
            UInt16[] value = GetValue<UInt16[]>(data, startIndex);
            return value.ToString();
        }
    }

    public class OF : VR
    {
        public OF(bool isBE, bool isLongVR) : base(isBE, isLongVR) { }
        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(float[]))
            {
                float[] vals = new float[(data.Length - startIndex) / 2];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    if (isBE)
                    {
                        val = data.ReverseForBigEndian(startIndex, 2);
                        idx = 0;
                    }
                    else
                        idx = startIndex;
                    vals[i++] = BitConverter.ToSingle(val, idx);//single即float
                    startIndex += 2;
                }
                return (T)(object)vals;
            }
            else
                throw new NotSupportedException();
        }
        public override string ToString(byte[] data, int startIndex, string head)
        {
            float[] value = GetValue<float[]>(data, startIndex);
            string str = "";
            foreach (float datum in value)
                str += datum.ToString() + ", ";
            return str;
        }
    }

    public class OB : VR
    {
        public OB(bool isBE, bool isLongVR) : base(isBE, isLongVR) { }
        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(byte[]))
            {
                byte[] vals = new byte[(data.Length - startIndex) / 2];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    if (isBE)
                    {
                        val = data.ReverseForBigEndian(startIndex, 2);
                        idx = 0;
                    }
                    else
                        idx = startIndex;
                    vals[i++] = val[idx];
                    startIndex += 2;
                }
                return (T)(object)vals;
            }
            else
                throw new NotSupportedException();
        }
        public override string ToString(byte[] data, int startIndex, string head)
        {
            byte[] value = GetValue<byte[]>(data, startIndex);
            string str = "";
            foreach (byte datum in value)
                str += datum.ToString() + ", ";
            return str;
        }
    }

    public class AS : VR
    {
        public AS(bool isBE, bool isLongVR) : base(isBE, isLongVR) { }
        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(byte[]))
            {
                byte[] vals = new byte[data.Length - startIndex];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    idx = startIndex;
                    vals[i++] = val[idx];
                    startIndex++;
                }
                return (T)(object)vals;
            }
            else
                throw new NotSupportedException();
        }
        public override string ToString(byte[] data, int startIndex, string head)
        {
            byte[] value = GetValue<byte[]>(data, startIndex);
            return Encoding.ASCII.GetString(value);
        }
    }

    public class UI : VR
    {
        public UI(bool isBE, bool isLongVR) : base(isBE, isLongVR) { }
        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(string))
            {
                byte[] vals = null;
                if (data[data.Length - 1] != 0x00)
                    vals = new byte[data.Length - startIndex];
                else
                    vals = new byte[data.Length - startIndex - 1];//回避偶数位填充字节的/0影响
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < vals.Length)
                {
                    idx = startIndex;
                    vals[i++] = val[idx];
                    startIndex++;
                }
                return (T)(object)Encoding.Default.GetString(vals);
            }
            else if(typeof(T) == typeof(byte[]))
            {
                byte[] vals = new byte[data.Length - startIndex];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    idx = startIndex;
                    vals[i++] = val[idx];
                    startIndex++;
                }
                return (T)(object)vals;
            }
            else
                throw new NotSupportedException();
        }
        public override string ToString(byte[] data, int startIndex, string head)
        {
            byte[] value = null;
            if (GetValue<byte[]>(data, startIndex) != null)
                value = GetValue<byte[]>(data, startIndex);
            else
                value = Encoding.Default.GetBytes(" ");
            return Encoding.Default.GetString(value);
        }
    }

    public class DT : VR
    {
        public DT(bool isBE, bool isLongVR) : base(isBE, isLongVR)
        {

        }

        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(UInt16[]))
            {
                UInt16[] vals = new UInt16[(data.Length - startIndex) / 2];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    if (isBE)
                    {
                        val = data.ReverseForBigEndian(startIndex, 2);
                        idx = 0;
                    }
                    else
                        idx = startIndex;
                    vals[i++] = BitConverter.ToUInt16(val, idx);
                    startIndex += 2;
                }
                return (T)(object)vals;
            }
            else
                throw new NotSupportedException();

        }

        public override string ToString(byte[] data, int startIndex, string head)
        {
            UInt16[] value = GetValue<UInt16[]>(data, startIndex);
            return value.ToString();
        }
    }

    public class TM : VR
    {
        public TM(bool isBE, bool isLongVR) : base(isBE, isLongVR)
        {

        }

        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(byte[]))
            {
                byte[] vals = new byte[data.Length - startIndex];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    idx = startIndex;
                    vals[i++] = val[idx];
                    startIndex++;
                }
                return (T)(object)vals;
            }
            else
                //throw new NotSupportedException();
                return default(T);
        }
        public override string ToString(byte[] data, int startIndex, string head)
        {
            byte[] value = GetValue<byte[]>(data, startIndex);
            return Encoding.ASCII.GetString(value);
        }
    }

    public class DA : VR
    {
        public DA(bool isBE, bool isLongVR) : base(isBE, isLongVR) { }
        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(byte[]))
            {
                byte[] vals = new byte[data.Length - startIndex];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    idx = startIndex;
                    vals[i++] = val[idx];
                    startIndex ++;
                }
                return (T)(object)vals;
            }
            else
                throw new NotSupportedException();
        }
        public override string ToString(byte[] data, int startIndex, string head)
        {
            byte[] value = GetValue<byte[]>(data, startIndex);
            return System.Text.Encoding.ASCII.GetString(value);
        }
    }

    public class FD : VR
    {
        public FD(bool isBE, bool isLongVR) : base(isBE, isLongVR)
        {

        }

        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(UInt16[]))
            {
                UInt16[] vals = new UInt16[(data.Length - startIndex) / 2];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    if (isBE)
                    {
                        val = data.ReverseForBigEndian(startIndex, 2);
                        idx = 0;
                    }
                    else
                        idx = startIndex;
                    vals[i++] = BitConverter.ToUInt16(val, idx);
                    startIndex += 2;
                }
                return (T)(object)vals;
            }
            else
                throw new NotSupportedException();

        }

        public override string ToString(byte[] data, int startIndex, string head)
        {
            UInt16[] value = GetValue<UInt16[]>(data, startIndex);
            return value.ToString();
        }
    }

    public class DS : VR
    {
        public DS(bool isBE, bool isLongVR) : base(isBE, isLongVR) {}

        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(double))
            {
                string str = "";
                Console.WriteLine(data.Length);
                if (data.Length != 0)//对象不为空
                {
                    if (data[data.Length - 2] == 0x20)  //去除填充
                        str = Encoding.Default.GetString(data, startIndex, (int)(data.Length - startIndex - 1));
                    else
                        str = Encoding.Default.GetString(data, startIndex, (int)(data.Length - startIndex));
                    double dblVal = double.Parse(str);
                    return (T)(object)dblVal;
                }
                else//对象为空
                    return (T)(object)0.0;
            }
            else
                throw new NotSupportedException();

        }

        public override string ToString(byte[] data, int startIndex, string head)
        {
            double value = GetValue<double>(data, startIndex);
            if (value == 0.0)
                return " ";
            else
                return value.ToString();
        }
    }

    public class FL : VR
    {
        public FL(bool isBE, bool isLongVR) : base(isBE, isLongVR)
        {

        }

        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(UInt16[]))
            {
                UInt16[] vals = new UInt16[(data.Length - startIndex) / 2];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    if (isBE)
                    {
                        val = data.ReverseForBigEndian(startIndex, 2);
                        idx = 0;
                    }
                    else
                        idx = startIndex;
                    vals[i++] = BitConverter.ToUInt16(val, idx);
                    startIndex += 2;
                }
                return (T)(object)vals;
            }
            else
                throw new NotSupportedException();

        }

        public override string ToString(byte[] data, int startIndex, string head)
        {
            UInt16[] value = GetValue<UInt16[]>(data, startIndex);
            return value.ToString();
        }
    }

    public class SL : VR
    {
        public SL(bool isBE, bool isLongVR) : base(isBE, isLongVR)
        {

        }

        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(Int32[]))
            {
                Int32[] vals = new Int32[(data.Length - startIndex) / 2];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    if (isBE)
                    {
                        val = data.ReverseForBigEndian(startIndex, 2);
                        idx = 0;
                    }
                    else
                        idx = startIndex;
                    vals[i++] = BitConverter.ToInt32(val, idx);
                    startIndex += 2;
                }
                return (T)(object)vals;
            }
            else
                throw new NotSupportedException();

        }

        public override string ToString(byte[] data, int startIndex, string head)
        {
            Int32[] value = GetValue<Int32[]>(data, startIndex);
            return value.ToString();
        }
    }

    public class SS : VR
    {
        public SS(bool isBE, bool isLongVR) : base(isBE, isLongVR)
        {

        }

        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(Int16[]))
            {
                Int16[] vals = new Int16[(data.Length - startIndex) / 2];
                byte[] val = data;
                int idx, i = 0;
                while (startIndex < data.Length)
                {
                    if (isBE)
                    {
                        val = data.ReverseForBigEndian(startIndex, 2);
                        idx = 0;
                    }
                    else
                        idx = startIndex;
                    vals[i++] = BitConverter.ToInt16(val, idx);
                    startIndex += 2;
                }
                return (T)(object)vals;
            }
            else
                throw new NotSupportedException();

        }

        public override string ToString(byte[] data, int startIndex, string head)
        {
            UInt16[] value = GetValue<UInt16[]>(data, startIndex);
            return value.ToString();
        }
    }

    public class US : VR
    {
        public US(bool isBE, bool isLongVR) : base(isBE, isLongVR)
        {

        }

        public override T GetValue<T>(byte[] data, int startIndex)
        {
            if (typeof(T) == typeof(UInt16))
            {
                byte[] val = data;
                int idx;
                if (isBE)
                {
                    val = data.ReverseForBigEndian(startIndex, 4);
                    idx = 0;
                }
                else
                    idx = startIndex;
                return (T)(object)BitConverter.ToUInt16(val, idx);
            }
            else
                throw new NotSupportedException();
        }
        public override string ToString(byte[] data, int startIndex, string head)
        {
            UInt16 value = GetValue<UInt16>(data, startIndex);
            return value.ToString();
        }
    }

   

    public class VRFactory
    {
        protected bool isBE;
        //定义一个Hashtable用于存储享元对象，实现享元池
        private Hashtable VRs = new Hashtable();

        public VRFactory(bool isBE)
        {
            this.isBE = isBE;
        }

        public VR GetVR(string key)
        {
            //如果对象存在，则直接从享元池获取
            if (VRs.ContainsKey(key))
            {
                return (VR)VRs[key];
            }
            //如果对象不存在，先创建一个新的对象添加到享元池中，然后返回
            else
            {
                VR fw = null;
                switch (key)
                {
                    //整型
                    case "SS": fw = new SS(isBE, false); break;
                    case "SL": fw = new SL(isBE, false); break;
                    case "US": fw = new US(isBE, false); break;                 
                    case "UL": fw = new UL(isBE, false); break;
                     //浮点型
                    case "FL": fw = new FL(isBE, false); break;
                    case "FD": fw = new FD(isBE, false); break;
                    case "DS": fw = new DS(isBE, false); break;
                    //日期型
                    case "DA": fw = new DA(isBE, false); break;
                    case "TM": fw = new TM(isBE, false); break;
                    case "DT": fw = new DT(isBE, false); break;                   
                    //数组
                    case "OB": fw = new OB(isBE, true); break;
                    case "OF": fw = new OF(isBE, true); break;
                    case "OW": fw = new OW(isBE, true); break;
                    //文本型
                    case "LO": fw = new LO(isBE, true); break;
                    case "ST": fw = new LO(isBE, true); break;
                    //其他
                    //1.特殊文本
                    case "AS": fw = new AS(isBE, false); break;
                    case "UI": fw = new UI(isBE, false); break;
                    //2.SQ序列
                    case "SQ": fw = new SQ(isBE, true); break;
                    case "UT": fw = new UT(isBE, true); break;
                    case "UN": fw = new UN(isBE, true); break;
                    //default for text
                    default: fw = new ST(isBE, false); break;
                }
                VRs.Add(key, fw);
                return fw;
            }

        }
    }
}