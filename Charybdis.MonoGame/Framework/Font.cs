using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using Charybdis.Library.Core;
using Microsoft.Xna.Framework.Graphics;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace Charybdis.MonoGame
{
    public class Font
    {
        public string Name { get; set; }
        public float Spacing = .25f;
        public float Scale = 1f;
        public float Size { get; set; }
        public int LineHeight { get; set; }
        public char DefaultCharacter = '0';
        private Dictionary<char, OffsetRect> _glyphs = new Dictionary<char, OffsetRect>();
        public string TexturePath { get; private set; }
        public Texture2D Texture { get; set; }
        public Font BackupFont { get; set; }

        public Font(string filename)
        {
            LoadMetrics(filename);
            var fnParts = Path.GetFileNameWithoutExtension(filename).Split('-');
            Name = fnParts[0];
            Size = float.Parse(fnParts[1]);
        }

        public bool HasGlyph(char c)
        {
            return _glyphs.ContainsKey(c);
        }        

        public Vec2 CalculateTextSize(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return Vec2.Zero;
            var lines = s.Split('\n');
            return new Vec2(lines.Max(l => CalculateLineWidth(l)), lines.Length * LineHeight * Scale);
        }

        /*public Vec2 CalculateLineSize(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return Vec2.Zero;
            return new Vec2(CalculateLineWidth(s), CalculateLineHeight(s));
        }*/

        public float CalculateLineWidth(string s)
        {
            return s.Sum(c => _glyphs.ContainsKey(c) ? (_glyphs[c].DestPadding.X * Scale) : (BackupFont != null && BackupFont.HasGlyph(c) ? BackupFont._glyphs[c].DestPadding.X * BackupFont.Scale : 0));
        }

        /*public float CalculateLineHeight(string s)
        {
            return s.Max(c => (_glyphs[c].H + _glyphs[c].DestPadding.Y) * Scale);
        }*/

        public OffsetRect this[char c]
        {
            get
            {
                if (_glyphs.ContainsKey(c))
                    return _glyphs[c];
                else return _glyphs[DefaultCharacter];
            }
            set
            {
                _glyphs[c] = value;
            }
        }

        public OffsetRect this[int i]
        {
            get
            {
                if (_glyphs.ContainsKey((char)i))
                    return _glyphs[(char)i];
                else return _glyphs[DefaultCharacter];
            }
            set
            {
                _glyphs[(char)i] = value;
            }
        }

        public OffsetRect[] this[string s]
        {
            get
            {
                List<OffsetRect> rects = new List<OffsetRect>();
                foreach (char c in s)
                {
                    if (_glyphs.ContainsKey(c))
                        rects.Add(_glyphs[c]);
                    else rects.Add(_glyphs[DefaultCharacter]);
                }
                return rects.ToArray();
            }
        }

        public void LoadMetrics(string filename)
        {
            var ext = Path.GetExtension(filename);
            switch (ext)
            {
                case ".xml":
                    XmlDocument xml = new XmlDocument();
                    xml.Load(filename);
                    var fontMetrics = xml["fontMetrics"];
                    TexturePath = Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar + fontMetrics.Attributes["file"].InnerText;
                    XmlNodeList xchars = fontMetrics.GetElementsByTagName("character");
                    foreach (XmlNode xn in xchars)
                        _glyphs[(char)int.Parse(xn.Attributes["key"].InnerText)] = new OffsetRect(int.Parse(xn["x"].InnerText), int.Parse(xn["y"].InnerText), int.Parse(xn["width"].InnerText), int.Parse(xn["height"].InnerText));
                    if (!_glyphs.ContainsKey('\t')) //If the tab character isn't in the dictionary already..
                        _glyphs.Add('\t', new OffsetRect(0, 0, 0, 0) { DestPadding = new Vec2(_glyphs[' '].W, 0) }); //Add it at 4x the padding width of a space character.
                    return;
                case ".fnt":
                    XmlDocument fnt = new XmlDocument();
                    fnt.Load(filename);
                    var font = fnt["font"];
                    LineHeight = int.Parse(font["common"].Attributes["lineHeight"].InnerText);
                    TexturePath = Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar + font["pages"].FirstChild.Attributes["file"].InnerText;
                    XmlNodeList fchars = font.GetElementsByTagName("char");
                    foreach (XmlNode xn in fchars)
                    {
                        OffsetRect r = new OffsetRect(int.Parse(xn.Attributes["x"].InnerText), int.Parse(xn.Attributes["y"].InnerText), int.Parse(xn.Attributes["width"].InnerText), int.Parse(xn.Attributes["height"].InnerText));
                        r.DestOffset = new Vec2(int.Parse(xn.Attributes["xoffset"].InnerText), int.Parse(xn.Attributes["yoffset"].InnerText));
                        r.DestPadding = new Vec2(int.Parse(xn.Attributes["xadvance"].InnerText), 0);
                        _glyphs[(char)int.Parse(xn.Attributes["id"].InnerText)] = r;
                    }
                    if (!_glyphs.ContainsKey('\t')) //If the tab character isn't in the dictionary already..
                        _glyphs.Add('\t', new OffsetRect(0, 0, 0, 0) { DestPadding = new Vec2(_glyphs[' '].W, 0) }); //Add it at 4x the padding width of a space character.
                    return;
                default:
                    throw new NotImplementedException("There is no handling for \"" + ext + "\".");
            }
        }

        public class Offset
        {
            public int XOffset;
            public int YOffset;
            public int XAdvance;

            public Offset(int xo, int yo, int xa)
            {
                XOffset = xo;
                YOffset = yo;
                XAdvance = xa;
            }
        }
    }
}
