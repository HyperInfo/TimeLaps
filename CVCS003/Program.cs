using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using OpenCvSharp.Extensions;
using OpenCvSharp;
using CommandLine.Text;

namespace CVCS003
{
    class Program
    {
        public class Options
        {
            [CommandLine.Option('o',DefaultValue="test.avi",HelpText="Output File Name and Directory")]
            public string OutputFile
            {
                get;
                set;
            }
            [CommandLine.Option('f', DefaultValue=12.0, HelpText="FPS of OutputVideo")]
            public double fps
            {
                get;
                set;
            }
            [CommandLine.Option('i', DefaultValue=1,HelpText="Capture Interval. Default is 1sec")]
            public int interval
            {
                get;
                set;
            }
            [CommandLine.Option('z',DefaultValue=1.0,HelpText="Zoom rate 0.1<zoom<1.0 but this does not work")]
            public double zoom
            {
                get;
                set;
            }
            [CommandLine.HelpOption]
            public string GetUsage()
            {
                var usage = new StringBuilder();
                usage.AppendLine("Timelapse 1.0");
                usage.AppendLine(" -o Outputfilename(string) -f fps(double) -i CaptureInterval(int)");

                HelpText help = new HelpText();
               // help.AdditionalNewLineAfterOption = true;
                help.AddOptions(this);

                return help.ToString();
            }

        }
           



        static void Main(string[] args)
        {
            //  CreateCameraCaptureの引数はカメラのIndex(通常は0から始まる)
            using (var capture = Cv.CreateCameraCapture(0))
            {
                /*
                double fps=12.0;
                int interval=1;
                double zoom=1.0;
                string OutputFile;
                */

                double fps ;
                int interval ;
                double zoom=1.0 ;
                string OutputFile;

                var opts = new Options();
                 bool isSuccess = CommandLine.Parser.Default.ParseArguments(args, opts);
               
                if(!isSuccess)
                {
                    opts.GetUsage();
                    Console.WriteLine(Environment.GetCommandLineArgs()[0] + "  -o Outputfilename(string) -f fps(double) -i CaptureInterval(int)");
                    Environment.Exit(0);
                }

                    fps = opts.fps;
                    interval = opts.interval;
                    zoom = opts.zoom;
                    OutputFile = opts.OutputFile;
                    Console.WriteLine(OutputFile);
                    if (fps > 30 | interval < 0.1) 
                    {
                        Console.WriteLine(" :-p");
                        Environment.Exit(1);
                    }

                Int32 codec = 0; // コーデック(AVI)
                IplImage frame = new IplImage();

                /*
                double width = capture.FrameWidth/2;
                double height = capture.FrameHeight/2;

                //double width = 640, height = 240;
                Cv.SetCaptureProperty(capture, CaptureProperty.FrameWidth, width);
                Cv.SetCaptureProperty(capture, CaptureProperty.FrameHeight, height);
                CvSize size = new CvSize((int)width, (int)height);
                CvVideoWriter vw = new CvVideoWriter(OutputFile, codec, fps, size, true);
                */

                
                int width = (int)(Cv.GetCaptureProperty(capture, CaptureProperty.FrameWidth)*zoom);
                int height = (int)(Cv.GetCaptureProperty(capture, CaptureProperty.FrameHeight)*zoom);

                //Cv.SetCaptureProperty(capture, CaptureProperty.FrameWidth, width);
                //Cv.SetCaptureProperty(capture, CaptureProperty.FrameWidth, height);
                //Bitmap bitmap = new Bitmap(width, height);
                
                CvSize size = new CvSize(width, height);
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
                        frame.PutText(str, new CvPoint(10, 20), font, new CvColor(255,0, 255));
                        Cv.ShowImage("Timelapse", frame);
                        //frame.SaveImage("result.bmp");
                       //bitmap = BitmapConverter.ToBitmap(frame);
                        //OpenCvSharp.IplImage ipl2 = (OpenCvSharp.IplImage)BitmapConverter.ToIplImage(bitmap);
                        vw.WriteFrame(frame);
                        // vw.WriteFrame(ipl2);
                        frame.Dispose();
                    }       
                }

                Cv.DestroyWindow("Capture");
                vw.Dispose();
            }

        }
    }
}
