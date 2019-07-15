using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMLib
{
    public abstract class DCMAbstractType
    {
        /// <summary>
        /// 元素号
        /// </summary>
        public ushort gtag;
        /// <summary>
        /// 组号
        /// </summary>
        public ushort etag;
        /// <summary>
        /// 数据元素名
        /// </summary>
        public string name;
        /// <summary>
        /// 值表示法
        /// </summary>
        public string vr;
        /// <summary>
        /// 多值
        /// </summary>
        public string vm;
        /// <summary>
        /// 值长度
        /// </summary>
        public uint length;
        /// <summary>
        /// 值
        /// </summary>
        public object value;
        public VR vrparser;

        public uint UIntTag
        {
            get
            {
                return (uint)(gtag << 16) + etag;
            }


        }

        /// <summary>
        /// 获取解码后的字符串
        /// </summary>
        /// <param name="head">添加在左</param>
        public abstract string ToString(string head);

        public virtual T GetValue<T>()//文件解析用
        {
            return vrparser.GetValue<T>((byte[])value, 0);//去掉了length
        }
    }

    public class DCMDataElement : DCMAbstractType
    {
        public override string ToString(string head)
        {
            string str = head;
            str += gtag.ToString("X4") + "," + etag.ToString("X4") + "\t";
            str += vr + "\t";
            str += name + "\t";
            if (length == 0xffffffff)
                str += "Undefined\n";
            else
                str += length.ToString() + "\t";
            if (vr == "SQ")
                str += ((DCMDataSequence)value).ToString(head + ">");
            else
                str += vrparser.ToString((byte[])value, 0, head);
            return str;

        }
    }

    public class DCMDataSet : DCMAbstractType
    {
        /// <summary>
        /// 所容纳的数据元素或条目
        /// </summary>
        public List<DCMAbstractType> items;
        public TransferSyntax syn;

        //索引成员, 对items的读写操作通过索引进行
        public DCMAbstractType this[uint tag]
        {
            get
            {
                return items.Find(elem => elem.UIntTag == tag); //查找对应元素
            }
            set
            {
                int idx = items.FindIndex(elem => elem.UIntTag == tag); //查找对应下标
                if (idx != -1)     //找到直接替换
                    items[idx] = value;
                else
                {
                    items.Add(value);              //否则添加
                    items.Sort((left, right) => left.UIntTag.CompareTo(right.UIntTag)); //组号递增元素号递增排序
                }
            }
        }


        public DCMDataSet() { }
        public DCMDataSet(TransferSyntax transfer)
        {
            this.syn = transfer;
        }

        public override string ToString(string head)
        {
            string str = "";
            foreach (DCMAbstractType item in items)
            {
                if (item != null)
                {
                    if (str != "") str += "\n";  //两个数据元素之间用换行符分割
                    str += item.ToString(head);
                }
            }
            return str;

        }

        /// <param name="data">数据流</param>
        /// <param name="idx">位置索引</param>
        public virtual List<DCMAbstractType> Decode(byte[] data, ref uint idx)
        {
            items = new List<DCMAbstractType>();
            while (idx < data.Length)
            {                
                DCMAbstractType item = null;
                //此处调用传输语法对象解码一条数据元素
                item = syn.Decode(data, ref idx);
                //判断特殊标记
                if (item.gtag == 0xfffe && item.etag == 0xe0dd)
                    break;
                if (item.gtag == 0xfffe && item.etag == 0xe00d)
                    break;
                if (item.vr == "SQ")
                {
                    DCMDataSequence sq = new DCMDataSequence(syn);
                    uint ulidx = 0;
                    byte[] val = (byte[])item.value;
                    sq.Decode(val, ref ulidx);
                    item.value = sq;
                    //todo：修正idx位置
                    if (item.length == 0xffffffff)  //修正idx位置
                        idx -= (uint)(val.Length - ulidx);
                }

                Console.WriteLine(idx);
                items.Add(item);             
            }
            return items;

        }
    }

    public class DicomDictionary
    {
        public List<DicomDictionaryEntry> store;

        public DicomDictionary(string path)
        {
            //#1.open dic
            FileStream fs = File.OpenRead(path);

            //#2.load dic
            //set container
            string s = "";
            //initial StreamReader
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            fs.Seek(0, SeekOrigin.Begin);
            //Stream to string
            s += sr.ReadToEnd();
            sr.Close();
            fs.Close();
            string[] sp = s.Split('\n');


            //#3.attach dic
            store = new List<DicomDictionaryEntry>();
            for (int i = 0; i < sp.Length ; i ++)
            {
                DicomDictionaryEntry buffer = new DicomDictionaryEntry();
                string[] cache = sp[i].Split('\t');
                buffer.Tag = cache[0];
                if (cache.Length > 2)
                {
                    buffer.Name = cache[1];
                    buffer.Keyword = cache[2];
                    buffer.VR = cache[3];
                    buffer.VM = cache[4];
                }
                if (cache.Length == 6)
                    buffer.Anno = cache[5];
                else
                    buffer.Anno = "";
                store.Add(buffer);
            }

        }


        public void queryInfo(string tag)
        {
            if (!store.Exists(x => x.Tag == tag))
            {
                Console.WriteLine("Tag不存在！");
            }
            else
            {
                int index = store.FindIndex(x => x.Tag == tag);
                string VR1 = store[index].VR;
                string Name1 = store[index].Name;
                string Anno1 = store[index].Anno;
                string VM1 = store[index].VM;
                string Keyword1 = store[index].Keyword;
                Console.WriteLine("QueryTag:{4},  Name:{0},  Keyword:{1},  VR:{2},  VM:{3},  Anno:{5}", Name1, Keyword1, VR1, VM1, tag,Anno1);
            }
        }

        public void printDic()
        {
            foreach (DicomDictionaryEntry de in store)
                Console.WriteLine("{0}", de.Tag);
        }

        public DicomDictionaryEntry GetEntry(ushort gtag, ushort etag)
        {
            string tag = '('+gtag.ToString("X4") + ','+ etag.ToString("X4") + ')';
            Console.WriteLine(tag);
            if (!store.Exists(x => x.Tag == tag))
            {
                Console.WriteLine("GE_null");
                return null;
            }
            else
            {
                int index = store.FindIndex(x => x.Tag == tag);
                string VR1 = store[index].VR;
                string Name1 = store[index].Name;
                string VM1 = store[index].VM;
                string Keyword1 = store[index].Keyword;
                Console.WriteLine("QueryTag:{4},  Name:{0},  Keyword:{1},  VR:{2},  VM:{3}", Name1, Keyword1, VR1, VM1, tag);
                return store[index];
            }
        }

        public void GetVR()
        {
            throw new System.NotImplementedException();
        }
    }

    public class DicomDictionaryEntry
    {
        public string Tag { get; set; }
        public string Name { get; set; }
        public string Keyword { get; set; }
        public string VR { get; set; }
        public string VM { get; set; }
        public string Anno { get; set; }
    }

    public class DCMDataSequence : DCMDataSet
    {
        public DCMDataSequence(TransferSyntax syn):base(syn)
        {
            this.syn = syn;
        }

        public override List<DCMAbstractType> Decode(byte[] data, ref uint idx)
        {
            items = new List<DCMAbstractType>();
            while (idx < data.Length)
            {
                DCMDataItem item = new DCMDataItem(syn);
                item.Decode(data, ref idx);  //解码一个item，加入items列表
                if (item.items!=null && item.items.Count > 0)
                    items.Add(item);
                else
                    break;
            }
            return items;
        }

        public override string ToString(string head)
        {
            string str = "";
            int i = 1;
            foreach (DCMAbstractType item in items)
            {
                if (item != null)
                {
                    str +="\n" +">";  //两个数据元素之间用换行符分割
                    str += "ITEM" + i++ +"\n";
                    str += item.ToString(">");
                }
            }
            return str;

        }
    }

    public class DCMDataItem : DCMDataSet
    {
        public DCMDataItem(TransferSyntax syn):base(syn)
        {
            this.syn = syn;
        }

        public override List<DCMAbstractType> Decode(byte[] data, ref uint idx)
        {
            DCMAbstractType item = syn.Decode(data, ref idx);
            if (item.gtag == 0xfffe && item.etag == 0xe000)  //item start
            {
                uint ulidx = 0;
                byte[] val = (byte[])item.value;
                base.Decode(val, ref ulidx);
                //todo：修正idx位置
                if (item.length == 0xffffffff)  //修正idx位置
                    idx -= (uint)(val.Length - ulidx);
            }
            return base.items;
        }

        public override string ToString(string head)
        {
            string str = "";
            foreach (DCMAbstractType item in items)
            {
                if (item != null)
                {
                    if (str != "") str += "\n";  //两个数据元素之间用换行符分割
                    str += item.ToString(head);
                }
            }
            return str;

        }
    }

    public class DCMFileMeta : DCMDataSet
    {
        public DCMFileMeta(TransferSyntax transfer)
        {
            this.syn = transfer;
        }

        public override List<DCMAbstractType> Decode(byte[] data, ref uint idx)
        {
            DCMAbstractType elem = syn.Decode(data, ref idx);
            if (elem.gtag == 0x0002 && elem.etag == 0x0000)
            {
                uint ulidx = 0;
                int glen = (int)elem.GetValue<UInt32>();
                byte[] val = new byte[glen];
                Array.Copy(data, idx, val, 0, glen);
                base.Decode(val, ref ulidx);
                idx += ulidx;
            }
            return items;
        }
    }

    public class DCMFile : DCMDataSet
    {
        protected string filename;
        public DCMFileMeta filemeta;
        public DCMFile() : base(new ImplicitVRLittleEndian())
        {
        }

        public override List<DCMAbstractType> Decode(byte[] data, ref uint idx)
        {
            //a)	用ExplicitVRLittleEndian对象实例化filemeta对象，通过其Decode方法从data中读取头元素
            filemeta = new DCMFileMeta(new ExplicitVRLittleEndian());
            filemeta.Decode(data, ref idx);
            //b)	读取(0002,0010)头元素
            DCMAbstractType head = filemeta.items.Find(x => ((x.gtag == 0x0002) && (x.etag == 0x0010)));
            //得到数据集传输语法的uid，
            String uid = head.GetValue<string>();
            //在TransferSyntaxes中找到对应的传输语法对象赋给基类的syn字段
            base.syn = TransferSyntaxs.All[uid];
            //c)	调用base.Decode方法解码数据集。
            return base.Decode(data, ref idx);
        }

        public List<DCMAbstractType> Decode(string fname)
        {
            //a)	保存文件名fname到filename
            filename = fname;
            //b)	打开filename文件，读取文件内容到byte数组data
            FileStream fs = File.OpenRead(filename);
            BinaryReader br = new BinaryReader(fs,Encoding.Default);
            //c)	跳过128字节前导符(idx = 128)
            fs.Seek(128, SeekOrigin.Begin);
            uint idx = 0;
            //d)	读取4字节的”DICM”，判断是否为dicom文件,为否就退出
            string trigger = Encoding.Default.GetString(br.ReadBytes(4));
            if (trigger != "DICM")
            {
                throw new NotSupportedException();
            }
            byte[] data = new byte[fs.Length-132];
            int i = 0;
            while (br.PeekChar() > -1)
                data[i++] = br.ReadByte() ;
            //e)	调用byte数组参数的Decode方法            
            return this.Decode(data, ref idx);
        }

        public override string ToString(string head)
        {
            //先调用filemeta.ToString方法           
            //然后调用base.ToString方法，将两次调用得到的字符串拼接起来。
            return filemeta.ToString() + base.ToString(head);
        }
    }
}
