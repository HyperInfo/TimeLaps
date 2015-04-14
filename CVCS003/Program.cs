using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using OpenCvSharp.Extensions;
using OpenCvSharp;

namespace CVCS003
{
    class Program
    {
        public class Options
        {
            [CommandLine.Option('o',DefaultValue="test.avi")]
            public string OutputFile
            {
                get;
                set;
            }
            [CommandLine.Option('f', DefaultValue=12.0)]
            public double fps
            {
                get;
                set;
            }
            [CommandLine.Option('i', DefaultValue=1)]
            public int interval
            {
                get;
                set;
            }
            [CommandLine.Option('z',DefaultValue=0.5)]
            public double zoom
            {
                get;
                set;
            }
        }
           



        static void Main(string[] args)
        {
            //  CreateCameraCaptureの引数はカメラのIndex(通常は0から始まる)
            using (var capture = Cv.CreateCameraCapture(0))
            {
                double fps;
                int interval;
                double zoom;
                string OutputFile;

                var opts = new Options();
                 bool isSuccess = CommandLine.Parser.Default.ParseArguments(args, opts);
               
                if(!isSuccess)
                {
                    Console.WriteLine("argument error ;-p");
                    Environment.Exit(0);
                }

                    fps = opts.fps;
                    interval = opts.interval;
                    zoom = opts.zoom;
                    OutputFile = opts.OutputFile;
                
                  
                
                IplImage frame = new IplImage();
               
                //  W320 x H240のウィンドウを作る
                //double w = 320, h = 240;
                //Cv.SetCaptureProperty(capture, CaptureProperty.FrameWidth, w);
                //Cv.SetCaptureProperty(capture, CaptureProperty.FrameHeight, h);
                int width = (int)(Cv.GetCaptureProperty(capture, CaptureProperty.FrameWidth)*zoom);
                int height = (int)(Cv.GetCaptureProperty(capture, CaptureProperty.FrameHeight)*zoom);
                Bitmap bitmap = new Bitmap(width, height);
                                //aviファイル設定
              
                int codec = 0; // コーデック(AVI)
                CvSize size = new CvSize(width, height);

                //Cv.CreateVideoWriter("test.avi", codec, fps, size, true);
                CvVideoWriter vw = new CvVideoWriter(OutputFile, codec, fps, size, true);

                CvFont font = new CvFont(FontFace.HersheyComplex, 0.7, 0.7);

                //  何かキーを押すまでは、Webカメラの画像を表示し続ける
                while (Cv.WaitKey(1) == -1)
                {
                    System.Threading.Thread.Sleep(1000*interval);
                    //  カメラからフレームを取得
                    frame = Cv.QueryFrame(capture);
                    string str = DateTime.Now.ToString();
                    
                    //  Window「Capture」を作って、Webカメラの画像を表示
                    if (frame != null)
                    {
                        frame.PutText(str, new CvPoint(10, 20), font, new CvColor(0, 255, 100));
                        Cv.ShowImage("Capture", frame);
                        //frame.SaveImage("result.bmp");
                       bitmap = BitmapConverter.ToBitmap(frame);
                        OpenCvSharp.IplImage ipl2 = (OpenCvSharp.IplImage)BitmapConverter.ToIplImage(bitmap);
                        vw.WriteFrame(ipl2);
                        frame.Dispose();
                    }       
                }
                //  使い終わったWindow「Capture」を破棄
                Cv.DestroyWindow("Capture");
                vw.Dispose();
            }

        }
    }
}
