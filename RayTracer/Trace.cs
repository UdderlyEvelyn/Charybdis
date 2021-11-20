using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using System.Runtime.InteropServices;
using System.IO;

namespace Charybdis.RayTracer
{
    public static class RayTracer
    {
        public const int VIRTUAL_WIDTH = 320;
        public const int VIRTUAL_HEIGHT = 240;
        public const int VIRTUAL_PIXELS = VIRTUAL_WIDTH * VIRTUAL_HEIGHT;
        public const int BYTES_PER_PIXEL = 3;
        public const int BUFFER_BYTES = VIRTUAL_WIDTH * VIRTUAL_HEIGHT * BYTES_PER_PIXEL;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Pixel
    {
        [FieldOffset(0)]
        public byte R;
        [FieldOffset(1)]
        public byte G;
        [FieldOffset(2)]
        public byte B;

        public void SetColor(Col3 c)
        {
            R = (byte)c.R;
            G = (byte)c.G;
            B = (byte)c.B;
        }

        public static Pixel FromCol3(Col3 c)
        {
            return new Pixel
            {
                R = (byte)c.R,
                G = (byte)c.G,
                B = (byte)c.B,
            };
        }
    }

    public class Camera
    {
        public double Facing = 0;
        public Vec2 Position = Vec2.Zero;

        public Camera(Vec2 position, double facing)
        {
            Position = position;
            Facing = facing;
        }

        public Camera(Vec2 position) : this(position, 0)
        {

        }
    }

    //Designed for 24-bit pixels, i.e. 3 byte RGB.
    public unsafe class Tracer
    {
        public Array2<Col3> World;
        public double HorizontalFOV = 70;
        public double VerticalFOV = 50;
        double _hDegPerX = 0;
        double _vDegPerY = 0;
        Col3 fogColor = Col3.Black;
        int halfWidth = RayTracer.VIRTUAL_WIDTH / 2;
        bool _logging = false;
        //public string LoggingFilePath = "";
        //StreamWriter _writer;

        public Tracer()
        {
            _hDegPerX = (HorizontalFOV / RayTracer.VIRTUAL_WIDTH);
            _vDegPerY = (VerticalFOV / RayTracer.VIRTUAL_HEIGHT);
        }

        public void Trace(Pixel* target, Vec2 position, double facing, float viewDistance)
        {
            //if (_logging)
            //    _writer = File.AppendText(LoggingFilePath);

            for (int x = 0; x < RayTracer.VIRTUAL_WIDTH; x++)
            {
                
                //_log("X: " + x);
                var adjustedX = x - halfWidth;
                //_log("Adjusted X: " + adjustedX);
                //_log("*");
                Col3 c = Col3.White;
                var hDeg = _hDegPerX * adjustedX;
                //_log("Horizontal Degrees Per X: " + _hDegPerX);
                //_log("=" + hDeg);
                //_log("+");
                //_log("Facing: " + facing);
                var angle = hDeg + facing;
                //_log("=" + angle + " (Wrapped: " + facing.Wrap(hDeg, 0, 360) + ")");
                var rayDirection = position.AngularMovement(angle, 1);
                //_log("Ray Direction: " + rayDirection);
                for (int i = 0; i < viewDistance; i++)
                {
                    //_log("I: " + i);
                    var slice = i == 1 ? rayDirection : position.AngularMovement(angle, i);
                    //_log("Slice: " + slice);
                    //_log("In Range: " + World.InRange(slice.Xi, slice.Yi));
                    if (!World.InRange(slice.Xi, slice.Yi))
                    {
                        c = fogColor;
                        break;
                    }
                    c = World.Get(slice.Xi, slice.Yi);
                    //_log("Actual Color In Array: " + c);
                    if (c != Col3.MagicPink)
                    {
                        var multiplier = 1 - (i / viewDistance);
                        if (multiplier != 1)
                            c *= multiplier;
                        break;
                    }
                    else c = fogColor;
                }
                //Set whole column's color.
                //_log("Final Color Applied To Column " + x + ": " + c);
                for (int y = 0; y < RayTracer.VIRTUAL_HEIGHT; y++)
                    target[RayTracer.VIRTUAL_WIDTH * y + x].SetColor(c);
            }

            //if (_logging)
            //{
            //    _logging = false;
            //    _writer.Close();
            //}
        }

        //private void _log(string text, bool line = true)
        //{
        //    if (_logging)
        //    {
        //        if (line)
        //            _writer.WriteLine(text);
        //        else
        //            _writer.Write(text);
        //    }
        //}

        //public void LogFrame()
        //{
        //    if (LoggingFilePath.HasContent())
        //        _logging = true;
        //    //else throw an Exception, probably..
        //}
    }
}
