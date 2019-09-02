using Screeps_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts.Screeps_API.ConsoleClientAbuse
{
    public class StyleTags : IConsoleClientAbuse
    {
        public void Abuse(ScreepsConsole.ConsoleMessage message)
        {
            // we use <color to color the font in TMP
            // we use <mark to simulate an alpha colored background color
            // `<log severity="1" style="color: yellow; background-color: red">[1] log level yellow and bg color</log>
            string stylePattern = @"<.*?(?:style=[\""'](?<style>.+?)[\""']).*?>(?<text>.+?)<\/.+?>";

            string propertyPattern = @"((?<cssProperty>.+?):\s?(?<cssValue>\w+|#[0-9a-fA-F]{6});?)";
            foreach (Match m in Regex.Matches(message, stylePattern))
            {
                var style = m.Groups["style"].Value;
                var text = m.Groups["text"].Value;

                string fontColor = null;
                string backgroundColor = null;
                string colorizedMessage = text;

                foreach (Match cssStyle in Regex.Matches(style, propertyPattern))
                {
                    var cssProperty = cssStyle.Groups["cssProperty"].Value.Trim();
                    var cssValue = cssStyle.Groups["cssValue"].Value.Trim();

                    switch (cssProperty)
                    {
                        case "color":
                            fontColor = cssValue;

                            if (!fontColor.StartsWith("#"))
                            {
                                var hexColor = ColorToHex.Parse(cssValue);
                                fontColor = hexColor != null ? hexColor.Hex : cssValue;
                            }

                            break;
                        case "background-color":
                            backgroundColor = cssValue;
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(fontColor))
                {
                    colorizedMessage = string.Format("<color={1}>{0}</color>", colorizedMessage, fontColor);
                }

                if (!string.IsNullOrEmpty(backgroundColor))
                {
                    // https://www.rapidtables.com/web/color/Yellow_Color.html
                    // according to TMP documentation, theese are the only supported color names
                    switch (backgroundColor)
                    {
                        case "black":
                            backgroundColor = "#000000";
                            break;
                        case "blue":
                            backgroundColor = "#0000FF";
                            break;
                        case "green":
                            backgroundColor = "#008000";
                            break;
                        case "orange":
                            backgroundColor = "#FFA500";
                            break;
                        case "purple":
                            backgroundColor = "#800080";
                            break;
                        case "red":
                            backgroundColor = "#FF0000";
                            break;
                        case "white":
                            backgroundColor = "#FFFFFF";
                            break;
                        case "yellow":
                            backgroundColor = "#FFFF00";
                            break;
                    }

                    if (backgroundColor.StartsWith("#"))
                    {
                        // http://digitalnativestudios.com/textmeshpro/docs/rich-text/
                        // mark needs opacity
                        backgroundColor += "22";
                    }

                    colorizedMessage = string.Format("<mark={1}>{0}</mark>", colorizedMessage, backgroundColor);
                }

                //Color messageColor;
                //ColorUtility.TryParseHtmlString(color, out messageColor);

                message.Message = message.Message.Replace(m.Value, colorizedMessage);
            }
        }
    }
}
