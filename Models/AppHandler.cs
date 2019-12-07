using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PropertyManagerWeb.Models
{
    public class AppHandler
    {
        static DB db = new DB();

        public static string Sha1(string rawText)
        {
            SHA1 _sha1 = SHA1.Create();
            byte[] sha1Byte = Encoding.Default.GetBytes(rawText);
            byte[] finalByte = _sha1.ComputeHash(sha1Byte);
            StringBuilder strBdr = new StringBuilder();
            int i = 0;
            do
            {
                strBdr.AppendFormat("{0:X2}", finalByte[i]);
                ++i;
            } while (i < finalByte.Length);
            return strBdr.ToString().ToLower();
        }

        public static bool IsEmpty(string str, bool trm = true)
        {
            if (trm)
                if (str.Trim().Length == 0) return true;
                else
                    if (str.Length == 0) return true;
            return false;
        }

        static Random rnd = new Random();
        public static int Rand(int lower, int upper)
        {
            return rnd.Next(lower, upper);
        }

        public static double Rand()
        {
            return rnd.NextDouble();
        }

        public static ArrayList Shuffle(ArrayList arr)
        {
            ArrayList newArray = new ArrayList();
            do
            {
                if (arr.Count == 0) break;
                int i = rnd.Next(0, arr.Count - 1);
                newArray.Add(arr[i]);
                arr.RemoveAt(i);
                SwapFirstLast(ref arr);
            } while (true);
            return newArray;
        }

        public static void SwapFirstLast(ref ArrayList arr)
        {
            if (arr.Count > 1)
            {
                string temp = arr[0].ToString();
                arr[0] = arr[arr.Count - 1];
                arr[arr.Count - 1] = temp;
            }
        }

        public static byte[] ImageToBinary(Image img)
        {
            MemoryStream imgStream = new MemoryStream();
            img.Save(imgStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] imgBinary = new byte[imgStream.Length];
            for (int i = 0; i < imgStream.ToArray().Length; i++)
                imgBinary[i] = imgStream.ToArray()[i];
            return imgBinary;
        }

        public static Image BinaryToImage(byte[] imgBytes)
        {
            MemoryStream imgMemory = new MemoryStream(imgBytes);
            Image image = Image.FromStream(imgMemory);
            return image;
        }

        public static bool IsEmptyImage(Image img)
        {
            try
            {
                int h = img.Size.Height;
            }
            catch (Exception ex)
            {
                return true;
            }
            return false;
        }

        public static bool IsEmptyByte(byte[] byt)
        {
            try
            {
                int l = byt.Length;
            }
            catch (Exception ex)
            {
                return true;
            }
            return false;
        }

        public static byte[] ByteCast(object byteArg)
        {
            try
            {
                return (byte[])byteArg;
            }
            catch (Exception ex) { }
            return new byte[] { };
        }

        public static string ImageToString(byte[] imageByte)
        {
            return Convert.ToBase64String(imageByte);
        }

        public static byte[] StringToImage(string imageString)
        {
            return Convert.FromBase64String(imageString);
        }

        public static string ArrayToString(Array arr, char delim = ',')
        {
            string strResult = "";
            foreach (Object a in arr)
            {
                strResult = a.ToString() + delim;
            }
            return strResult.Substring(0, strResult.Length - 1);
        }
        public static decimal[] String2DecimalArray(string str, int c = 0)
        {
            string[] arrString = str.Split(';');
            decimal[] dec = new decimal[arrString.Length];
            if (c == 0 || c > arrString.Length) c = arrString.Length;
            for (int i = 0; i < arrString.Length; i++)
                dec[i] = ToDecimal(arrString[i]);
            return dec;
        }

        public static int ToInt(object str)
        {
            int i = 0;
            int.TryParse(str.ToString(), out i);
            return i;
        }

        public static decimal ToDecimal(string str)
        {
            decimal dec = 0;
            decimal.TryParse(str, out dec);
            return dec;
        }

        public static double ToDouble(string str)
        {
            double dec = 0.0;
            double.TryParse(str, out dec);
            return dec;
        }

        public static DateTime ToDateTime(string str)
        {
            try
            {
                return Convert.ToDateTime(str);
            }
            catch (Exception ex) { }
            return new DateTime(1800, 1, 1);
        }

        public static DateTime ToTime(string strTime)
        {
            string[] arrTime = strTime.Split(':');
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ToInt(arrTime[0]), ToInt(arrTime[1]), ToInt(arrTime[2]));
        }

        public static string ToMySqlTime(DateTime dt)
        {
            string ampm = dt.ToLongTimeString().Contains("PM") ? "PM" : "AM";
            string[] strTime = dt.ToLongTimeString().Split(' ')[0].Split(':');
            int hr = AppHandler.ToInt(strTime[0]);
            if (hr < 10 && ampm == "AM") return "0" + Implode(strTime, ":");
            else if (hr < 10 && ampm == "PM") return (12 + hr) + ":" + strTime[1] + ":" + strTime[2];
            else if (hr == 12 && ampm == "PM") return "00:" + strTime[1] + ":" + strTime[2];
            return Implode(strTime, ":");
        }

        public static int[] String2IntegerArray(string str, int c = 0)
        {
            string[] arrString = str.Split(';');
            int[] intR = new int[arrString.Length];
            if (c == 0 || c > arrString.Length) c = arrString.Length;
            for (int i = 0; i < arrString.Length; i++)
                intR[i] = Convert.ToInt32(arrString[i]);
            return intR;
        }
        public static string ByteToStringOfIntegers(byte[] byt)
        {
            ArrayList integerStrings = new ArrayList();
            foreach (byte b in byt)
            {
                integerStrings.Add(Convert.ToInt32(b));
            }
            return Implode(integerStrings.ToArray(), ";");
        }
        public static byte[] StringIntegerToByte(string[] strInteger)
        {
            int i = -1;
            byte[] finalBytes = new byte[strInteger.Length];
            foreach (string si in strInteger)
            {
                finalBytes[++i] = Convert.ToByte(Convert.ToInt32(si));
            }
            return finalBytes;
        }

        public static bool IsNumeric(string input)
        {
            double num;
            return double.TryParse(input, out num);
        }

        public static bool CorrectSessionFormat(string ss)
        {
            string[] sess = ss.Split('/');
            if (sess.Length < 2) return false;
            if (!IsNumeric(sess[0]) || !IsNumeric(sess[1])) return false;
            if (Convert.ToInt32(sess[0]) < 2000 || Convert.ToInt32(sess[1]) < 2001) return false;
            if (Convert.ToInt32(sess[0]) >= Convert.ToInt32(sess[1])) return false;
            if (Convert.ToInt32(sess[1]) - Convert.ToInt32(sess[0]) != 1) return false;
            return true;
        }

        public static string FormatDate(DateTime dt)
        {
            return dt.Year + "-" + (dt.Month < 10 ? "0" : "") + dt.Month + "-" + (dt.Day < 10 ? "0" : "") + dt.Day;
        }

        public static string FormatDate(string d)
        {
            string[] dt = d.Split('/');
            return dt[2] + "-" + (ToInt(dt[1]) < 10 ? "0" : "") + dt[1] + "-" + (ToInt(dt[0]) < 10 ? "0" : "") + dt[0];
        }

        public static string To24Hours(string time)
        {
            string[] strTime = time.Replace(" AM", "").Replace(" PM", "").Split(':');
            string finalTime = "";
            int hr = ToInt(strTime[0]);
            if (time.EndsWith("AM"))
            {
                if (hr == 12)
                    finalTime = "00:" + strTime[1] + ":" + strTime[2];
                else
                    finalTime = (hr < 10 ? "0" + hr : hr.ToString()) + ":" + strTime[1] + ":" + strTime[2];
            }
            else
            {
                if (hr < 12)
                    hr += 12;
                finalTime = hr + ":" + strTime[1] + ":" + strTime[2];
            }
            return finalTime;
        }

        public static string Encrypt(string str)
        {
            if (str == string.Empty) return "";
            string encryptedString = Convert.ToBase64String(Encoding.Default.GetBytes(ReverseString(str)));
            encryptedString = Convert.ToBase64String(Encoding.Default.GetBytes(ReverseString(encryptedString)));
            return encryptedString;
        }

        public static string Decrypt(string encryptedString)
        {
            if (encryptedString == string.Empty) return "";
            string decryptedString = ReverseString(ByteToString(Convert.FromBase64String(encryptedString)));
            decryptedString = ReverseString(ByteToString(Convert.FromBase64String(decryptedString)));
            return decryptedString;
        }

        public static string ByteToString(byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }

        public static void MsgBox(string msg, string title = "Alert Info")
        {
            //System.Windows.Forms.MessageBox.Show(msg, title, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

        public static string GetFileExtension(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            return fi.Extension;
        }

        public static char[] StringToCharArray(string str)
        {
            if (str == string.Empty)
                return new char[] { };
            char[] chars = new char[str.Length];
            for (int i = 0; i < str.Length; i++)
                chars[i] = Convert.ToChar(str.Substring(i, 1));
            return chars;
        }

        public static string ReverseString(string str)
        {
            if (str == string.Empty) return "";
            string reversedString = "";
            char[] chars = StringToCharArray(str);
            for (int i = chars.Length - 1; i >= 0; i--)
                reversedString += chars[i];
            return reversedString;
        }

        public static string IgnoreDBNullString(object arg)
        {
            try
            {
                return arg.ToString();
            }
            catch (Exception ex) { }
            return "";
        }

        public static string[] GetCountries()
        {
            DataTable countryTable = new DataTable();
            ArrayList countries = new ArrayList();
            try
            {
                countryTable = db.ReadTable("Countries");
                if (countryTable.Rows.Count > 0)
                {
                    for (int i = 0; i < countryTable.Rows.Count; i++)
                        countries.Add(countryTable.Rows[i]["Country"]);
                }
            }
            catch (Exception ex) { }
            return (string[])countries.ToArray(typeof(string));
        }

        public static DataTable GetStates()
        {
            return db.ReadTable("NigerianStates");
        }

        public static string[] GetLGAs(string state)
        {
            string strLgas = db.ReadData("SELECT LgasNames FROM NigerianStates WHERE state='" + state + "'");
            if (strLgas == string.Empty) return new string[] { };
            string[] lgas = strLgas.Split(';');
            ArrayList arrLgas = new ArrayList(lgas);
            arrLgas.Sort();
            return (string[])arrLgas.ToArray(typeof(string));
        }

        public static string Implode(object[] arr, string delim = ",")
        {
            if (arr.Length == 0) return "";
            string result = "";
            for (int i = 0; i < arr.Length; i++)
            {
                result += arr[i] + (i == arr.Length - 1 ? "" : delim);
            }
            return result;
        }

        public static bool IsEmptySocres(string[] scores)
        {
            bool empty = true;
            foreach (string score in scores)
            {
                if (score == null) return false;
                if (score.Length > 0)
                {
                    empty = false;
                    break;
                }
            }
            return empty;
        }

        public static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        public static void Sort(int[] arr, bool descOrder = false)
        {
            for (int i = 0; i < arr.Length - 1; i++)
            {
                if (descOrder)
                {
                    if (arr[i] < arr[i + 1])
                    {
                        Swap(ref arr[i], ref arr[i + 1]);
                        i = -1;
                    }
                }
                else
                {
                    if (arr[i] > arr[i + 1])
                    {
                        Swap(ref arr[i], ref arr[i + 1]);
                        i = -1;
                    }
                }
            }
        }

        public static string PositionToString(int pos)
        {
            string stringPosition = "";
            if (pos < 1)
                stringPosition = "";
            else if (pos.ToString().EndsWith("11") || pos.ToString().EndsWith("12") || pos.ToString().EndsWith("13"))
                stringPosition = pos + "th";
            else if (pos.ToString().EndsWith("1"))
                stringPosition = pos + "st";
            else if (pos.ToString().EndsWith("2"))
                stringPosition = pos + "nd";
            else if (pos.ToString().EndsWith("3"))
                stringPosition = pos + "rd";
            else
                stringPosition = pos + "th";
            return stringPosition;
        }

        public static int IntegerCast(string s)
        {
            try
            {
                return Convert.ToInt32(s);
            }
            catch (Exception ex) { }
            return 0;
        }

        public static double DoubleCast(string s)
        {
            try
            {
                return Convert.ToDouble(s);
            }
            catch (Exception ex) { }
            return 0.0;
        }

        public static string HttpRemoteServerRequest(string request, string xtra = "")
        {
            string responseString = "";
            Uri targetUri = new Uri("http://localhost:1486/HRMS.net/Handlers/RegisterSchool.ashx");
            try
            {
                HttpWebRequest bsHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(targetUri);
                // Set the 'Method' property of the 'Webrequest' to 'POST'.
                bsHttpWebRequest.Method = "POST";
                string postData = "app_request=" + request + xtra;
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] byte1 = encoding.GetBytes(postData);
                // Set the content type of the data being posted.
                bsHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
                // Set the content length of the string being posted.
                bsHttpWebRequest.ContentLength = byte1.Length;
                Stream newStream = bsHttpWebRequest.GetRequestStream();
                newStream.Write(byte1, 0, byte1.Length);
                newStream.Close();
                HttpWebResponse requestResponse;
                requestResponse = (HttpWebResponse)bsHttpWebRequest.GetResponse();

                StreamReader PostRequestReader = new StreamReader(requestResponse.GetResponseStream());

                responseString = PostRequestReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                responseString = "Error: " + ex.Message;
            }
            return responseString;
        }

        public static string UploadPassportIntoServer(string code, string passportFilename)
        {
            WebClient client = new WebClient();
            NameValueCollection postQueryCol = new NameValueCollection();
            postQueryCol.Add("code", code);
            postQueryCol.Add("uploadimage", "true");
            client.QueryString = postQueryCol;
            //client.UploadProgressChanged
            byte[] r = client.UploadFile(new Uri("http://localhost:1486/HRMS.net/Default.aspx"), passportFilename);
            return System.Text.ASCIIEncoding.Default.GetString(r);
        }

        public static int GetAge(string bdt)
        {
            var age = 0;
            string[] strAge = bdt.Split('-');
            DateTime dt = new DateTime(Convert.ToInt32(strAge[0]), Convert.ToInt32(strAge[1]), Convert.ToInt32(strAge[2]));
            DateTime cdt = new DateTime();
            if (cdt.Month >= dt.Month)
                age = cdt.Year - dt.Year;
            else
                age = cdt.Year - dt.Year - 1;
            return age;
        }

        public static int GetAge(DateTime bdt)
        {
            var age = 0;
            DateTime cdt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            if (cdt.Month >= bdt.Month)
                age = cdt.Year - bdt.Year;
            else
                age = cdt.Year - bdt.Year - 1;
            return age;
        }

        public static bool IsGreaterDate(string dt1, string dt2)
        {
            string[] arrDt1 = dt1.Split('-');
            string[] arrDt2 = dt2.Split('-');
            if (Convert.ToInt32(arrDt1[0]) > Convert.ToInt32(arrDt2[0]))
                return true;
            else if (arrDt1[0] == arrDt2[0] && Convert.ToInt32(arrDt1[1]) > Convert.ToInt32(arrDt2[1]))
                return true;
            else if (arrDt1[0] == arrDt2[0] && arrDt1[1] == arrDt2[1] && Convert.ToInt32(arrDt1[2]) > Convert.ToInt32(arrDt2[2]))
                return true;
            return false;
        }

        public static bool IsGreaterDate(DateTime dt1, DateTime dt2)
        {
            string[] arrDt1 = { dt1.Year.ToString(), dt1.Month.ToString(), dt1.Day.ToString() };// dt1.Split('-');
            string[] arrDt2 = { dt2.Year.ToString(), dt2.Month.ToString(), dt2.Day.ToString() };//dt2.Split('-');
            if (Convert.ToInt32(arrDt1[0]) > Convert.ToInt32(arrDt2[0]))
                return true;
            else if (arrDt1[0] == arrDt2[0] && Convert.ToInt32(arrDt1[1]) > Convert.ToInt32(arrDt2[1]))
                return true;
            else if (arrDt1[0] == arrDt2[0] && arrDt1[1] == arrDt2[1] && Convert.ToInt32(arrDt1[2]) > Convert.ToInt32(arrDt2[2]))
                return true;
            return false;
        }

        public static string PadZero(string s, int n)
        {
            for (int i = s.Length; i <= n - 1; i++)
            {
                s = "0" + s;
            }
            return s;
        }

        public static DateTime GetRetirementDate(DateTime birthdate, DateTime firstAppDate)
        {
            int yr = birthdate.Year;
            int age = AppHandler.GetAge(birthdate);
            int rYear = (60 - age) + DateTime.Now.Year;
            DateTime dt = new DateTime(firstAppDate.Year + 35, firstAppDate.Month, firstAppDate.Day);
            if (dt.Year - birthdate.Year <= 60)
                return dt;
            return new DateTime(rYear, birthdate.Month, birthdate.Day);
        }

        public static string YMD2DMY(string ymd)
        {
            string[] date = ymd.Split('-');
            return date[2] + "/" + date[1] + "/" + date[0];
        }

        public static string ToYMD(DateTime dt)
        {
            try
            {
                return dt.Year + "-" + (dt.Month < 10 ? "0" + dt.Month : dt.Month + "") + "-" + (dt.Day < 10 ? "0" + dt.Day : dt.Day + "");
            }
            catch (Exception ex) { }
            return "";
        }
        public static string ToDMY(DateTime dt)
        {
            try
            {
                return dt.Day + "/" + dt.Month + "/" + dt.Year;
            }
            catch (Exception ex) { }
            return "";
        }

        public static long GetDuration(DateTime d1, DateTime d2)
        {
            long interval = (d2.Ticks - d1.Ticks) / 10000000;
            interval = (interval / 86400) + 1;  //'Add 1 to make day 1 inclusive
            return interval;
        }

        public static void SelectDropDownListItem(string item, System.Web.UI.WebControls.DropDownList ddl)
        {
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Text == item)
                {
                    ddl.SelectedIndex = i;
                    break;
                }
            }
        }

        public static void SelectDropDownListValue(string value, System.Web.UI.WebControls.DropDownList ddl)
        {
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Value == value)
                {
                    ddl.SelectedIndex = i;
                    break;
                }
            }
        }

        public static string Ucwords(string words)
        {
            string finalWords = "";
            string[] arrWords = words.ToLower().Split(' ');
            foreach (string word in arrWords)
            {
                string newWord = word.Length > 1 ? word.Substring(0, 1).ToUpper() + word.Substring(1) : word.ToUpper();
                finalWords += newWord + " ";
            }
            return finalWords.Trim();
        }

        public static string CamelNotation(string words)
        {
            return Ucwords(words).Replace(" ", "");
        }

        public static string PreviousSession(string session)
        {
            string[] newSession = session.Split('/');
            newSession[0] = (AppHandler.ToInt(newSession[0]) - 1).ToString();
            newSession[1] = (AppHandler.ToInt(newSession[1]) - 1).ToString();
            return newSession[0] + "/" + newSession[1];
        }

        public static string NextSession(string session)
        {
            string[] newSession = session.Split('/');
            newSession[0] = (AppHandler.ToInt(newSession[0]) + 1).ToString();
            newSession[1] = (AppHandler.ToInt(newSession[1]) + 1).ToString();
            return newSession[0] + "/" + newSession[1];
        }

        public static string RenameColumn(string col)
        {
            string newCol = col;
            if (col == "FirstTerm")
                newCol = "1st Term";
            else if (col == "SecondTerm")
                newCol = "2nd Term";
            else if (col == "ThirdTerm")
                newCol = "3rd Term";
            else if (col == "SubjectAverage")
                newCol = "Average";
            else if (col == "TotalScore")
                newCol = "Total";
            else if (col == "TotalCA")
                newCol = "Total CA";
            else if (col == "StudentName")
                newCol = "Student Name";
            return newCol;
        }

        public static DataTable ToDataTable(string[] arr, string columnName)
        {
            DataTable thisTable = new DataTable("ThisTable");
            thisTable.Columns.Add(columnName);
            foreach (string a in arr)
            {
                DataRow dr = thisTable.NewRow();
                dr[columnName] = a;
                thisTable.Rows.Add(dr);
            }
            return thisTable;
        }
    }
}