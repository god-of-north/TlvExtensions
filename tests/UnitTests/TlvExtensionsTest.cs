using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GON.Extensions;
using System.Collections.Generic;
using System.Linq;
using GON.Datasructures.Tlv;

namespace UnitTests
{
    [TestClass]
    public class TlvExtensionsTest
    {
        [TestMethod]
        public void GetTlvLenBytesLen_WithZeroOffset_ReturnsLength()
        {
            //Arrange
            var data1 = "10".bin();
            var data2 = "7F".bin();
            var data3 = "81FF".bin();
            var data4 = "8201FF".bin();
            var expected1 = 1;
            var expected2 = 1;
            var expected3 = 2;
            var expected4 = 3;

            //Act
            var res1 = data1.GetTlvLenBytesLen(0);
            var res2 = data2.GetTlvLenBytesLen(0);
            var res3 = data3.GetTlvLenBytesLen(0);
            var res4 = data4.GetTlvLenBytesLen(0);

            //Assert
            Assert.AreEqual(expected1, res1);
            Assert.AreEqual(expected2, res2);
            Assert.AreEqual(expected3, res3);
            Assert.AreEqual(expected4, res4);
        }

        [TestMethod]
        public void GetTlvLenBytesLen_WithOffset_ReturnsLength()
        {
            //Arrange
            var data1 = "000010".bin();
            var data2 = "007F".bin();
            var data3 = "00000081FF".bin();
            var data4 = "008201FF".bin();
            var expected1 = 1;
            var expected2 = 1;
            var expected3 = 2;
            var expected4 = 3;

            //Act
            var res1 = data1.GetTlvLenBytesLen(2);
            var res2 = data2.GetTlvLenBytesLen(1);
            var res3 = data3.GetTlvLenBytesLen(3);
            var res4 = data4.GetTlvLenBytesLen(1);

            //Assert
            Assert.AreEqual(expected1, res1);
            Assert.AreEqual(expected2, res2);
            Assert.AreEqual(expected3, res3);
            Assert.AreEqual(expected4, res4);
        }

        [TestMethod]
        public void DecodeTlvLen_WithotOffset_ReturnsLength()
        {
            //Arrange
            var data1 = "10".bin();
            var data2 = "7F".bin();
            var data3 = "81FF".bin();
            var data4 = "8201FF".bin();
            var expected1 = 0x10;
            var expected2 = 0x7F;
            var expected3 = 0xFF;
            var expected4 = 0x1FF;

            //Act
            var res1 = data1.DecodeTlvLen(0);
            var res2 = data2.DecodeTlvLen(0);
            var res3 = data3.DecodeTlvLen(0);
            var res4 = data4.DecodeTlvLen(0);

            //Assert
            Assert.AreEqual(expected1, res1);
            Assert.AreEqual(expected2, res2);
            Assert.AreEqual(expected3, res3);
            Assert.AreEqual(expected4, res4);
        }

        [TestMethod]
        public void DecodeTlvLen_WithOffset_ReturnsLength()
        {
            //Arrange
            var data1 = "0010".bin();
            var data2 = "00007F".bin();
            var data3 = "00000081FF".bin();
            var data4 = "008201FF".bin();
            var expected1 = 0x10;
            var expected2 = 0x7F;
            var expected3 = 0xFF;
            var expected4 = 0x1FF;

            //Act
            var res1 = data1.DecodeTlvLen(1);
            var res2 = data2.DecodeTlvLen(2);
            var res3 = data3.DecodeTlvLen(3);
            var res4 = data4.DecodeTlvLen(1);

            //Assert
            Assert.AreEqual(expected1, res1);
            Assert.AreEqual(expected2, res2);
            Assert.AreEqual(expected3, res3);
            Assert.AreEqual(expected4, res4);
        }

        [TestMethod]
        public void IsItTag_WithNonTag_ReturnsFalse()
        {
            //Arrange
            var data1 = "  ";
            var data2 = "Hello World";
            var data3 = "9F";
            var data4 = "BF81";

            //Act
            var res1 = data1.IsItTag();
            var res2 = data2.IsItTag();
            var res3 = data3.IsItTag();
            var res4 = data4.IsItTag();

            //Assert
            Assert.IsFalse(res1);
            Assert.IsFalse(res2);
            Assert.IsFalse(res3);
            Assert.IsFalse(res4);
        }

        [TestMethod]
        public void IsItTag_WithOneByteTag_ReturnsTrue()
        {
            //Arrange
            var data1 = "11";
            var data2 = "70";
            var data3 = "5A";
            var data4 = "8F";

            //Act
            var res1 = data1.IsItTag();
            var res2 = data2.IsItTag();
            var res3 = data3.IsItTag();
            var res4 = data4.IsItTag();

            //Assert
            Assert.IsTrue(res1);
            Assert.IsTrue(res2);
            Assert.IsTrue(res3);
            Assert.IsTrue(res4);
        }

        [TestMethod]
        public void IsItTag_WithTwoByteTag_ReturnsTrue()
        {
            //Arrange
            var data1 = "9F10";
            var data2 = "BF0C";
            var data3 = "3F33";
            var data4 = "5f20";

            //Act
            var res1 = data1.IsItTag();
            var res2 = data2.IsItTag();
            var res3 = data3.IsItTag();
            var res4 = data4.IsItTag();

            //Assert
            Assert.IsTrue(res1);
            Assert.IsTrue(res2);
            Assert.IsTrue(res3);
            Assert.IsTrue(res4);
        }

        [TestMethod]
        public void IsItTag_WithThreeByteTag_ReturnsTrue()
        {
            //Arrange
            var data1 = "9F8110";
            var data2 = "BF820C";
            var data3 = "3F8333";
            var data4 = "5f8F20";

            //Act
            var res1 = data1.IsItTag();
            var res2 = data2.IsItTag();
            var res3 = data3.IsItTag();
            var res4 = data4.IsItTag();

            //Assert
            Assert.IsTrue(res1);
            Assert.IsTrue(res2);
            Assert.IsTrue(res3);
            Assert.IsTrue(res4);
        }

        [TestMethod]
        public void SplitToTlvArrays_WithByteArray_ReturnsSplittedByteArray()
        {
            //Arrange
            var data = "9f1003a1a2a3 5A055152535455 9f4800 9F448101FF 3F8182835501BB 8F03AAAAAA".bin();
            var expected = new List<byte[]>() { "9f1003a1a2a3".bin(), "5A055152535455".bin(), "9f4800".bin(), "9F448101FF".bin(), "3F8182835501BB".bin(), "8F03AAAAAA".bin() };

            //Act
            var res = data.SplitToTlvArrays();

            //Assert
            Assert.IsInstanceOfType(res, typeof(IEnumerable<byte[]>));
            var i = expected.GetEnumerator();
            foreach (var item in res)
            {
                i.MoveNext();
                Assert.IsTrue(item.SequenceEqual(i.Current));
            }
        }

        [TestMethod]
        public void SplitToTlvStrings_WithString_ReturnsSplittedString()
        {
            //Arrange
            var data = "9f1003a1a2a3 5A055152535455 9f4800 9F448101FF 3F8182835501BB 8F03AAAAAA";
            var expected = new List<string>() { "9F1003A1A2A3", "5A055152535455", "9F4800", "9F448101FF", "3F8182835501BB", "8F03AAAAAA" };

            //Act
            var res = data.SplitToTlvStrings();

            //Assert
            Assert.IsInstanceOfType(res, typeof(IEnumerable<string>));
            var i = expected.GetEnumerator();
            foreach (var item in res)
            {
                i.MoveNext();
                Assert.AreEqual(item, i.Current);
            }
        }

        [TestMethod]
        public void GetTlvTagBytes_WithTLVEncodedArray_ReturnsTag()
        {
            //Arrange
            var data1 = "9f1003a1a2a3".bin();
            var data2 = "5A055152535455".bin();
            var data3 = "9f4800".bin();
            var data4 = "9F448101FF".bin();
            var data5 = "3F8182835501BB".bin();
            var data6 = "8F03AAAAAA".bin();
            var expected1 = "9f10".bin();
            var expected2 = "5A".bin();
            var expected3 = "9f48".bin();
            var expected4 = "9F44".bin();
            var expected5 = "3F81828355".bin();
            var expected6 = "8F".bin();

            //Act
            var res1 = data1.GetTlvTagBytes();
            var res2 = data2.GetTlvTagBytes();
            var res3 = data3.GetTlvTagBytes();
            var res4 = data4.GetTlvTagBytes();
            var res5 = data5.GetTlvTagBytes();
            var res6 = data6.GetTlvTagBytes();

            //Assert
            Assert.IsTrue(expected1.SequenceEqual(res1));
            Assert.IsTrue(expected2.SequenceEqual(res2));
            Assert.IsTrue(expected3.SequenceEqual(res3));
            Assert.IsTrue(expected4.SequenceEqual(res4));
            Assert.IsTrue(expected5.SequenceEqual(res5));
            Assert.IsTrue(expected6.SequenceEqual(res6));
        }

        [TestMethod]
        public void GetTlvLenBytes_WithTLVEncodedArray_ReturnsLength()
        {
            //Arrange
            var data1 = "9f1003a1a2a3".bin();
            var data2 = "5A055152535455".bin();
            var data3 = "9f4800".bin();
            var data4 = "9F448101FF".bin();
            var data5 = "3F8182835501BB".bin();
            var data6 = "8F03AAAAAA".bin();
            var expected1 = "03".bin();
            var expected2 = "05".bin();
            var expected3 = "00".bin();
            var expected4 = "8101".bin();
            var expected5 = "01".bin();
            var expected6 = "03".bin();

            //Act
            var res1 = data1.GetTlvLenBytes();
            var res2 = data2.GetTlvLenBytes();
            var res3 = data3.GetTlvLenBytes();
            var res4 = data4.GetTlvLenBytes();
            var res5 = data5.GetTlvLenBytes();
            var res6 = data6.GetTlvLenBytes();

            //Assert
            Assert.IsTrue(expected1.SequenceEqual(res1));
            Assert.IsTrue(expected2.SequenceEqual(res2));
            Assert.IsTrue(expected3.SequenceEqual(res3));
            Assert.IsTrue(expected4.SequenceEqual(res4));
            Assert.IsTrue(expected5.SequenceEqual(res5));
            Assert.IsTrue(expected6.SequenceEqual(res6));
        }

        [TestMethod]
        public void GetTlvValBytes_WithTLVEncodedArray_ReturnsValue()
        {
            //Arrange
            var data1 = "9f1003a1a2a3".bin();
            var data2 = "5A055152535455".bin();
            var data3 = "9f4800".bin();
            var data4 = "9F448101FF".bin();
            var data5 = "3F8182835501BB".bin();
            var data6 = "8F03AAAAAA".bin();
            var expected1 = "a1a2a3".bin();
            var expected2 = "5152535455".bin();
            var expected3 = "".bin();
            var expected4 = "FF".bin();
            var expected5 = "BB".bin();
            var expected6 = "AAAAAA".bin();

            //Act
            var res1 = data1.GetTlvValBytes();
            var res2 = data2.GetTlvValBytes();
            var res3 = data3.GetTlvValBytes();
            var res4 = data4.GetTlvValBytes();
            var res5 = data5.GetTlvValBytes();
            var res6 = data6.GetTlvValBytes();

            //Assert
            Assert.IsTrue(expected1.SequenceEqual(res1));
            Assert.IsTrue(expected2.SequenceEqual(res2));
            Assert.IsTrue(expected3.SequenceEqual(res3));
            Assert.IsTrue(expected4.SequenceEqual(res4));
            Assert.IsTrue(expected5.SequenceEqual(res5));
            Assert.IsTrue(expected6.SequenceEqual(res6));
        }

        [TestMethod]
        public void GetTlvTagHex_WithTLVEncodedHexString_ReturnsTag()
        {
            var data1 = "9f1003a1a2a3";
            var data2 = "5A055152535455";
            var data3 = "9f4800";
            var data4 = "9F448101FF";
            var data5 = "3F8182835501BB";
            var data6 = "8F03AAAAAA";
            var expected1 = "9F10";
            var expected2 = "5A";
            var expected3 = "9F48";
            var expected4 = "9F44";
            var expected5 = "3F81828355";
            var expected6 = "8F";

            //Act
            var res1 = data1.GetTlvTagHex();
            var res2 = data2.GetTlvTagHex();
            var res3 = data3.GetTlvTagHex();
            var res4 = data4.GetTlvTagHex();
            var res5 = data5.GetTlvTagHex();
            var res6 = data6.GetTlvTagHex();

            //Assert
            Assert.AreEqual(expected1, res1);
            Assert.AreEqual(expected2, res2);
            Assert.AreEqual(expected3, res3);
            Assert.AreEqual(expected4, res4);
            Assert.AreEqual(expected5, res5);
            Assert.AreEqual(expected6, res6);
        }

        [TestMethod]
        public void GetTlvLenHex_WithTLVEncodedHexString_ReturnsLength()
        {
            //Arrange
            var data1 = "9f1003a1a2a3";
            var data2 = "5A055152535455";
            var data3 = "9f4800";
            var data4 = "9F448101FF";
            var data5 = "3F8182835501BB";
            var data6 = "8F03AAAAAA";
            var expected1 = "03";
            var expected2 = "05";
            var expected3 = "00";
            var expected4 = "8101";
            var expected5 = "01";
            var expected6 = "03";

            //Act
            var res1 = data1.GetTlvLenHex();
            var res2 = data2.GetTlvLenHex();
            var res3 = data3.GetTlvLenHex();
            var res4 = data4.GetTlvLenHex();
            var res5 = data5.GetTlvLenHex();
            var res6 = data6.GetTlvLenHex();

            //Assert
            Assert.AreEqual(expected1, res1);
            Assert.AreEqual(expected2, res2);
            Assert.AreEqual(expected3, res3);
            Assert.AreEqual(expected4, res4);
            Assert.AreEqual(expected5, res5);
            Assert.AreEqual(expected6, res6);
        }

        [TestMethod]
        public void GetTlvValHex_WithTLVEncodedHexString_ReturnsValue()
        {
            //Arrange
            var data1 = "9f1003a1a2a3";
            var data2 = "5A055152535455";
            var data3 = "9f4800";
            var data4 = "9F448101FF";
            var data5 = "3F8182835501BB";
            var data6 = "8F03AAAAAA";
            var expected1 = "A1A2A3";
            var expected2 = "5152535455";
            var expected3 = "";
            var expected4 = "FF";
            var expected5 = "BB";
            var expected6 = "AAAAAA";

            //Act
            var res1 = data1.GetTlvValHex();
            var res2 = data2.GetTlvValHex();
            var res3 = data3.GetTlvValHex();
            var res4 = data4.GetTlvValHex();
            var res5 = data5.GetTlvValHex();
            var res6 = data6.GetTlvValHex();

            //Assert
            Assert.AreEqual(expected1, res1);
            Assert.AreEqual(expected2, res2);
            Assert.AreEqual(expected3, res3);
            Assert.AreEqual(expected4, res4);
            Assert.AreEqual(expected5, res5);
            Assert.AreEqual(expected6, res6);
        }

        [TestMethod]
        public void ToTlvLen_WithShortLength_ReturnsOneByteLength()
        {
            //Arrange
            var data1 = 0x10;
            var data2 = 0x7F;
            var data3 = 0x01;
            var data4 = 0x66;
            var expected1 = "10".bin();
            var expected2 = "7F".bin();
            var expected3 = "01".bin();
            var expected4 = "66".bin();

            //Act
            var res1 = data1.ToTlvLen();
            var res2 = data2.ToTlvLen();
            var res3 = data3.ToTlvLen();
            var res4 = data4.ToTlvLen();

            //Assert
            Assert.IsTrue(expected1.SequenceEqual(res1));
            Assert.IsTrue(expected2.SequenceEqual(res2));
            Assert.IsTrue(expected3.SequenceEqual(res3));
            Assert.IsTrue(expected4.SequenceEqual(res4));
        }

        [TestMethod]
        public void ToTlvLen_WithLongLength_ReturnsTwoByteLength()
        {
            //Arrange
            var data1 = 0x80;
            var data2 = 0xFF;
            var data3 = 0x100;
            var data4 = 0x504000;
            var expected1 = "8180".bin();
            var expected2 = "81FF".bin();
            var expected3 = "820100".bin();
            var expected4 = "83504000".bin();

            //Act
            var res1 = data1.ToTlvLen();
            var res2 = data2.ToTlvLen();
            var res3 = data3.ToTlvLen();
            var res4 = data4.ToTlvLen();

            //Assert
            Assert.IsTrue(expected1.SequenceEqual(res1));
            Assert.IsTrue(expected2.SequenceEqual(res2));
            Assert.IsTrue(expected3.SequenceEqual(res3));
            Assert.IsTrue(expected4.SequenceEqual(res4));
        }

        [TestMethod]
        public void ToSimpleTlvLen_WithShortLength_ReturnsOneByteArray()
        {
            //Arrange
            var data1 = 0x10;
            var data2 = 0x7F;
            var data3 = 0x01;
            var data4 = 0x66;
            var expected1 = "10".bin();
            var expected2 = "7F".bin();
            var expected3 = "01".bin();
            var expected4 = "66".bin();

            //Act
            var res1 = data1.ToSimpleTlvLen();
            var res2 = data2.ToSimpleTlvLen();
            var res3 = data3.ToSimpleTlvLen();
            var res4 = data4.ToSimpleTlvLen();

            //Assert
            Assert.IsTrue(expected1.SequenceEqual(res1), res1.hex());
            Assert.IsTrue(expected2.SequenceEqual(res2), res2.hex());
            Assert.IsTrue(expected3.SequenceEqual(res3), res3.hex());
            Assert.IsTrue(expected4.SequenceEqual(res4), res4.hex());
        }

        [TestMethod]
        public void ToSimpleTlvLen_WithLongLength_ReturnsThreeByteArray()
        {
            //Arrange
            var data1 = 0x180;
            var data2 = 0xFF;
            var data3 = 0x100;
            var data4 = 0x5040;
            var expected1 = "FF0180".bin();
            var expected2 = "FF00FF".bin();
            var expected3 = "FF0100".bin();
            var expected4 = "FF5040".bin();

            //Act
            var res1 = data1.ToSimpleTlvLen();
            var res2 = data2.ToSimpleTlvLen();
            var res3 = data3.ToSimpleTlvLen();
            var res4 = data4.ToSimpleTlvLen();

            //Assert
            Assert.IsTrue(expected1.SequenceEqual(res1), res1.hex());
            Assert.IsTrue(expected2.SequenceEqual(res2), res2.hex());
            Assert.IsTrue(expected3.SequenceEqual(res3), res3.hex());
            Assert.IsTrue(expected4.SequenceEqual(res4), res4.hex());
        }

        [TestMethod]
        public void EMVPadding_WithPadSize_ReturnsPaddedArray()
        {
            //Arrange
            var data = "11223344".bin();
            var expected = "1122334480000000".bin();

            //Act
            var res = data.EMVPadding(8);

            //Assert
            Assert.IsTrue(res.SequenceEqual(expected));
        }

        [TestMethod]
        public void EMVPadding_WithArrayMultipleOf8Bytes_ReturnsPaddedArray()
        {
            //Arrange
            var data = "1122334455667788".bin();
            var expected = "11223344556677888000000000000000".bin();

            //Act
            var res = data.EMVPadding();

            //Assert
            Assert.IsTrue(res.SequenceEqual(expected));
        }

        [TestMethod]
        public void EMVPadding_WithArrayNotMultipleOf8Bytes_ReturnsPaddedArray()
        {
            //Arrange
            var data = "11223344".bin();
            var expected = "1122334480000000".bin();

            //Act
            var res = data.EMVPadding();

            //Assert
            Assert.IsTrue(res.SequenceEqual(expected));
        }

        [TestMethod]
        public void WrapTlv_WithByteValByteTag_ReturnsByteArray()
        {
            //Arrange
            var data = "11223344".bin();
            var tag = "9F10".bin();
            var expected = "9F10 04 11223344".bin();

            //Act
            var res = data.WrapTlv(tag);

            //Assert
            Assert.IsInstanceOfType(res, typeof(byte[]));
            Assert.IsTrue(res.SequenceEqual(expected));
        }

        [TestMethod]
        public void WrapTlv_WithByteValStringTag_ReturnsByteArray()
        {
            //Arrange
            var data = "11223344".bin();
            var tag = "9F10";
            var expected = "9F10 04 11223344".bin();

            //Act
            var res = data.WrapTlv(tag);

            //Assert
            Assert.IsInstanceOfType(res, typeof(byte[]));
            Assert.IsTrue(res.SequenceEqual(expected), res.hex());
        }

        [TestMethod]
        public void WrapTlv_WithStringValByteTag_ReturnsString()
        {
            //Arrange
            var data = "11223344";
            var tag = "9F10".bin();
            var expected = "9F100411223344";

            //Act
            var res = data.WrapTlv(tag);

            //Assert
            Assert.IsInstanceOfType(res, typeof(string));
            Assert.AreEqual(expected, res);
        }

        [TestMethod]
        public void WrapTlv_WithStringValStringTag_ReturnsString()
        {
            //Arrange
            var data = "11223344";
            var tag = "9F10";
            var expected = "9F100411223344";

            //Act
            var res = data.WrapTlv(tag);

            //Assert
            Assert.IsInstanceOfType(res, typeof(string));
            Assert.AreEqual(expected, res);
        }

        [TestMethod]
        public void WrapApdu_WithByteValByteCommand_ReturnsByteArray()
        {
            //Arrange
            var data = "A00000005555".bin();
            var apdu = "00A40000".bin();
            var expected = "00A40000 06 A00000005555".bin();

            //Act
            var res = data.WrapApdu(apdu);

            //Assert
            Assert.IsTrue(res.First().SequenceEqual(expected));
        }

        [TestMethod]
        public void WrapApdu_WithByteValStringCommand_ReturnsByteArray()
        {
            //Arrange
            var data = "A00000005555".bin();
            var apdu = "00A40000";
            var expected = "00A40000 06 A00000005555".bin();

            //Act
            var res = data.WrapApdu(apdu);

            //Assert
            Assert.IsTrue(res.First().SequenceEqual(expected));
        }

        [TestMethod]
        public void WrapApdu_WithStringValByteCommand_ReturnsString()
        {
            //Arrange
            var data = "A00000005555";
            var apdu = "00A40000".bin();
            var expected = "00A4000006A00000005555";

            //Act
            var res = data.WrapApdu(apdu);

            //Assert
            Assert.AreEqual(expected, res.First());
        }

        [TestMethod]
        public void WrapApdu_WithStringValStringCommand_ReturnsString()
        {
            //Arrange
            var data = "A00000005555";
            var apdu = "00A40000";
            var expected = "00A4000006A00000005555";

            //Act
            var res = data.WrapApdu(apdu);

            //Assert
            Assert.AreEqual(expected, res.First());
        }

        [TestMethod]
        public void FindTag_WithByteArray_ReturnsByteArray()
        {
            //Arrange
            var data = "9f1003a1a2a3 5A055152535455 9f4800 9F448101FF 3F8182835501BB 8F03AAAAAA".bin();

            var tag1 = "9F10".bin();
            var tag2 = "5A".bin();
            var tag3 = "9F48".bin();
            var tag4 = "9F44".bin();
            var tag5 = "3F81828355".bin();
            var tag6 = "8F".bin();

            var expected1 = "9F1003A1A2A3".bin();
            var expected2 = "5A055152535455".bin();
            var expected3 = "9F4800".bin();
            var expected4 = "9F448101FF".bin();
            var expected5 = "3F8182835501BB".bin();
            var expected6 = "8F03AAAAAA".bin();

            //Act
            var res1 = data.FindTag(tag1);
            var res2 = data.FindTag(tag2);
            var res3 = data.FindTag(tag3);
            var res4 = data.FindTag(tag4);
            var res5 = data.FindTag(tag5);
            var res6 = data.FindTag(tag6);

            //Assert
            Assert.IsTrue(res1.SequenceEqual(expected1));
            Assert.IsTrue(res2.SequenceEqual(expected2));
            Assert.IsTrue(res3.SequenceEqual(expected3));
            Assert.IsTrue(res4.SequenceEqual(expected4));
            Assert.IsTrue(res5.SequenceEqual(expected5));
            Assert.IsTrue(res6.SequenceEqual(expected6));
        }

        [TestMethod]
        public void FindTag_WithString_ReturnsString()
        {
            //Arrange
            var data = "9f1003a1a2a3 5A055152535455 9f4800 9F448101FF 3F8182835501BB 8F03AAAAAA";

            var tag1 = "9F10";
            var tag2 = "5A";
            var tag3 = "9F48";
            var tag4 = "9F44";
            var tag5 = "3F81828355";
            var tag6 = "8F";

            var expected1 = "9F1003A1A2A3";
            var expected2 = "5A055152535455";
            var expected3 = "9F4800";
            var expected4 = "9F448101FF";
            var expected5 = "3F8182835501BB";
            var expected6 = "8F03AAAAAA";

            //Act
            var res1 = data.FindTag(tag1);
            var res2 = data.FindTag(tag2);
            var res3 = data.FindTag(tag3);
            var res4 = data.FindTag(tag4);
            var res5 = data.FindTag(tag5);
            var res6 = data.FindTag(tag6);

            //Assert
            Assert.AreEqual(expected1, res1);
            Assert.AreEqual(expected2, res2);
            Assert.AreEqual(expected3, res3);
            Assert.AreEqual(expected4, res4);
            Assert.AreEqual(expected5, res5);
            Assert.AreEqual(expected6, res6);
        }

        [TestMethod]
        public void WrapSimpleTlv_WithByteValByteTag_ReturnsByteArray()
        {
            //Arrange
            var data = "11223344".bin();
            var tag = "9F10".bin();
            var expected = "9F10 04 11223344".bin();

            //Act
            var res = data.WrapTlv(tag);

            //Assert
            Assert.IsInstanceOfType(res, typeof(byte[]));
            Assert.IsTrue(res.SequenceEqual(expected));
        }

        [TestMethod]
        public void WrapSimpleTlv_WithByteValStringTag_ReturnsByteArray()
        {
            //Arrange
            var data = "11223344".bin();
            var tag = "9F10";
            var expected = "9F10 04 11223344".bin();

            //Act
            var res = data.WrapTlv(tag);

            //Assert
            Assert.IsInstanceOfType(res, typeof(byte[]));
            Assert.IsTrue(res.SequenceEqual(expected));
        }

        [TestMethod]
        public void WrapSimpleTlv_WithStringValByteTag_ReturnsString()
        {
            //Arrange
            var data = "11223344";
            var tag = "9F10".bin();
            var expected = "9F100411223344";

            //Act
            var res = data.WrapTlv(tag);

            //Assert
            Assert.IsInstanceOfType(res, typeof(string));
            Assert.AreEqual(expected, res);
        }

        [TestMethod]
        public void WrapSimpleTlv_WithStringValStringTag_ReturnsString()
        {
            //Arrange
            var data = "11223344";
            var tag = "9F10";
            var expected = "9F100411223344";

            //Act
            var res = data.WrapTlv(tag);

            //Assert
            Assert.IsInstanceOfType(res, typeof(string));
            Assert.AreEqual(expected, res);
        }

        [TestMethod]
        public void ToTlvDictionary_WithString_ReturnsDictionary()
        {
            //Arrange
            var data = "9f1003a1a2a3 5A055152535455 9f4800 9F448101FF 3F8182835501BB 8F03AAAAAA".bin();
            var expected = new List<KeyValuePair<string, byte[]>>()
            {
                new KeyValuePair<string, byte[]>("9F10", "9F1003A1A2A3".bin()), 
                new KeyValuePair<string, byte[]>("5A", "5A055152535455".bin()), 
                new KeyValuePair<string, byte[]>("9F48", "9F4800".bin()), 
                new KeyValuePair<string, byte[]>("9F44", "9F448101FF".bin()), 
                new KeyValuePair<string, byte[]>("3F81828355", "3F8182835501BB".bin()), 
                new KeyValuePair<string, byte[]>("8F", "8F03AAAAAA".bin())
            };

            //Act
            var res = data.ToTlvDictionary();

            //Assert
            foreach (var item in expected)
            {
                Assert.IsTrue(res.ContainsKey(item.Key));
                Assert.IsTrue(item.Value.SequenceEqual(res[item.Key]));
            }
        }

        [TestMethod]
        public void ToTlvDictionary_WithByteArray_ReturnsDictionary()
        {
            //Arrange
            var data = "9f1003a1a2a3 5A055152535455 9f4800 9F448101FF 3F8182835501BB 8F03AAAAAA";
            var expected = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("9F10", "9F1003A1A2A3"),
                new KeyValuePair<string, string>("5A", "5A055152535455"),
                new KeyValuePair<string, string>("9F48", "9F4800"),
                new KeyValuePair<string, string>("9F44", "9F448101FF"),
                new KeyValuePair<string, string>("3F81828355", "3F8182835501BB"),
                new KeyValuePair<string, string>("8F", "8F03AAAAAA")
            };

            //Act
            var res = data.ToTlvDictionary();

            //Assert
            foreach (var item in expected)
            {
                Assert.IsTrue(res.ContainsKey(item.Key));
                Assert.AreEqual(item.Value, res[item.Key]);
            }
        }

        [TestMethod]
        public void GetTagClass_WithByte_ReturnsClass()
        {
            //Arrange
            byte data1 = 0b00101010;
            byte data2 = 0b01101010;
            byte data3 = 0b10101010;
            byte data4 = 0b11101010;
            var expected1 = BerTlvTagClass.Universal;
            var expected2 = BerTlvTagClass.Application;
            var expected3 = BerTlvTagClass.ContextSpecific;
            var expected4 = BerTlvTagClass.Private;

            //Act
            var res1 = data1.GetTagClass();
            var res2 = data2.GetTagClass();
            var res3 = data3.GetTagClass();
            var res4 = data4.GetTagClass();

            //Assert
            Assert.AreEqual(expected1, res1);
            Assert.AreEqual(expected2, res2);
            Assert.AreEqual(expected3, res3);
            Assert.AreEqual(expected4, res4);
        }

        [TestMethod]
        public void GetTagClass_WithByteArray_ReturnsClass()
        {
            //Arrange
            var data1 = "3F11";
            var data2 = "7F22";
            var data3 = "BF33";
            var data4 = "FF44";
            var expected1 = BerTlvTagClass.Universal;
            var expected2 = BerTlvTagClass.Application;
            var expected3 = BerTlvTagClass.ContextSpecific;
            var expected4 = BerTlvTagClass.Private;

            //Act
            var res1 = data1.GetTagClass();
            var res2 = data2.GetTagClass();
            var res3 = data3.GetTagClass();
            var res4 = data4.GetTagClass();

            //Assert
            Assert.AreEqual(expected1, res1);
            Assert.AreEqual(expected2, res2);
            Assert.AreEqual(expected3, res3);
            Assert.AreEqual(expected4, res4);
        }

        [TestMethod]
        public void GetTagClass_WithString_ReturnsClass()
        {
            //Arrange
            var data1 = "3F11".bin();
            var data2 = "7F22".bin();
            var data3 = "BF33".bin();
            var data4 = "FF44".bin();
            var expected1 = BerTlvTagClass.Universal;
            var expected2 = BerTlvTagClass.Application;
            var expected3 = BerTlvTagClass.ContextSpecific;
            var expected4 = BerTlvTagClass.Private;

            //Act
            var res1 = data1.GetTagClass();
            var res2 = data2.GetTagClass();
            var res3 = data3.GetTagClass();
            var res4 = data4.GetTagClass();

            //Assert
            Assert.AreEqual(expected1, res1);
            Assert.AreEqual(expected2, res2);
            Assert.AreEqual(expected3, res3);
            Assert.AreEqual(expected4, res4);
        }

        [TestMethod]
        public void IsTlvDataConstructed_WithConstructedTag_ReturnsTrue()
        {
            //Arrange
            var data1 = "3F11";
            var data2 = "BF33".bin();

            //Act
            var res1 = data1.IsTlvDataConstructed();
            var res2 = data2.IsTlvDataConstructed();

            //Assert
            Assert.IsTrue(res1);
            Assert.IsTrue(res2);
        }

        [TestMethod]
        public void IsTlvDataConstructed_WithPrimitiveTag_ReturnsFalse()
        {
            //Arrange
            var data1 = "5A";
            var data2 = "8F".bin();

            //Act
            var res1 = data1.IsTlvDataConstructed();
            var res2 = data2.IsTlvDataConstructed();

            //Assert
            Assert.IsFalse(res1);
            Assert.IsFalse(res2);
        }

        [TestMethod]
        public void IsTlvDataPrimitive_WithConstructedTag_ReturnsFalse()
        {
            //Arrange
            var data1 = "3F11";
            var data2 = "BF33".bin();

            //Act
            var res1 = data1.IsTlvDataPrimitive();
            var res2 = data2.IsTlvDataPrimitive();

            //Assert
            Assert.IsFalse(res1);
            Assert.IsFalse(res2);
        }

        [TestMethod]
        public void IsTlvDataPrimitive_WithPrimitiveTag_ReturnsTrue()
        {
            //Arrange
            var data1 = "5A";
            var data2 = "8F".bin();

            //Act
            var res1 = data1.IsTlvDataPrimitive();
            var res2 = data2.IsTlvDataPrimitive();

            //Assert
            Assert.IsTrue(res1);
            Assert.IsTrue(res2);
        }

        [TestMethod]
        public void IsSingleByteTag_WithSingleByteTag_ReturnsTrue()
        {
            //Arrange
            var data1 = "5A";
            var data2 = "8F".bin();

            //Act
            var res1 = data1.IsSingleByteTag();
            var res2 = data2.IsSingleByteTag();

            //Assert
            Assert.IsTrue(res1);
            Assert.IsTrue(res2);
        }

        [TestMethod]
        public void IsSingleByteTag_WithMultibyteTag_ReturnsFalse()
        {
            //Arrange
            var data1 = "3F11";
            var data2 = "BF33".bin();

            //Act
            var res1 = data1.IsSingleByteTag();
            var res2 = data2.IsSingleByteTag();

            //Assert
            Assert.IsFalse(res1);
            Assert.IsFalse(res2);
        }

        [TestMethod]
        public void IsLastTagByte_WithLastTagByte_ReturnsTrue()
        {
            //Arrange
            byte data = 0x55;

            //Act
            var res = data.IsLastTagByte();

            //Assert
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void IsLastTagByte_WithNonLastTagByte_ReturnsFalse()
        {
            //Arrange
            byte data = 0x81;

            //Act
            var res = data.IsLastTagByte();

            //Assert
            Assert.IsFalse(res);
        }


    }
}
