using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screeps3D.RoomObjects.Season2
{
    public static class Season2Constants
    {
        public static Color32 SymbolToColor(string symbol)
        {
            /*
                symbol_aleph: 0xC63946,
                symbol_beth: 0xB72E6F,
                symbol_gimmel: 0xB72FA5,
                symbol_daleth: 0xA334B7,
                symbol_he: 0x9D41ED,
                symbol_waw: 0x8441ED,
                symbol_zayin: 0x6E49FF,
                symbol_heth: 0x4E71FF,
                symbol_teth: 0x5088F4,
                symbol_yodh: 0x3DA1EA,
                symbol_kaph: 0x38A9C7,
                symbol_lamedh: 0x35B7B5,
                symbol_mem: 0x36B79A,
                symbol_nun: 0x33B75D,
                symbol_samekh: 0x3FB147,
                symbol_ayin: 0x69A239,
                symbol_pe: 0x7EA232,
                symbol_tsade: 0x9FA23B,
                symbol_qoph: 0xBB933A,
                symbol_res: 0xD88942,
                symbol_sim: 0xDC763D,
                symbol_taw: 0xD64B3D
            */
            Color newCol;
            switch (symbol)
            {
                case "symbol_aleph": ColorUtility.TryParseHtmlString("#C63946", out newCol); break;
                case "symbol_beth": ColorUtility.TryParseHtmlString("#B72E6F", out newCol); break;
                case "symbol_gimmel": ColorUtility.TryParseHtmlString("#B72FA5", out newCol); break;
                case "symbol_daleth": ColorUtility.TryParseHtmlString("#A334B7", out newCol); break;
                case "symbol_he": ColorUtility.TryParseHtmlString("#9D41ED", out newCol); break;
                case "symbol_waw": ColorUtility.TryParseHtmlString("#8441ED", out newCol); break;
                case "symbol_zayin": ColorUtility.TryParseHtmlString("#6E49FF", out newCol); break;
                case "symbol_heth": ColorUtility.TryParseHtmlString("#4E71FF", out newCol); break;
                case "symbol_teth": ColorUtility.TryParseHtmlString("#5088F4", out newCol); break;
                case "symbol_yodh": ColorUtility.TryParseHtmlString("#3DA1EA", out newCol); break;
                case "symbol_kaph": ColorUtility.TryParseHtmlString("#38A9C7", out newCol); break;
                case "symbol_lamedh": ColorUtility.TryParseHtmlString("#35B7B5", out newCol); break;
                case "symbol_mem": ColorUtility.TryParseHtmlString("#36B79A", out newCol); break;
                case "symbol_nun": ColorUtility.TryParseHtmlString("#33B75D", out newCol); break;
                case "symbol_samekh": ColorUtility.TryParseHtmlString("#3FB147", out newCol); break;
                case "symbol_ayin": ColorUtility.TryParseHtmlString("#69A239", out newCol); break;
                case "symbol_pe": ColorUtility.TryParseHtmlString("#7EA232", out newCol); break;
                case "symbol_tsade": ColorUtility.TryParseHtmlString("#9FA23B", out newCol); break;
                case "symbol_qoph": ColorUtility.TryParseHtmlString("#BB933A", out newCol); break;
                case "symbol_res": ColorUtility.TryParseHtmlString("#D88942", out newCol); break;
                case "symbol_sim": ColorUtility.TryParseHtmlString("#DC763D", out newCol); break;
                case "symbol_taw": ColorUtility.TryParseHtmlString("#D64B3D", out newCol); break;
                default:
                    newCol = new Color32(222, 0, 0, 0); break;
            }
            return newCol;
        }
    }
}
