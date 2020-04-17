using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SamOatesGames.Systems.Core;
using UnityEngine;

namespace SamOatesGames.Systems
{
    public class FileSaveSystem : UnitySingleton<FileSaveSystem>
    {
        /// <summary>
        /// 
        /// </summary>
        public string Passcode;

        /// <summary>
        /// 
        /// </summary>
        private byte[] m_passcode;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (Passcode.Length != 16)
            {
                Debug.LogError("[FileSaveSystem] Your passcode must by 16 characters long.");
            }

            m_passcode = Encoding.UTF8.GetBytes(Passcode);
            Passcode = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        public bool SaveData<T>(T data, string fileName)
        {
            var content = JsonUtility.ToJson(data);
            var encryptedData = EncryptJson(content);

            var filePath = Path.Combine(Application.persistentDataPath, fileName);
            try
            {
                using (var streamWriter = new StreamWriter(filePath, false))
                {
                    streamWriter.Write(encryptedData);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("[FileSaveSystem] Failed to save to the file '{0}'.", filePath);
                Debug.LogException(ex);
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool LoadData<T>(out T data, string fileName)
        {
            data = default(T);

            var filePath = Path.Combine(Application.persistentDataPath, fileName);
            if (!File.Exists(filePath))
            {
                return false;
            }

            string encryptedData = null;
            try
            {
                using (var streamReader = new StreamReader(filePath))
                {
                    encryptedData = streamReader.ReadToEnd();
                }

                var content = DecryptToJson(encryptedData);
                data = JsonUtility.FromJson<T>(content);
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    // Attempt a legacy load.
                    // If we succeed in loading, when the file is saved
                    // it will be saved as the new file format
                    if (encryptedData != null)
                    {
                        var content = Encoding.UTF8.GetString(Convert.FromBase64String(encryptedData));
                        data = JsonUtility.FromJson<T>(content);
                        return true;
                    }
                }
                catch (Exception)
                {
                    Debug.LogErrorFormat("[FileSaveSystem] Failed to load from the file '{0}'. Reason: {1}", filePath,
                        ex);
                }
            }

            // Failed to load the file, back it up, but we won't load this again.
            try
            {
                new FileInfo(filePath).IsReadOnly = false;
                File.Move(filePath, filePath + "." + DateTime.Now.Ticks + ".bak");
            }
            catch (Exception)
            {
                // ignored
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        private string EncryptJson(string jsonContent)
        {
            var toEncryptArray = Encoding.UTF8.GetBytes(jsonContent);
            var rDel = CreateRijndaelManaged();
            var cTransform = rDel.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedData"></param>
        /// <returns></returns>
        private string DecryptToJson(string encryptedData)
        {
            var toEncryptArray = Convert.FromBase64String(encryptedData);
            var rDel = CreateRijndaelManaged();
            var cTransform = rDel.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private RijndaelManaged CreateRijndaelManaged()
        {
            var result = new RijndaelManaged();

            var newKeysArray = new byte[16];
            Array.Copy(m_passcode, 0, newKeysArray, 0, 16);

            result.Key = newKeysArray;
            result.Mode = CipherMode.ECB;
            result.Padding = PaddingMode.PKCS7;
            return result;
        }
    }
}
