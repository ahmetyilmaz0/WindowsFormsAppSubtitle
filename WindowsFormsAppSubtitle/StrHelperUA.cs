
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WindowsFormsAppSubtitle
{
    //Analyze the subtitle file
    public class StrHelperUA
    {
        //Define a list of ModelList to accept the content read from the file
        private static List<StrModelUA> mySrtModelList;

        //Define a method to get the string displayed at the current time
        public static string GetTimeString(int timeMile)
        {
            String currentTimeTxt = "";
            if (mySrtModelList != null)
            {
                foreach (StrModelUA sm in mySrtModelList)
                {
                    if (timeMile > sm.BeginTime && timeMile < sm.EndTime)
                    {
                        currentTimeTxt = sm.SrtString;
                    }
                }
            }
            return currentTimeTxt;
        }

        //Read the contents of the file to the mySrtModelList list
        public static List<StrModelUA> ParseSrt(string srtPath)
        {
            mySrtModelList = new List<StrModelUA>();
            string line;
            using (FileStream fs = new FileStream(srtPath, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    StringBuilder sb = new StringBuilder();
                    while ((line = sr.ReadLine()) != null)
                    {

                        if (!line.Equals(""))
                        {
                            sb.Append(line).Append("@");
                            continue;
                        }

                        string[] parseStrs = sb.ToString().Split('@');
                        if (parseStrs.Length < 3)
                        {
                            sb.Remove(0, sb.Length);// Clear, otherwise it will affect the analysis of the next subtitle element</i>  
                            continue;
                        }

                        StrModelUA srt = new StrModelUA();
                        string strToTime = parseStrs[1];
                        int beginHour = int.Parse(strToTime.Substring(0, 2));
                        int begin_mintue = int.Parse(strToTime.Substring(3, 2));
                        int begin_scend = int.Parse(strToTime.Substring(6, 2));
                        int begin_milli = int.Parse(strToTime.Substring(9, 3));
                        int beginTime = (beginHour * 3600 + begin_mintue * 60 + begin_scend) * 1000 + begin_milli;

                        int end_hour = int.Parse(strToTime.Substring(17, 2));
                        int end_mintue = int.Parse(strToTime.Substring(20, 2));
                        int end_scend = int.Parse(strToTime.Substring(23, 2));
                        int end_milli = int.Parse(strToTime.Substring(26, 2));
                        int endTime = (end_hour * 3600 + end_mintue * 60 + end_scend) * 1000 + end_milli;

                        srt.BeginTime = beginTime;
                        srt.EndTime = endTime;
                        string strBody = null;
                        for (int i = 2; i < parseStrs.Length; i++)
                        {
                            strBody += parseStrs[i];
                        }
                        srt.SrtString = strBody;
                        mySrtModelList.Add(srt);
                        sb.Remove(0, sb.Length);
                    }
                }

            }
            return mySrtModelList;
        }
        public static string GetTitle(string title)
        {
            return UseRegex(StrHelperUA.GetTimeString(int.Parse(Change(title))));
        }
        public static string UseRegex(string strIn)
        {
            // Replace invalid characters with empty strings.
            strIn = Regex.Replace(strIn, @"<i>", "");

            return Regex.Replace(strIn, @"</i>", "");
        }
        private static string Change(string value)
        {
            string realvalue = "";
            int start = 0;
            int stop = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == ',')
                {
                    start = i + 1;
                    break;
                }
                stop++;
            }
            realvalue = value.Substring(0, stop) + value.Substring(start, 3);
            return realvalue;
        }
    }

    //Define a StrModel class to accept the file format read from the srt file
    public class StrModelUA
    {
        private int beginTime;
        private int endTime;
        private string srtString;
        private int index;
        public int BeginTime { get => beginTime; set => beginTime = value; }
        public int EndTime { get => endTime; set => endTime = value; }
        public string SrtString { get => srtString; set => srtString = value; }
        public int Index { get => index; set => index = value; }
    }
}
