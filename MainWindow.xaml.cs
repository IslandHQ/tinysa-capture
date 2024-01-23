using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace tinysa_capture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ImageCapture _imageCapture;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonOpen_OnClick(object sender, RoutedEventArgs e)
        {
            _imageCapture = new ImageCapture("COM3");
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            _imageCapture.Close();
        }

        private void ButtonCapture_OnClick(object sender, RoutedEventArgs e)
        {
            Img.Source = _imageCapture.Capture();
        }
    }

    public class ImageCapture
    {
        private readonly SerialPort _serial;

        public ImageCapture(string portName, int baudRate = 115200)
        {
            try
            {
                _serial = new SerialPort(portName, baudRate);
                _serial.Open();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("シリアルポートを開けませんでした。", ex);
            }
        }

        public void Close()
        {
            if (_serial != null && _serial.IsOpen)
            {
                _serial.Close();
            }
        }

        public BitmapSource Capture()
        {
            try
            {
                _serial.WriteLine("capture\r");
                byte[] buffer = new byte[360 * 240 * 2];
                _serial.Read(buffer, 0, buffer.Length);

                int width = 320;
                int height = 240;
                int stride = width * 2;

                WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr565, null);
                bitmap.WritePixels(new Int32Rect(0,0,width,height),buffer,stride,0);
                return bitmap;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("データの読み取り中にエラーが発生しました。", ex);
            }
        }
    }
}