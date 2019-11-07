﻿using HtmlAgilityPack;
using System;
using System.Text;
using System.Security.Cryptography;
namespace TradeFollowLiteForex
{
    public class Util
    {
        public static string MD5(string str)
        {
            byte[] originalBytes;
            byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original string and compute hash
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(str);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            string hashed = BitConverter.ToString(encodedBytes).Replace("-", "").ToLower();
            return hashed;
        }
        public static string GetSubstringByString(string text, string a, string b)
        {
            try
            {
                if (text != "")
                {
                    int begin_a = text.IndexOf(a);
                    return text.Substring((begin_a + a.Length), (text.IndexOf(b, begin_a + a.Length) - begin_a - a.Length)).Trim();
                }
            }
            catch (Exception)
            {
                return text;
            }
            return text;
        }
        public static string EscapeJson(string input)
        {
            return input.Replace("&#39;", "%27").Replace(":", "%3A").Replace("[", "%5B").Replace("{", "%7B").Replace("]", "%5D").Replace("}", "%7D").Replace(",", "%2C").Replace("&amp;", "&").Replace(" ", "%20");
        }

        public static string EscapeDataString(string input)
        {
            return Uri.EscapeDataString(input);
        }

        public static string UnescapeDataString(string input)
        {
            return Uri.UnescapeDataString(input);
        }
        public static string HtmlGetAttributeValue(string content, string attributeName, string xpath)
        {
            string ret = "";
            try
            {
                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                html.OptionFixNestedTags = true;
                html.LoadHtml(content);
                HtmlNode node = html.DocumentNode.SelectSingleNode(xpath);
                ret = node.Attributes[attributeName].Value;
            }
            catch (Exception)
            {
                ret = "";
            }
            return ret;
        }

        public static string HtmlGetInnerText(string content, string xpath)
        {
            string ret = "";
            try
            {
                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                html.OptionFixNestedTags = true;
                html.LoadHtml(content);
                HtmlNode node = html.DocumentNode.SelectSingleNode(xpath);
                ret = node.InnerText;
            }
            catch (Exception)
            {
                ret = "";
            }
            return ret;
        }

        public static string HtmlGetInnerText(string content, string xpath, string split)
        {
            string ret = "";
            try
            {
                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                html.OptionFixNestedTags = true;
                html.LoadHtml(content);
                HtmlNodeCollection nodes = html.DocumentNode.SelectNodes(xpath);

                foreach (HtmlNode node in nodes)
                {
                    ret += node.InnerText + split;
                }
            }
            catch (Exception)
            {
                ret = "";
            }
            return ret;
        }

        public static HtmlNodeCollection HtmlGetNodeCollection(string content, string xpath)
        {
            HtmlNodeCollection ret = null;
            try
            {
                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                html.OptionFixNestedTags = true;
                html.LoadHtml(content);
                ret = html.DocumentNode.SelectNodes(xpath);
            }
            catch (Exception)
            {
                ret = null;
            }
            return ret;
        }

    }
}
