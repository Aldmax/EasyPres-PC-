using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using WindowsInput;
using WindowsInput.Native;
using System.Globalization;

namespace Serv
{
    static class Program
    {
        // Порт
        static int port = 10000;
        // Адрес
        static IPAddress ipAddress = IPAddress.Parse("0.0.0.0");
        // Оправить сообщение
        const byte codeMsg = 1;
        // Повернуть экран
        const byte codeRotate = 2;
        // Выключить компьютер
        const byte codePoff = 3;
        // Повышает громкость
        const byte codeVolumeUp = 4;
        // Понимажает громкость
        const byte codeVolumeDown = 5;
        // Повышает яркость
        const byte codeBriUp = 6;
        // Понижает яркость
        const byte codeBriDown = 7;
        // Предыдущий слайд
        const byte codePrevSlide = 8;
        // Следующий слайд
        const byte codeNextSlide = 9;
        // Эмуляция левой кнопки мыши
        const byte codeLKM = 10;
        // Эмуляция правой кнопки мыши
        const byte codePKM = 11;
        // Включить управление курсором
        const byte codeMouse = 12;
        // Начало презентации с первого слайда
        const byte codePlayFirst = 13;
        // Начало презентации с текущего слайда
        const byte codePlay = 14;
        // Выход из режима презентации
        const byte codeStopPres = 15;
        // Временное скрытие слайда черным экраном
        const byte codeStopShow = 16;

        [STAThread]

        static void Main()
        {
            // Создаем локальную конечную точку
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            // Создаем сокет
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            VolumeCtrl volU = new VolumeCtrl();
            //vold.VolDown();
            volU.SetVolume(30);
            BrightnessCtrl gammaUpd = new BrightnessCtrl();
            gammaUpd.SetBri(150);
            int newVolume = 30;
            int newBrig = 150;
            try
            {
                // Связываем сокет с конечной точкой
                socket.Bind(ipEndPoint);
                // Переходим в режим "прослушивания" соединения
                socket.Listen(1);
                // Ждем соединение. При удачном соедиениее создается новый экземпляр socket, связанный с этим соединением
                while (true)
                {
                    Socket handler = socket.Accept();
                    // Массив, где сохраняем приняятые данные.
                    byte[] recBytes = new byte[1024];
                    int nBytes = handler.Receive(recBytes);
                    float[] coord = new float[2];
                    switch (recBytes[0])    // Определяемся с командами клиента
                    {
                        case codeMsg:
                            nBytes = handler.Receive(recBytes); // Читаем данные сообщения
                            if (nBytes != 0)
                            {
                                String msg = Encoding.UTF8.GetString(recBytes, 0, nBytes);
                                MessageBox.Show(msg, "msg from EasyPres");
                            }
                            break;
                        case codePoff:
                            System.Diagnostics.Process ofc = new System.Diagnostics.Process();
                            ofc.StartInfo.FileName = "shutdown.exe";
                            ofc.StartInfo.Arguments = "-s -t 00";
                            ofc.Start();
                            socket.Close();
                            break;
                        case codeVolumeUp:
                            newVolume += 5;
                            volU.SetVolume(newVolume);
                            break;
                        case codeVolumeDown:
                            newVolume -= 5;
                            volU.SetVolume(newVolume);
                            break;
                        case codeBriUp:
                            if (newBrig < 200)
                            {
                                newBrig += 5;
                                gammaUpd.SetBri(newBrig);
                            }
                            break;
                        case codeBriDown:
                            if (newBrig > 0)
                            {
                                newBrig -= 5;
                                gammaUpd.SetBri(newBrig);
                            }
                            break;
                        case codePrevSlide:
                            SendKey sndP = new SendKey();
                            sndP.SendP();
                            break;
                        case codeNextSlide:
                            SendKey sndN = new SendKey();
                            sndN.SendN();
                            //SendKeys.Send("N");
                            break;
                        case codeLKM:
                            MouseOperations LKM = new MouseOperations();
                            LKM.MouseEventLKM();
                            break;
                        case codePKM:
                            
                            break;
                        case codeMouse:
                            nBytes = handler.Receive(recBytes); // Читаем данные сообщения
                            if (nBytes != 0)
                            {
                                String Cooooord = Encoding.UTF8.GetString(recBytes, 0, nBytes);
                                //Console.WriteLine(Cooooord);
                                string[] word = Cooooord.Split('/');

                                //coord[0] = Convert.ToSingle(word[0], new CultureInfo("en-US"));
                                //coord[1] = Convert.ToSingle(word[1], new CultureInfo("en-US"));
                                float.TryParse(word[0], NumberStyles.Any, new CultureInfo("en-US"), out coord[0]);
                                float.TryParse(word[1], NumberStyles.Any, new CultureInfo("en-US"), out coord[1]);
                                //coord[0] = float.Parse(word[0]);
                                //coord[1] = float.Parse(word[1]);
                                MouseOperations moveMouse = new MouseOperations();
                                moveMouse.MouseMove(coord[0], coord[1]);
                                //MessageBox.Show(Cooooord+"-----"+coord[0]+"!!!"+coord[1].GetType(), "Ичпочмак");
                            }
                            //if (coord > 9)
                            //{
                            //    Console.WriteLine();
                                    //MessageBox.Show("Ичпочмак");
                            //}
                            break;
                        case codePlayFirst:
                            SendKey sndF5 = new SendKey();
                            sndF5.SendF5();
                            //SendKeys.Send("{F5}");
                            break;
                        case codePlay:
                            SendKey sndShF5 = new SendKey();
                            sndShF5.SendShF5();
                            //SendKeys.Send("+{F5}");
                            break;
                        case codeStopPres:
                            SendKey sndEsc = new SendKey();
                            sndEsc.SendEsc();
                            //SendKeys.Send("{ESC}");
                            break;
                        case codeStopShow:
                            SendKey sndB = new SendKey();
                            sndB.SendB();
                            //SendKeys.Send("B");
                            break;
                    }
                    // Освобождаем сокеты
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }


            }
            catch (Exception ex)
            {
            }

        }
    }

    public class VolumeCtrl
    {
        public void SetVolume(int level)
        {
            try
            {
                //Instantiate an Enumerator to find audio devices
                NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
                //Get all the devices, no matter what condition or status
                NAudio.CoreAudioApi.MMDeviceCollection DevCol = MMDE.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.All, NAudio.CoreAudioApi.DeviceState.All);
                //Loop through all devices
                foreach (NAudio.CoreAudioApi.MMDevice dev in DevCol)
                {
                    try
                    {
                        if (dev.State == NAudio.CoreAudioApi.DeviceState.Active)
                        {
                            var newVolume = (float)Math.Max(Math.Min(level, 100), 0) / (float)100;

                            //Set at maximum volume
                            dev.AudioEndpointVolume.MasterVolumeLevelScalar = newVolume;

                            dev.AudioEndpointVolume.Mute = level == 0;

                            //Get its audio volume
                            Console.WriteLine("Volume of " + dev.FriendlyName + " is " + dev.AudioEndpointVolume.MasterVolumeLevelScalar.ToString());
                        }
                        else
                        {
                            Console.WriteLine("Ignoring device " + dev.FriendlyName + " with state " + dev.State);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Do something with exception when an audio endpoint could not be muted
                        Console.WriteLine(dev.FriendlyName + " could not be muted with error " + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                //When something happend that prevent us to iterate through the devices
                Console.WriteLine("Could not enumerate devices due to an excepion: " + ex.Message);
            }
        }
    }

    public class SendKey
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public void SendP()
        {
            System.Diagnostics.Process[] p = System.Diagnostics.Process.GetProcessesByName("POWERPNT"); //поиск процесса Power Point
            if (p.Length > 0) //проверка найдено ли окно
            {
                SetForegroundWindow(p[0].MainWindowHandle); //делаем окно Power Point активным
            }

            SendKeys.SendWait("p"); //нажатие клавиши "p" для Power Point
        }
        public void SendN()
        {
            System.Diagnostics.Process[] p = System.Diagnostics.Process.GetProcessesByName("POWERPNT"); //поиск процесса Power Point
            if (p.Length > 0) //проверка найдено ли окно
            {
                SetForegroundWindow(p[0].MainWindowHandle); //делаем окно Power Point активным
            }

            SendKeys.SendWait("n"); //нажатие клавиши "n" для Power Point
        }
        public void SendB()
        {
            System.Diagnostics.Process[] p = System.Diagnostics.Process.GetProcessesByName("POWERPNT"); //поиск процесса Power Point
            if (p.Length > 0) //проверка найдено ли окно
            {
                SetForegroundWindow(p[0].MainWindowHandle); //делаем окно Power Point активным
            }

            SendKeys.SendWait("b"); //нажатие клавиши "b" для Power Point
    }
        public void SendEsc()
        {
            System.Diagnostics.Process[] p = System.Diagnostics.Process.GetProcessesByName("POWERPNT"); //поиск процесса Power Point
            if (p.Length > 0) //проверка найдено ли окно
            {
                SetForegroundWindow(p[0].MainWindowHandle); //делаем окно Power Point активным
            }

            SendKeys.SendWait("{ESC}"); //нажатие клавиши "esc" для Power Point
    }
        public void SendF5()
        {
            System.Diagnostics.Process[] p = System.Diagnostics.Process.GetProcessesByName("POWERPNT"); //поиск процесса Power Point
            if (p.Length > 0) //проверка найдено ли окно
            {
                SetForegroundWindow(p[0].MainWindowHandle); //делаем окно Power Point активным
            }

            SendKeys.SendWait("{F5}"); //нажатие клавиши "esc" для Power Point
    }
        public void SendShF5()
        {
            System.Diagnostics.Process[] p = System.Diagnostics.Process.GetProcessesByName("POWERPNT"); //поиск процесса Power Point
            if (p.Length > 0) //проверка найдено ли окно
            {
                SetForegroundWindow(p[0].MainWindowHandle); //делаем окно Power Point активным
            }

            SendKeys.SendWait("+{F5}"); //нажатие клавиши "esc" для Power Point
        }
    }

    class BrightnessCtrl
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);

        [DllImport("gdi32.dll")]
        public static extern int SetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RAMP
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Red;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Green;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Blue;
        }

        public int Value
        {
            get
            {
                return _Value;
            }

            set
            {
                this.SetBri(value);
                _Value = value;
            }
        }

        private int _Value = 128;

        public void SetBri(int Value)
        {
            IntPtr DC = GetDC(GetDesktopWindow());

            if (DC != null)
            {

                RAMP _Rp = new RAMP();

                _Rp.Blue = new ushort[256];
                _Rp.Green = new ushort[256];
                _Rp.Red = new ushort[256];

                for (int i = 1; i < 256; i++)
                {
                    int value = i * (Value + 128);

                    if (value > 65535)
                        value = 65535;

                    _Rp.Red[i] = _Rp.Green[i] = _Rp.Blue[i] = Convert.ToUInt16(value);
                }

                SetDeviceGammaRamp(DC, ref _Rp);
            }
        }
    }

    public class MouseOperations
    {
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        public static void MouseEvent(MouseEventFlags value)
        {
            MousePoint position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }

        public void MouseMove(float a, float b)
        {
            int CursorX = Cursor.Position.X;
            int CursorY = Cursor.Position.Y;

            if (a < -2 && b < -2)
            {
                SetCursorPos(CursorX + 3, CursorY-3);
            } else if (a > 2 && b > 2)
            {
                SetCursorPos(CursorX - 3, CursorY + 3);
            } else if (a < -2 && b > 2)
            {
                SetCursorPos(CursorX + 3, CursorY + 3);
            } else if (a > 2 && b < -2)
            {
                SetCursorPos(CursorX - 3, CursorY - 3);
            } else if (a > 2)
            {
                SetCursorPos(CursorX - 3, CursorY);
            } else if (a < -2)
            {
                SetCursorPos(CursorX + 3, CursorY);
            } else if (b > 2)
            {
                SetCursorPos(CursorX, CursorY + 3);
            } else if (b < -2)
            {
                SetCursorPos(CursorX, CursorY - 3);
            }
        }

        public void MouseEventPKM()
        {
            MouseEvent(MouseEventFlags.RightDown);
        }

        public void MouseEventLKM()
        {
            MouseEvent(MouseEventFlags.LeftDown);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
