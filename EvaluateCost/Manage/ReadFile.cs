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
            string[] headersRu = GetSplitString(headersRus);
            lines.RemoveAt(0);
            string[] headersEn = ReadFile.GetEnHeaders(headersRus, headRuEn);

            foreach (var line in lines)
            {
                Type type = TypeObject.GetTypeObject(line, headersEn);
                if (type != null)
                {
                    T obj = ReadFile.CreateType<T>(line, headersEn, headersRu, type);
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

        private static void SetNullableValue(object obj, PropertyInfo prop, string sprop)
        {
            if (!string.IsNullOrEmpty(sprop))
            {
                double? dprop = GetPercentFromString(sprop);

                if (dprop != null)
                    prop.SetValue(obj, dprop);
                else
                    prop.SetValue(obj, Convert.ChangeType(sprop, prop.PropertyType));
            }
            else
            {
                double? propValue = (double?)prop.GetValue(obj);

                if ((propValue == null) || (propValue.Value == 0))
                    prop.SetValue(obj, null);
            }            
        }

        public static T CreateType<T>(string line, string[] headersEn, string[] headersRu, Type type)
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
<<<<<<< HEAD
                            //break;
=======
>>>>>>> propertyName
                        }
                        else if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
                        {
                            SetNullableValue(obj, prop, sprop);
                        }
                        else if (prop.PropertyType.IsGenericType)
                        {
                            List<PropertyInfo> listGenericProp = prop.PropertyType.GetTypeInfo().DeclaredProperties.ToList();
                            Type generic = typeof(StringProperty<>);
                            Type[] types = new Type[] { listGenericProp[0].PropertyType };
                            Type curGenericType = generic.MakeGenericType(types);

                            dynamic createdGeneric = CreateType(curGenericType);
                            dynamic needGenericType = Convert.ChangeType(createdGeneric, curGenericType);
                            needGenericType.Name = headersRu[i].Trim();
                            if (listGenericProp[0].PropertyType.IsEnum)
                            {
                                Enum eprop = TypeObject.GetTypeObject(sprop);
                                needGenericType.Value = eprop;
                                prop.SetValue(obj, needGenericType);
                            }
                            else if (Nullable.GetUnderlyingType(listGenericProp[0].PropertyType) != null)
                            {
                                if (!string.IsNullOrEmpty(sprop))
                                {
                                    double? dprop = GetPercentFromString(sprop);

                                    if (dprop != null)
                                        needGenericType.Value = dprop;
                                    else
                                        needGenericType.Value = double.Parse(sprop);

                                    prop.SetValue(obj, needGenericType);
                                }
                                else
                                {
                                    dynamic propValue = prop.GetValue(obj);

                                    if ((propValue == null) || (propValue.Value == 0))
                                    {
                                        needGenericType.Value = null;
                                        prop.SetValue(obj, needGenericType);
                                    }
                                }                                
                            }
                            else
                            {
<<<<<<< HEAD
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
=======
                                needGenericType.Value = sprop;                                
                                prop.SetValue(obj, needGenericType);
                            }

                        }
                        else
                        {
                            prop.SetValue(obj, Convert.ChangeType(sprop, prop.PropertyType));
>>>>>>> propertyName
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
