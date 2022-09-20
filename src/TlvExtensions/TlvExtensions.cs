using GON.Datasructures.Tlv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GON.Extensions
{
    public static class TlvExtensions
    {
        public static int GetTlvLenBytesLen(this IEnumerable<byte> bt, int offset)
        {
            if ((bt.Skip(offset).First() & 0xF0) == 0x80)
                return 1 + (bt.Skip(offset).First() & 0x0F);
            return 1;
        }

        public static int DecodeTlvLen(this IEnumerable<byte> bt, int offset)
        {
            var len = bt.GetTlvLenBytesLen(offset);
            if (len == 1)
                return bt.Skip(offset).First();

            return BitConverter.ToInt32(bt.Skip(offset + 1).Take(len - 1).Reverse().ToArray().PadRight(4), 0);
        }

        public static bool IsItTag(this string tag)
        {
            if (tag.Length < 2)
                return false;

            if (!tag.IsHexString())
                return false;

            if (tag.Length == 2 && tag.IsSingleByteTag())
                return true;

            if (!tag.IsSingleByteTag())
                return tag.bin().Skip(1).SkipWhile(x => !x.IsLastTagByte()).Count() == 1;

            return false;
        }

        public static IEnumerable<byte[]> SplitToTlvArrays(this IEnumerable<byte> arr)
        {
            int skip = 0;

            while (arr.Count() > skip)
            {
                int stage = 0;
                int counter = 0;
                int len = 0;
                var ret = arr.Skip(skip).TakeWhile(bt =>
                {
                    if (stage == 0) //Take Tag bytes
                    {
                        if (bt.IsSingleByteTag())
                        {
                            stage += 2;
                            return true;
                        }
                        stage++;
                        return true;
                    }
                    else if (stage == 1) //Take Tag bytes
                    {
                        if (!bt.IsLastTagByte())
                        {
                            return true;
                        }
                        stage++;
                        return true;
                    }
                    else if (stage == 2) //Take Len bytes
                    {
                        if ((bt & 0xF0) == 0x80)
                        {
                            counter = (bt & 0x0F);
                            len = 0;
                            stage++;
                        }
                        else
                        {
                            len = bt + 1;
                            stage += 2;
                        }
                        return true;
                    }
                    else if (stage == 3)
                    {
                        len = (len << 8) + bt;
                        counter--;
                        if (counter == 0)
                        {
                            len++;
                            stage++;
                        }
                        return true;
                    }

                    //Take Value bytes
                    len--;
                    if (len == 0)
                        return false;
                    return true;
                }).ToArray();
                skip += ret.Count();

                yield return ret;
            }
        }

        public static IEnumerable<string> SplitToTlvStrings(this string str) => str.bin().SplitToTlvArrays().Select(x => x.hex());

        public static IEnumerable<byte> GetTlvTagBytes(this IEnumerable<byte> arr)
        {
            int stage = 0;
            return arr.TakeWhile(bt =>
            {
                if (stage == 0) //Take Tag bytes
                {
                    if (bt.IsSingleByteTag())
                    {
                        stage += 2;
                        return true;
                    }
                    stage++;
                    return true;
                }
                else if (stage == 1)
                {
                    if (!bt.IsLastTagByte())
                        return true;
                    stage++;
                    return true;
                }
                return false;
            }).ToArray();
        }

        public static IEnumerable<byte> GetTlvLenBytes(this IEnumerable<byte> arr)
        {
            int stage = 1;
            int counter = 0;
            return arr.Skip(arr.GetTlvTagBytes().Count()).TakeWhile(bt =>
            {
                if (stage == 1)
                {
                    if ((bt & 0xF0) == 0x80)
                    {
                        counter = (bt & 0x0F);
                        stage++;
                    }
                    else
                        stage += 2;
                    return true;
                }
                else if (stage == 2)
                {
                    counter--;
                    if (counter == 0)
                        stage++;
                    return true;
                }
                return false;
            });

        }

        public static IEnumerable<byte> GetTlvValBytes(this IEnumerable<byte> arr)
        {
            var len = arr.GetTlvLenBytes().ToArray();
            return arr.Skip(arr.GetTlvTagBytes().Count() + len.Count()).Take(len.DecodeTlvLen(0));
        }

        public static string GetTlvTagHex(this string str) => str.bin().GetTlvTagBytes().hex();
        public static string GetTlvLenHex(this string str) => str.bin().GetTlvLenBytes().hex();
        public static string GetTlvValHex(this string str) => str.bin().GetTlvValBytes().hex();

        public static byte[] ToTlvLen(this int len)
        {
            var val = BitConverter.GetBytes(len).Reverse().SkipWhile(x => x == 0).ToArray();
            if (val.Length > 1)
            {
                return AddTlvLenCounter(val);
            }
            else if (val.Length == 1)
            {
                return val[0] > 0x7F ? AddTlvLenCounter(val) : val;
            }
            return new byte[] { 0 };
        }

        private static byte[] AddTlvLenCounter(byte[] val)
        {
            var ret = new byte[val.Length + 1];
            ret[0] = (byte)(0x80 + val.Length);
            Buffer.BlockCopy(val, 0, ret, 1, val.Length);
            return ret;
        }

        public static byte[] ToSimpleTlvLen(this int len)
        {
            if (len > 65535)
                throw new ArgumentException("Value should be less than 65536");

            if (len < 0xFF)
                return new byte[] { (byte)len };

            var val = BitConverter.GetBytes(len).Reverse().SkipWhile(x => x == 0).ToArray();
            var ret = new byte[] { 0xFF, 0x00, 0x00 };
            if (val.Length == 2)
            {
                ret[1] = val[0];
                ret[2] = val[1];
            }
            else if (val.Length == 1)
            {
                ret[2] = val[0];
            }
            return ret;
        }

        public static byte[] EMVPadding(this byte[] pBuf, int nLen)
        {
            byte[] ret = new byte[nLen];
            Buffer.BlockCopy(pBuf, 0, ret, 0, pBuf.Length);
            for (int i = pBuf.Length; i < nLen; i++)
            {
                if (i == pBuf.Length) ret[i] = 0x80;
                else ret[i] = 0x00;
            }
            return ret;
        }
        public static byte[] EMVPadding(this byte[] pBuf) => EMVPadding(pBuf, (((int)(pBuf.Length / 8)) + 1) * 8);

        public static byte[] WrapTlv(this byte[] val, byte[] tag) => tag.Combine(new byte[][] { val.Length.ToTlvLen(), val });
        public static byte[] WrapTlv(this byte[] val, string tag) => val.WrapTlv(tag.bin());
        public static string WrapTlv(this string val, string tag) => val.bin().WrapTlv(tag).hex();
        public static string WrapTlv(this string val, byte[] tag) => val.bin().WrapTlv(tag).hex();

        public static IEnumerable<IEnumerable<byte>> WrapApdu(this byte[] val, byte[] command)
        {
            var chunks = val.SplitToChunks(0xFF);
            foreach (var item in chunks)
            {
                var chunk = item.ToArray();
                yield return command.Combine(new byte[][] { new byte[] { (byte)chunk.Length }, chunk });
            }
        }
        public static IEnumerable<IEnumerable<byte>> WrapApdu(this byte[] val, string command) => val.WrapApdu(command.bin());
        public static IEnumerable<string> WrapApdu(this string val, string command) => val.bin().WrapApdu(command.bin()).Select(x => x.hex());
        public static IEnumerable<string> WrapApdu(this string val, byte[] command) => val.bin().WrapApdu(command).Select(x => x.hex());

        public static IEnumerable<byte> FindTag(this IEnumerable<byte> buf, IEnumerable<byte> cmp)
        {
            var tlvs = buf.SplitToTlvArrays();
            foreach (var tlv in tlvs)
            {
                if (tlv.GetTlvTagBytes().Compare(cmp))
                {
                    return tlv;
                }
            }
            return null;
        }
        public static string FindTag(this string buf, string cmp) => buf.bin().FindTag(cmp.bin())?.hex();

        public static byte[] WrapSimpleTlv(this byte[] val, byte[] tag) => tag.Combine(new byte[][] { val.Length.ToSimpleTlvLen(), val });
        public static byte[] WrapSimpleTlv(this byte[] val, string tag) => val.WrapSimpleTlv(tag.bin());
        public static string WrapSimpleTlv(this string val, string tag) => val.bin().WrapSimpleTlv(tag).hex();
        public static string WrapSimpleTlv(this string val, byte[] tag) => val.bin().WrapSimpleTlv(tag).hex();

        public static IDictionary<string, string> ToTlvDictionary(this string str) => str.SplitToTlvStrings().ToDictionary<string, string>(k => k.GetTlvTagHex());
        public static IDictionary<string, byte[]> ToTlvDictionary(this IEnumerable<byte> buf) => buf.SplitToTlvArrays().ToDictionary(k => k.GetTlvTagBytes().hex());


        //The value 00 indicates a data object of the universal class.
        //The value 01 indicates a data object of the application class.
        //The value 10 indicates a data object of the context-specific class.
        //The value 11 indicates a data object of the private class.
        public static BerTlvTagClass GetTagClass(this byte bt) => (BerTlvTagClass)(bt >> 6);
        public static BerTlvTagClass GetTagClass(this IEnumerable<byte> buf) => buf.First().GetTagClass();
        public static BerTlvTagClass GetTagClass(this string str) => str.Substring(0, 2).bin().GetTagClass();

        //Bit 6 of the first byte of the tag field indicates an encoding.
        //The value 0 indicates a primitive encoding of the data object, i.e., the value field is not encoded in BER-TLV.
        //The value 1 indicates a constructed encoding of the data object, i.e., the value field is encoded in BER-TLV.
        public static bool IsTlvDataConstructed(this byte bt) => (bt & 0x20) == 0x20;
        public static bool IsTlvDataConstructed(this IEnumerable<byte> buf) => buf.First().IsTlvDataConstructed();
        public static bool IsTlvDataConstructed(this string str) => str.Substring(0, 2).bin().IsTlvDataConstructed();
        public static bool IsTlvDataPrimitive(this byte bt) => !bt.IsTlvDataConstructed();
        public static bool IsTlvDataPrimitive(this IEnumerable<byte> buf) => !buf.IsTlvDataConstructed();
        public static bool IsTlvDataPrimitive(this string str) => !str.IsTlvDataConstructed();

        //If bits 5 to 1 of the first byte of the tag field are not all set to 1, then they encode a tag number from zero to thirty and the tag field consists of a single byte.
        public static bool IsSingleByteTag(this byte bt) => (bt & 0x1F) != 0x1F;
        public static bool IsSingleByteTag(this IEnumerable<byte> buf) => buf.First().IsSingleByteTag();
        public static bool IsSingleByteTag(this string str) => str.Substring(0, 2).bin().IsSingleByteTag();

        //Bit 8 of each subsequent byte shall be set to 1, unless it is the last subsequent byte.
        //Bits 7 to 1 of the first subsequent byte shall not be all set to 0.
        //Bits 7 to 1 of the first subsequent byte, followed by bits 7 to 1 of each further subsequent byte, up to and including bits 7 to 1 of the last subsequent byte encode a tag number.
        public static bool IsLastTagByte(this byte bt) => (bt >> 7) == 0;

    }
}
