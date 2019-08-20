// ***********************************************************************
// Assembly         : Encryption
// Author           : Rajendra
// Created          : 05-23-2013
//
// Last Modified By : Rajendra
// Last Modified On : 05-23-2013
// ***********************************************************************
// <copyright file="ClsExtention.cs" company="Microsoft">
//     Microsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Globalization;

namespace Encryption
{
    public static class ClsExtention
    {
        public static XmlDocument GetXmlData(string path)
        {
            XmlDocument doc;
            RijndaelManaged key = null;
            try
            {
                key = new RijndaelManaged();
                doc = new XmlDocument();
                doc.Load(path);
                Decrypt(doc);
             //   Convert.ToDateTime(path, CultureInfo.CurrentCulture);
                //doc.Save(path);
            }
            catch (Exception ex)
            {
                throw; 
            }
            return doc;
        }

        public static XmlDocument EncryptXmlData(string path)
        {
            XmlDocument doc;
            RijndaelManaged key = null;
            try
            {
                key = new RijndaelManaged();
                doc = new XmlDocument();
                doc.Load(path);
                Encrypt(doc);
                doc.Save(path);
            }
            catch (Exception ex)
            {
                throw; 
            }
            return doc;
        }

        private static void Decrypt(XmlDocument doc)
        {
            try
            {
                if (doc == null)
                    throw new ArgumentNullException("Doc");
                SymmetricAlgorithm symAlgo = new RijndaelManaged();
                byte[] salt = Encoding.ASCII.GetBytes("CAD!ineEncrypT!0N");
                Rfc2898DeriveBytes theKey = new Rfc2898DeriveBytes("myclass", salt);
                symAlgo.Key = theKey.GetBytes(symAlgo.KeySize / 8);
                symAlgo.IV = theKey.GetBytes(symAlgo.BlockSize / 8);
                XmlElement encryptedElement = doc.DocumentElement;
                if (encryptedElement == null)
                {
                    throw new XmlException("The EncryptedData element was not found.");
                }
                EncryptedData edElement = new EncryptedData();
                edElement.LoadXml(encryptedElement);
                EncryptedXml exml = new EncryptedXml();
                byte[] rgbOutput = exml.DecryptData(edElement, symAlgo);
                exml.ReplaceData(encryptedElement, rgbOutput);
            }
            catch
            {
                throw;
            }
        }

        public static XmlDocument Encrypt(XmlDocument doc)
        {
            SymmetricAlgorithm symAlgo = new RijndaelManaged();
            byte[] salt = Encoding.ASCII.GetBytes("CAD!ineEncrypT!0N");
            Rfc2898DeriveBytes theKey = new Rfc2898DeriveBytes("myclass", salt);
            symAlgo.Key = theKey.GetBytes(symAlgo.KeySize / 8);
            symAlgo.IV = theKey.GetBytes(symAlgo.BlockSize / 8);
            if (doc == null)
                throw new ArgumentNullException("Doc");
            if (symAlgo == null)
                throw new ArgumentNullException("Alg");
            XmlElement elementToEncrypt = doc.DocumentElement;
            if (elementToEncrypt == null)
            {
                throw new XmlException("The specified element was not found");

            }
            EncryptedXml eXml = new EncryptedXml();
            byte[] encryptedElement = eXml.EncryptData(elementToEncrypt, symAlgo, false);
            EncryptedData edElement = new EncryptedData();
            edElement.Type = EncryptedXml.XmlEncElementUrl;
            string encryptionMethod = null;
            if (symAlgo is TripleDES)
            {
                encryptionMethod = EncryptedXml.XmlEncTripleDESUrl;
            }
            else if (symAlgo is DES)
            {
                encryptionMethod = EncryptedXml.XmlEncDESUrl;
            }
            if (symAlgo is Rijndael)
            {
                switch (symAlgo.KeySize)
                {
                    case 128:
                        encryptionMethod = EncryptedXml.XmlEncAES128Url;
                        break;
                    case 192:
                        encryptionMethod = EncryptedXml.XmlEncAES192Url;
                        break;
                    case 256:
                        encryptionMethod = EncryptedXml.XmlEncAES256Url;
                        break;
                    default:
                        // do the defalut action
                        break;
                }
            }
            else
            {
                throw new CryptographicException("The specified algorithm is not supported for XML Encryption.");
            }
            edElement.EncryptionMethod = new EncryptionMethod(encryptionMethod);
            edElement.CipherData.CipherValue = encryptedElement;
            EncryptedXml.ReplaceElement(elementToEncrypt, edElement, false);
            return doc;
        }

    }
}
