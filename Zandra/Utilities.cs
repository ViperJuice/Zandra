using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Zandra
{
    public class Utilities
    {
        public ZandraUserPreferences userPreferences;
        public Utilities()
        {
            userPreferences = new ZandraUserPreferences(this);
        }
        public static void SaveToXml(Object objectToSave, string filePath)
        {
            if (objectToSave != null)
            {
                string text;
                try
                {
                    text = StringToXML(ref objectToSave);
                    System.IO.File.WriteAllText(Path.GetFullPath(filePath), text);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (System.IO.DirectoryNotFoundException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (System.IO.DriveNotFoundException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (System.UnauthorizedAccessException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public static string StringToXML(ref Object objectToSerialize)
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(objectToSerialize.GetType());
                try
                {
                    serializer.Serialize(stringwriter, objectToSerialize);

                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                    throw;
                }
                return stringwriter.ToString();
            }
        }
        public static bool RestoreFromXML<T>(ref T target, string filePath) where T : class
        {
            string xmlText;
            try
            {
                xmlText = System.IO.File.ReadAllText(filePath);
                StringReader stringReader = new System.IO.StringReader(xmlText);
                try
                {
                    XmlSerializer serializer = new XmlSerializer(target.GetType());
                    target = serializer.Deserialize(stringReader) as T;
                    return true;
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                    throw (ex);
                }
            }
            catch (System.IO.FileNotFoundException ex)
            {
                return false;
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (System.IO.DriveNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            catch (System.UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public void RestoreMe(ref ZandraUserPreferences me)
        {
            RestoreFromXML(ref me, Path.GetFullPath(@"ZandraUserPreferences.xml"));
        }
    }
}

