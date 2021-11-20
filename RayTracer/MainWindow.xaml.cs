using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using Charybdis.Library.Core;
using Rect = System.Windows.Rect;

namespace Charybdis.RayTracer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //static ulong _totalUpdates = 0;
        //static ulong _updates = 0;
        //static ulong _fps = 0;
        bool _testPattern = false;
        string _logFilePath = "log.txt";
        public MainWindow()
        {
            InitializeComponent();
            _target = new Rect(0d, 0d, RenderSize.Width, RenderSize.Height);
            _tracer.World = new Array2<Col3>(_worldWidth, _worldHeight);
            for (int y = 0; y < _worldHeight; y++)
                for (int x = 0; x < _worldWidth; x++)
                {
                    Col3 c = Col3.MagicPink;
                    if (x == 0) //Left Wall
                        c = Col3.Red;
                    else if (x == _worldWidth - 1) //Right Wall
                        c = Col3.Yellow;
                    if (y == 0) //Top Wall
                        c = Col3.Green;
                    else if (y == _worldHeight - 1) //Bottom Wall
                        c = Col3.Blue;
                    _tracer.World.Set(x, y, c);
                }
            ImgControl.Source = _writeableBitmap;
            ImgControl.Stretch = Stretch.Fill;
            RenderOptions.SetBitmapScalingMode(ImgControl, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(ImgControl, EdgeMode.Aliased);
            Task mainThread = null;
            mainThread = Tasking.Do(() =>
                {
                    while (true)
                    {
                        //_updates++;
                        try
                        {
                            //Map
                            //string map = "__| G G G G G G G G G G |__ \n";
                            //for (int y = 1; y < _worldHeight - 1; y+=10)
                            //{
                            //    if (_camera.Position.Yi >= y && _camera.Position.Yi < y + 10)
                            //    {
                            //        string line = "|R| ";
                            //        for (int x = 1; x < _worldWidth - 1; x+=10)
                            //        {
                            //            if (_camera.Position.Xi >= x && _camera.Position.Xi < x + 10)
                            //                line += "X ";
                            //            else
                            //                line += "_ ";
                            //        }
                            //        map += line + "|Y|\n";
                            //    }
                            //    else
                            //    {
                            //        map += "|R| _ _ _ _ _ _ _ _ _ _ |Y|\n";
                            //    }
                            //}
                            //map += "``| B B B B B B B B B B |``";
                            Dispatcher.Invoke(() =>
                            {
                                //sbiUpdates.Content = _fps + "FPS (Updates: " + _updates + ", Total: " + _totalUpdates + ")";
                                sbiPosition.Content = "Position: " + _camera.Position.ToString();
                                sbiFacing.Content = "Facing: " + _camera.Facing.ToString();
                                sbiThread.Content = "Thread Status: " + mainThread.Status.ToString();
                                //TxtMap.Text = map;
                                _writeableBitmap.Lock();
                                unsafe
                                {
                                    /*int y = rnd.Next(RayTracer.VIRTUAL_HEIGHT);
                                    Col3 randomColor = new Col3((float)(rnd.NextDouble() * 255), (float)(rnd.NextDouble() * 255), (float)(rnd.NextDouble() * 255));
                                    for (int x = 0; x < RayTracer.VIRTUAL_WIDTH; x++)
                                        ((Pixel*)writeableBitmap.BackBuffer)[RayTracer.VIRTUAL_WIDTH * y + x].SetColor(randomColor);*/
                                    if (_testPattern)
                                    {
                                        for (int x = 0; x < RayTracer.VIRTUAL_WIDTH; x++)
                                            for (int y = 0; y < RayTracer.VIRTUAL_HEIGHT; y++)
                                                ((Pixel*)_writeableBitmap.BackBuffer)[RayTracer.VIRTUAL_WIDTH * y + x].SetColor(new Col3(x, 0, y));
                                    }
                                    else
                                        _tracer.Trace((Pixel*)_writeableBitmap.BackBuffer, _camera.Position, _camera.Facing, _viewDistance);
                                }
                                _writeableBitmap.AddDirtyRect(_wholeBitmapRect);
                                _writeableBitmap.Unlock();
                            });
                        }
                        catch (TaskCanceledException)
                        {
                            Console.WriteLine("Main thread cancelled.");
                            /*Pass*/
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                }
            );
            //FPS
            //new System.Threading.Timer(o =>
            //{
            //    System.Diagnostics.Debug.WriteLine("FPS Timer Executed.");
            //    _fps = _updates;
            //    _totalUpdates += _fps;
            //    _updates = 0;
            //}, null, 0, 1000);
        }

        Int32Rect _wholeBitmapRect = new Int32Rect(0, 0, RayTracer.VIRTUAL_WIDTH, RayTracer.VIRTUAL_HEIGHT);
        WriteableBitmap _writeableBitmap = new WriteableBitmap(RayTracer.VIRTUAL_WIDTH, RayTracer.VIRTUAL_HEIGHT, 96, 96, PixelFormats.Rgb24, null);
        int _worldWidth = 100;
        int _worldHeight = 100;
        float _viewDistance = 101;
        Camera _camera = new Camera(new Vec2(1, 99), 100);
        Tracer _tracer = new Tracer();// { LoggingFilePath = "log.txt" };
        PixelFormat _pixelFormat = PixelFormats.Rgb24;
        Rect _target;
        Random _rnd = new Random();
        bool _fast = false;
        
        //public unsafe void SetPixel(int x, int y, Col3 color)
        //{
        //    tracer.Target[RayTracer.VIRTUAL_WIDTH * y + x].SetColor(color);
        //}

        //public void SetRow(int y, Col3 color)
        //{
        //    for (int x = 0; x < RayTracer.VIRTUAL_WIDTH; x++)
        //        SetPixel(x, y, color);
        //}

        //public void SetColumn(int x, Col3 color)
        //{
        //    for (int y = 0; y < RayTracer.VIRTUAL_HEIGHT; y++)
        //        SetPixel(x, y, color);
        //}

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_camera == null)
                return;
            if (e.Key == Key.Left || e.Key == Key.A)
                _camera.Facing = _camera.Facing.Wrap(_fast ? -10 : -1, 0, 360);
            if (e.Key == Key.Right || e.Key == Key.D)
                _camera.Facing = _camera.Facing.Wrap(_fast ? 10 : 1, 0, 360);
            if (e.Key == Key.Up || e.Key == Key.W)
            {
                var newPosition = _camera.Position.AngularMovement(_camera.Facing, _fast ? 10 : 1);
                if (newPosition.X > 0 && newPosition.X < _worldWidth)
                    _camera.Position.X = newPosition.X;
                if (newPosition.Y > 0 && newPosition.Y < _worldHeight)
                    _camera.Position.Y = newPosition.Y;
            }
            if (e.Key == Key.Down || e.Key == Key.S)
            {
                var newPosition = _camera.Position.AngularMovement(_camera.Facing, _fast ? -10 : -1);
                if (newPosition.X > 0 && newPosition.X < _worldWidth)
                    _camera.Position.X = newPosition.X;
                if (newPosition.Y > 0 && newPosition.Y < _worldHeight)
                    _camera.Position.Y = newPosition.Y;
            }
            if (e.Key == Key.F)
                _fast = !_fast;
            if (e.Key == Key.T)
                _testPattern = !_testPattern;
            //if (e.Key == Key.L)
            //{
            //    if (System.IO.File.Exists(_logFilePath))
            //        System.IO.File.Delete(_logFilePath);
            //    _tracer.LogFrame();
            //}
            if (e.Key == Key.Escape)
                App.Current.Shutdown();
        }
    }
}