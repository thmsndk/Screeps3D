using Screeps_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts.Screeps_API.ConsoleClientAbuse
{
    public class FontColor : IConsoleClientAbuse
    {
        public void Abuse(ScreepsConsole.ConsoleMessage message)
        {
            // find font tag
            // <font color=\"#ff5555\" type=\"highlight\">console test string</font>
            string fontPattern = @"<font (?:color=\""(?<color>.+?)\"")?\s*?(?:type=\""(?<type>.+?)\"")?\s*?>(?<text>.+?)<\/font>";
            foreach (Match m in Regex.Matches(message, fontPattern))
            {
                var hexColor = m.Groups["color"].Value;
                var type = m.Groups["type"].Value;
                var text = m.Groups["text"].Value;

                //Color messageColor;
                //ColorUtility.TryParseHtmlString(color, out messageColor);
                message.Message = message.Message.Replace(m.Value, string.Format("<color={0}>{1}</color>", hexColor, text));
            }
        }
    }
}
