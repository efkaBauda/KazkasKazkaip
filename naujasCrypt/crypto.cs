using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace naujasCrypt
{
    internal class crypto
    {

        class AesEncrypter
        {
            static void Main()
            {
                string iv = "qwertyuiopasdfgh";
                /*
                Console.WriteLine("iveskite norima uzkoduoti fraze");
                string originalText = Console.ReadLine();
                Console.WriteLine("iveskite norima naudoti rakta");
                string key = Console.ReadLine();*/
                Console.WriteLine("koki sifravimo buda norite naudoti ?   1 - AES     2 - 3DES");
                string option = Console.ReadLine();
                Console.WriteLine("irasykite norima uzkoduoti zodi: ");
                string originalText = Console.ReadLine();
                Console.WriteLine("irasykite norima naudoti rakta:");
                string key = Console.ReadLine();

                if ( option == "1" )
                {
                    EncryptAes(originalText, key, iv);
                    Console.WriteLine(DecryptAes(key, iv));
                }
                if ( option == "2" )
                {
                    Encrypt3DES(originalText, key);
                    //Console.WriteLine(Decrypt3DES(key));
                    Decrypt3DES(key);
                }

                

            }

            public static byte[] MakeKey(string stringKey, int size)
            {
                byte[] fakeKey = new byte[size];
                byte[] textBytes = Encoding.UTF8.GetBytes(stringKey);

                for (int i = 0; i < size; i++)
                {
                    if (textBytes.Length > i)
                    {
                        fakeKey[i] = (byte)textBytes[i];
                    }
                    else
                    {
                        fakeKey[i] = (byte)i;
                    }
                }
                return fakeKey;
            }




            public static byte[] MakeIv(byte[] key, int size)
            {
                byte[] fakeIV = new byte[size];
                for (int i = 0; i < size / 2; i++)
                {
                    fakeIV[i] = key[i * 2];
                    fakeIV[i + size / 2] = key[i * 2 + 1];
                }
                return fakeIV;
            }


            static void EncryptAes(string plainText, string key, string iv)
            {
                using (AesManaged aesAlg = new AesManaged())
                {
                    aesAlg.Key = MakeKey(key, 32);
                    aesAlg.IV = MakeIv(aesAlg.Key, 16);

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (FileStream fsEncrypt = new FileStream("dataAES.txt", FileMode.OpenOrCreate))
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(fsEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }
                        }
                    }
                }
            }



            static string DecryptAes(string key, string iv)
            {
                using (AesManaged aesAlg = new AesManaged())
                {

                    aesAlg.Key = MakeKey(key, 32);
                    aesAlg.IV = MakeIv(aesAlg.Key, 16);

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (FileStream fsDecrypt = new FileStream("dataAES.txt", FileMode.Open))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(fsDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }



            static void Encrypt3DES(string originalText, string key)
            {
                byte[] Key = MakeKey(key, 24);
                byte[] IV = MakeIv(Key, 8);
                using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Padding = PaddingMode.PKCS7;
                    ICryptoTransform encryptor = tdes.CreateEncryptor(Key, IV);
                    using (FileStream fs = new FileStream("data3DES.txt", FileMode.OpenOrCreate))
                    {
                        using (CryptoStream cs = new CryptoStream(fs, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(cs))
                            {
                                sw.Write(originalText);
                            }
                        }
                    }
                }
            }

            static void Decrypt3DES(string Skey)
            {
                byte[] Key = MakeKey(Skey, 24);
                byte[] IV = MakeIv(Key, 8);
                using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Padding = PaddingMode.PKCS7;
                    tdes.KeySize = 192;
                    ICryptoTransform decryptor = tdes.CreateDecryptor(Key, IV);
                    using (FileStream fs = new FileStream("data3DES.txt", FileMode.Open))
                    {
                        using (CryptoStream cs = new CryptoStream(fs, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader(cs))
                            {
                                try
                                {
                                    string decryptedPassword = streamReader.ReadToEnd();
                                    Console.WriteLine("Decrypted Password (3DES): " + decryptedPassword + "\r\n");
                                }
                                catch
                                {
                                    Console.WriteLine("Failed to decrypt password (3DES)" + "\r\n");
                                }
                            }
                        }
                    }
                }
            }



        }
    }
}