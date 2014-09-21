using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TransformHelper
{
    public class TransformUtils
    {
        public TransformUtils() { }

        public string NormalizeISDN(string ISDN)
        {
            string retStr = "";
            string test = ISDN.ToLower().Replace("-", "");
            string strRegex = @"([0-9]{9}(\d|x|X)\b|[0-9]{12}(\d|x|X)\b)";
            RegexOptions myRegexOptions = RegexOptions.None;
            Regex myRegex = new Regex(strRegex, myRegexOptions);
            //(ISBN[\:\=\s][\s]*(?=[-0-9xX ]{13})(?:[0-9]+[- ]){3}[0-9]*[xX0-9])|(ISBN[\:\=\s][ ]*\d{9,10}[\d|x])/g;
            foreach (Match myMatch in myRegex.Matches(test))
            {
                if (myMatch.Success)
                {
                    retStr += "ISBN:" + myMatch.Groups[1].Value + ", ";
                }
            }
            return retStr;
        }
    }
}