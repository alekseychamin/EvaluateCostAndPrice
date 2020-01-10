using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    static class ReadFile
    {
        public static string[] GetSplitString(string line)
        {
            return line.Split(';');
        }

        public static void Load<T>(string filename, List<T> listToAdd, string folder = null) where T : IGetTypeObject
        {
            listToAdd.Clear();
            string projectFileName;

            if (folder != null)
                projectFileName = folder + "\\" + filename;
            else
                projectFileName = filename;

            List<T> listT = new List<T>();
            listT = ReadFile.GetObjects<T>(projectFileName, Properties.Value);
            listToAdd.AddRange(listT);
        }

        public static string[] GetEnHeaders(string headersRus, Dictionary<string, string> headRuEn)
        {
            string[] words = GetSplitString(headersRus);
            string[] output = new string[words.Length];
            int i = 0;

            foreach (var item in words)
            {
                item.Trim();
                if (headRuEn.ContainsKey(item))
                    output[i] = headRuEn[item];
                else
                    output[i] = null;
                i++;
            }
            return output;
        }
        public static List<T> GetObjects<T>(string filename, Dictionary<string, string> headRuEn) where T : IGetTypeObject
        {
            List<T> listT = new List<T>();

            var lines = File.ReadAllLines(filename, Encoding.Default).ToList();
            var headersRus = lines.First();
            lines.RemoveAt(0);
            string[] headersEn = ReadFile.GetEnHeaders(headersRus, headRuEn);

            foreach (var line in lines)
            {
                Type type = TypeObject.GetTypeObject(line, headersEn);
                if (type != null)
                {
                    T obj = ReadFile.CreateType<T>(line, headersEn, type);
                    obj.GetTypeObject = TypeObject.GetTypeObject;
                    listT.Add(obj);
                }
            }
            return listT;
        }
        private static double? GetPercentFromString(string str)
        {
            int iPercent = str.IndexOf(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol);

            if (iPercent > 0)
            {
                str = str.Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                return double.Parse(str) / 100;
            }
            else
                return null;
        }
        public static T CreateType<T>(string line, string[] headersEn, Type type)
        {
            string[] lines = GetSplitString(line);
            T obj = (T)Activator.CreateInstance(type);
            PropertyInfo[] props = obj.GetType().GetProperties();

            foreach (var prop in props)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (prop.Name == headersEn[i])
                    {
                        string sprop = lines[i].Trim();

                        if (prop.PropertyType.IsEnum)
                        {
                            Enum eprop = TypeObject.GetTypeObject(sprop);
                            prop.SetValue(obj, Convert.ChangeType(eprop, prop.PropertyType));
                            //break;
                        }
                        else if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
                        {
                            if (!string.IsNullOrEmpty(sprop))
                            {
                                double? dprop = GetPercentFromString(sprop);

                                if (dprop != null)
                                    prop.SetValue(obj, Convert.ChangeType(dprop, Nullable.GetUnderlyingType(prop.PropertyType)));
                                else
                                    prop.SetValue(obj, Convert.ChangeType(sprop, Nullable.GetUnderlyingType(prop.PropertyType)));
                            }
                            else
                            {
                                object propValue = prop.GetValue(obj);

                                if ((propValue == null) || ((double)propValue == 0))
                                    prop.SetValue(obj, null);
                            }
                            //break;
                        }
                        else
                        {
                            double? dprop = GetPercentFromString(sprop);

                            if (dprop != null)
                                prop.SetValue(obj, Convert.ChangeType(dprop, prop.PropertyType));
                            else                                                           
                                prop.SetValue(obj, Convert.ChangeType(sprop, prop.PropertyType));
                            

                            //int iPercent = sprop.IndexOf(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol);
                            //if (iPercent > 0)
                            //{
                            //    sprop = sprop.Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                            //    double dPercent = double.Parse(sprop) / 100;
                            //    prop.SetValue(obj, Convert.ChangeType(dPercent, prop.PropertyType));
                            //}
                            //else
                            //    prop.SetValue(obj, Convert.ChangeType(sprop, prop.PropertyType));

                            //break;
                        }
                    }
                }
            }
            return obj;
        }
        public static object CreateType(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}
