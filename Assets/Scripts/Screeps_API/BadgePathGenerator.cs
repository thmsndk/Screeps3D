using System;
using System.Collections.Generic;
using Screeps3D;
using UnityEngine;

namespace Screeps_API
{
    internal class BadgePathGenerator
    {
        private Dictionary<int, Action<SvgParams>> _algorithms = new Dictionary<int, Action<SvgParams>>();

        internal BadgePathGenerator()
        {
            _algorithms[1] = svg =>
            {
                float vert = 0,
                    hor = 0;

                if (svg.param > 0)
                    vert = svg.param * 30 / 100;
                else if (svg.param < 0)
                    hor = -svg.param * 30 / 100;

                svg.path1 = FormattableString.Invariant($"M 50 {100 - vert} L {hor} 50 H {100 - hor} Z");
                svg.path2 = FormattableString.Invariant($"M {hor} 50 H {100 - hor} L 50 {vert} Z");
                svg.rotation = 0;
            };
            _algorithms[2] = svg =>
            {
                float x = 0,
                    y = 0;

                if (svg.param > 0)
                    x = svg.param * 30 / 100;
                else if (svg.param < 0)
                    y = -svg.param * 30 / 100;

                svg.path1 = FormattableString.Invariant($"M {x} {y} L 50 50 L {100 - x} {y} V -1 H -1 Z");
                svg.path2 = FormattableString.Invariant($"M {x} {100 - y} L 50 50 L {100 - x} {100 - y} V 101 H -1 Z");
                svg.rotation = 0;
            };
            _algorithms[3] = svg =>
            {
                float angle = Mathf.PI / 4 + Mathf.PI / 4 * (svg.param + 100) / 200,
                    angle1 = -Mathf.PI / 2,
                    angle2 = Mathf.PI / 2 + Mathf.PI / 3,
                    angle3 = Mathf.PI / 2 - Mathf.PI / 3;


                svg.path1 = FormattableString.Invariant(
                    $"M 50 50 L {50 + 100 * Mathf.Cos(angle1 - angle / 2)} {50 + 100 * Mathf.Sin(angle1 - angle / 2)} L {50 + 100 * Mathf.Cos(angle1 + angle / 2)} {50 + 100 * Mathf.Sin(angle1 + angle / 2)} Z");

                svg.path2 = FormattableString.Invariant(
                    $"M 50 50 L {50 + 100 * Mathf.Cos(angle2 - angle / 2)} {50 + 100 * Mathf.Sin(angle2 - angle / 2)} L {50 + 100 * Mathf.Cos(angle2 + angle / 2)} {50 + 100 * Mathf.Sin(angle2 + angle / 2)} Z M 50 50 L {50 + 100 * Mathf.Cos(angle3 - angle / 2)} {50 + 100 * Mathf.Sin(angle3 - angle / 2)} L {50 + 100 * Mathf.Cos(angle3 + angle / 2)} {50 + 100 * Mathf.Sin(angle3 + angle / 2)}");
                svg.rotation = 180;
            };
            _algorithms[4] = svg =>
            {
                svg.param += 100;
                float y1 = 50 - svg.param * 30 / 200, y2 = 50 + svg.param * 30 / 200;
                svg.path1 = FormattableString.Invariant($"M 0 {y2} H 100 V 100 H 0 Z");
                svg.path2 = svg.param > 0 ? FormattableString.Invariant($"M 0 {y1} H 100 V {y2} H 0 Z") : "";
                svg.rotation = 90;
            };
            _algorithms[5] = svg =>
            {
                svg.param += 100;
                float x1 = 50 - svg.param * 10 / 200 - 10, x2 = 50 + svg.param * 10 / 200 + 10;
                svg.path1 = FormattableString.Invariant($"M {x1} 0 H {x2} V 100 H {x1} Z");
                svg.path2 = FormattableString.Invariant($"M 0 {x1} H 100 V {x2} H 0 Z");
                svg.rotation = 45;
            };
            _algorithms[6] = svg =>
            {
                float width = 5 + (svg.param + 100) * 8 / 200,
                    x1 = 50,
                    x2 = 20,
                    x3 = 80;

                svg.path1 = FormattableString.Invariant($"M {x1 - width} 0 H {x1 + width} V 100 H {x1 - width}");
                svg.path2 = FormattableString.Invariant(
                    $"M {x2 - width} 0 H {x2 + width} V 100 H {x2 - width} Z M {x3 - width} 0 H {x3 + width} V 100 H {x3 - width} Z");
                svg.rotation = 90;
            };
            _algorithms[7] = svg =>
            {
                var w = 20 + svg.param * 10 / 100;
                svg.path1 = "M 0 50 Q 25 30 50 50 T 100 50 V 100 H 0 Z";
                svg.path2 = FormattableString.Invariant(
                    $"M 0 {50 - w} Q 25 {30 - w} 50 {50 - w} T 100 {50 - w} V {50 + w} Q 75 {70 + w} 50 {50 + w} T 0 {50 + w} Z");
                svg.rotation = 90;
            };
            _algorithms[8] = svg =>
            {
                var y = svg.param * 20 / 100;
                svg.path1 = "M 0 50 H 100 V 100 H 0 Z";
                svg.path2 = FormattableString.Invariant($"M 0 50 Q 50 {y} 100 50 Q 50 {100 - y} 0 50 Z");

                svg.rotation = 90;
            };
            _algorithms[9] = svg =>
            {
                float y1 = 0,
                    y2 = 50,
                    h = 70;

                if (svg.param > 0)
                    y1 += svg.param / 100 * 20;
                else if (svg.param < 0)
                    y2 += svg.param / 100 * 30;

                svg.path1 = FormattableString.Invariant($"M 50 {y1} L 100 {y1 + h} V 101 H 0 V {y1 + h} Z");
                svg.path2 = FormattableString.Invariant(
                    $"M 50 {y1 + y2} L 100 {y1 + y2 + h} V 101 H 0 V {y1 + y2 + h} Z");
                svg.rotation = 180;
            };
            _algorithms[10] = svg =>
            {
                float r = 30,
                    d = 7;

                if (svg.param > 0)
                    r += svg.param * 50 / 100;
                else if (svg.param < 0)
                    d -= svg.param * 20 / 100;

                svg.path1 = FormattableString.Invariant(
                    $"M {50 + d + r} {50 - r} A {r} {r} 0 0 0 {50 + d + r} {50 + r} H 101 V {50 - r} Z");
                svg.path2 = FormattableString.Invariant(
                    $"M {50 - d - r} {50 - r} A {r} {r} 0 0 1 {50 - d - r} {50 + r} H -1 V {50 - r} Z");
                svg.rotation = 90;
            };
            _algorithms[11] = svg =>
            {
                float a1 = 30,
                    a2 = 30,
                    x = 50 - 50 * Mathf.Cos(Mathf.PI / 4),
                    y = 50 - 50 * Mathf.Sin(Mathf.PI / 4);
                if (svg.param > 0)
                {
                    a1 += svg.param * 25 / 100;
                    a2 += svg.param * 25 / 100;
                }
                else if (svg.param < 0)
                {
                    a2 -= svg.param * 50 / 100;
                }

                svg.path1 = FormattableString.Invariant(
                    $"M {x} {y} Q {a1} 50 {x} {100 - y} H 0 V {y} Z M {100 - x} {y} Q {100 - a1} 50 {100 - x} {100 - y} H 100 V {y} Z");
                svg.path2 = FormattableString.Invariant(
                    $"M {x} {y} Q 50 {a2} {100 - x} {y} V 0 H {x} Z M {x} {100 - y} Q 50 {100 - a2} {100 - x} {100 - y} V 100 H {x} Z");
                svg.rotation = 90;
            };
            _algorithms[12] = svg =>
            {
                float a1 = 30,
                    a2 = 35;

                if (svg.param > 0)
                    a1 += svg.param * 30 / 100;
                else if (svg.param < 0)
                    a2 += svg.param * 15 / 100;

                svg.path1 = FormattableString.Invariant($"M 0 {a1} H 100 V 100 H 0 Z");
                svg.path2 = FormattableString.Invariant(
                    $"M 0 {a1} H {a2} V 100 H 0 Z M 100 {a1} H {100 - a2} V 100 H 100 Z");
                svg.rotation = 180;
            };
            _algorithms[13] = svg =>
            {
                float r = 30,
                    d = 0;

                if (svg.param > 0)
                    r += svg.param * 50 / 100;
                else if (svg.param < 0)
                    d -= svg.param * 20 / 100;

                svg.path1 = "M 0 0 H 50 V 100 H 0 Z";
                svg.path2 = FormattableString.Invariant(
                    $"M {50 - r} {50 - d - r} A {r} {r} 0 0 0 {50 + r} {50 - r - d} V 0 H {50 - r} Z");

                svg.rotation = 180;
            };
            _algorithms[14] = svg =>
            {
                float a = Mathf.PI / 4,
                    d = 0;
                a += svg.param * Mathf.PI / 4 / 100;
                svg.path1 = FormattableString.Invariant(
                    $"M 50 0 Q 50 {50 + d} {50 + 50 * Mathf.Cos(a)} {50 + 50 * Mathf.Sin(a)} H 100 V 0 H 50 Z");
                svg.path2 = FormattableString.Invariant(
                    $"M 50 0 Q 50 {50 + d} {50 - 50 * Mathf.Cos(a)} {50 + 50 * Mathf.Sin(a)} H 0 V 0 H 50 Z");

                svg.rotation = 180;
            };
            _algorithms[15] = svg =>
            {
                float w = 13 + svg.param * 6 / 100,
                    r1 = 80,
                    r2 = 45,
                    d = 10;
                svg.path1 = FormattableString.Invariant(
                    $"M {50 - r1 - w} {100 + d} A {r1 + w} {r1 + w} 0 0 1 {50 + r1 + w} {100 + d} H {50 + r1 - w} A {r1 - w} {r1 - w} 0 1 0 {50 - r1 + w} {100 + d}");
                svg.path2 = FormattableString.Invariant(
                    $"M {50 - r2 - w} {100 + d} A {r2 + w} {r2 + w} 0 0 1 {50 + r2 + w} {100 + d} H {50 + r2 - w} A {r2 - w} {r2 - w} 0 1 0 {50 - r2 + w} {100 + d}");

                svg.rotation = 180;
            };
            _algorithms[16] = svg =>
            {
                float a = 30 * Mathf.PI / 180,
                    d = 25;
                if (svg.param > 0)
                    a += 30 * Mathf.PI / 180 * svg.param / 100;
                else if (svg.param < 0)
                    d += svg.param * 25 / 100;

                svg.path1 = "";
                for (var i = 0; i < 3; i++)
                {
                    float angle1 = i * Mathf.PI * 2 / 3 + a / 2 - Mathf.PI / 2,
                        angle2 = i * Mathf.PI * 2 / 3 - a / 2 - Mathf.PI / 2;
                    svg.path1 += FormattableString.Invariant(
                        $"M {50 + 100 * Mathf.Cos(angle1)} {50 + 100 * Mathf.Sin(angle1)} L {50 + 100 * Mathf.Cos(angle2)} {50 + 100 * Mathf.Sin(angle2)} L {50 + d * Mathf.Cos(angle2)} {50 + d * Mathf.Sin(angle2)} A {d} {d} 0 0 1 {50 + d * Mathf.Cos(angle1)} {50 + d * Mathf.Sin(angle1)} Z");
                }

                svg.path2 = "";
                for (var i = 0; i < 3; i++)
                {
                    float angle1 = i * Mathf.PI * 2 / 3 + a / 2 + Mathf.PI / 2,
                        angle2 = i * Mathf.PI * 2 / 3 - a / 2 + Mathf.PI / 2;
                    svg.path2 += FormattableString.Invariant(
                        $"M {50 + 100 * Mathf.Cos(angle1)} {50 + 100 * Mathf.Sin(angle1)} L {50 + 100 * Mathf.Cos(angle2)} {50 + 100 * Mathf.Sin(angle2)} L {50 + d * Mathf.Cos(angle2)} {50 + d * Mathf.Sin(angle2)} A {d} {d} 0 0 1 {50 + d * Mathf.Cos(angle1)} {50 + d * Mathf.Sin(angle1)} Z");
                }

                svg.rotation = 0;
            };
            _algorithms[17] = svg =>
            {
                float w = 35, h = 45;
                if (svg.param > 0)
                    w += svg.param * 20 / 100;
                else if (svg.param < 0)
                    h -= svg.param * 30 / 100;

                svg.path1 = FormattableString.Invariant($"M 50 45 L {50 - w} {h + 45} H {50 + w} Z");
                svg.path2 = FormattableString.Invariant($"M 50 0 L {50 - w} {h} H {50 + w} Z");
                svg.rotation = 0;
            };
            _algorithms[18] = svg =>
            {
                float a = 90 * Mathf.PI / 180, d = 10;
                if (svg.param > 0)
                    a -= 60 / 180 * Mathf.PI * svg.param / 100;
                else if (svg.param < 0)
                    d -= svg.param * 15 / 100;

                svg.path1 = "";
                svg.path2 = "";
                for (var i = 0; i < 3; i++)
                {
                    float angle1 = Mathf.PI * 2 / 3 * i + a / 2 - Mathf.PI / 2,
                        angle2 = Mathf.PI * 2 / 3 * i - a / 2 - Mathf.PI / 2;
                    string path = FormattableString.Invariant(
                        $"M {50 + 100 * Mathf.Cos(angle1)} {50 + 100 * Mathf.Sin(angle1)} L {50 + 100 * Mathf.Cos(angle2)} {50 + 100 * Mathf.Sin(angle2)} L {50 + d * Mathf.Cos((angle1 + angle2) / 2)} {50 + d * Mathf.Sin((angle1 + angle2) / 2)} Z");
                    if (i == 0)
                    {
                        svg.path1 += path;
                    }
                    else
                    {
                        svg.path2 += path;
                    }
                }

                svg.rotation = 180;
            };
            _algorithms[19] = svg =>
            {
                float w2 = 20,
                    w1 = 60;
                w1 += svg.param * 20 / 100;
                w2 += svg.param * 20 / 100;
                svg.path1 = FormattableString.Invariant($"M 50 -10 L {50 - w1} 100 H {50 + w1} Z");
                svg.path2 = "";
                if (w2 > 0)
                {
                    svg.path2 = FormattableString.Invariant($"M 50 0 L {50 - w2} 100 H {50 + w2} Z");
                }

                svg.rotation = 180;
            };
            _algorithms[20] = svg =>
            {
                float w = 10,
                    h = 20;

                if (svg.param > 0)
                    w += svg.param * 20 / 100;
                else if (svg.param < 0)
                    h += svg.param * 40 / 100;

                svg.path1 = FormattableString.Invariant($"M 0 {50 - h} H {50 - w} V 100 H 0 Z");
                svg.path2 = FormattableString.Invariant($"M {50 + w} 0 V {50 + h} H 100 V 0 Z");
                svg.rotation = 90;
            };
            _algorithms[21] = svg =>
            {
                float w = 40,
                    h = 50;

                if (svg.param > 0)
                    w -= svg.param * 20 / 100;
                else if (svg.param < 0)
                    h += svg.param * 20 / 100;

                svg.path1 = FormattableString.Invariant(
                    $"M 50 {h} Q {50 + w} 0 50 0 T 50 {h} Z M 50 {100 - h} Q {50 + w} 100 50 100 T 50 {100 - h} Z");
                svg.path2 = FormattableString.Invariant(
                    $"M {h} 50 Q 0 {50 + w} 0 50 T {h} 50 Z M {100 - h} 50 Q 100 {50 + w} 100 50 T {100 - h} 50 Z");

                svg.rotation = 45;
            };
            _algorithms[22] = svg =>
            {
                float w = 20;
                w += svg.param * 10 / 100;
                svg.path1 = FormattableString.Invariant($"M {50 - w} {50 - w} H {50 + w} V {50 + w} H {50 - w} Z");
                svg.path2 = "";
                for (var i = -4; i < 4; i++)
                {
                    for (var j = -4; j < 4; j++)
                    {
                        var a = (i + j) % 2;
                        svg.path2 += FormattableString.Invariant(
                            $"M {50 - w - w * 2 * i} {50 - w - w * 2 * (j + a)} h {-w * 2} v {w * 2} h {w * 2} Z");
                    }
                }

                svg.rotation = 45;
            };
            _algorithms[23] = svg =>
            {
                float w = 17,
                    h = 25;

                if (svg.param > 0)
                    w += svg.param * 35 / 100;
                else if (svg.param < 0)
                    h -= svg.param * 23 / 100;

                svg.path1 = "";
                for (var i = -4; i <= 4; i++)
                {
                    svg.path1 +=
                        FormattableString.Invariant(
                            $"M {50 - w * i * 2} {50 - h} l {-w} {-h} l {-w} {h} l {w} {h} Z");
                }

                svg.path2 = "";
                for (var i = -4; i <= 4; i++)
                {
                    svg.path2 +=
                        FormattableString.Invariant(
                            $"M {50 - w * i * 2} {50 + h} l {-w} {-h} l {-w} {h} l {w} {h} Z");
                }

                svg.rotation = 90;
            };
            _algorithms[24] = svg =>
            {
                float w = 50,
                    h = 45;
                if (svg.param > 0)
                    w += svg.param * 60 / 100;
                else if (svg.param < 0)
                    h += svg.param * 30 / 100;

                svg.path1 = FormattableString.Invariant($"M 0 {h} L 50 70 L 100 {h} V 100 H 0 Z");
                svg.path2 = FormattableString.Invariant(
                    $"M 50 0 L {50 + w} 100 H 100 V {h} L 50 70 L 0 {h} V 100 H {50 - w} Z");
                svg.rotation = 180;
            };
        }

        internal void Add(JSONObject badge, SvgParams badgeParams)
        {
            if (badge["type"].IsNumber)
            {
                Generate((int) badge["type"].n, badgeParams);
            }
            else
            {
                Assign(badge["type"], badgeParams);
            }
        }

        private void Assign(JSONObject jsonObject, SvgParams badgeParams)
        {
            badgeParams.path1 = jsonObject["path1"].str;
            badgeParams.path2 = jsonObject["path2"].str;
        }

        private void Generate(int type, SvgParams badgeParams)
        {
            var algorithm = _algorithms[1];
            if (_algorithms.ContainsKey(type))
            {
                algorithm = _algorithms[type];
            }

            algorithm(badgeParams);
        }
    }
}