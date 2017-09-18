using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ServerPart
{
    public partial class Form1 : Form
    {
        static int index = 0;
        static string pathFile = @"E:\test\";
        static string nameFile = "test";
        static string extentionFile = ".nst";
        static string fileFullName;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                fileFullName = pathFile + nameFile + index++ + extentionFile;
                File.WriteAllText(fileFullName, textBox1.Text);

                
                MessageBox.Show("Your file is save");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(pathFile))
            {
                Directory.CreateDirectory(pathFile);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (fileFullName != "" || fileFullName != null)
            {
                try
                {
                    SendMessageFromSocket(5656);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    Console.ReadLine();
                }
            }
        }
        private static void SendMessageFromSocket(int port)
        {
            // Буфер для входящих данных
            byte[] bytes = new byte[1024];

            // Соединяемся с удаленным устройством

            // Устанавливаем удаленную точку для сокета
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = IPAddress.Parse("10.11.30.32");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);

            byte[] fileNameByte = Encoding.ASCII.GetBytes(nameFile);

            byte[] fileData = File.ReadAllBytes(fileFullName);
            byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
            byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);

            fileNameLen.CopyTo(clientData, 0);
            fileNameByte.CopyTo(clientData, 4);
            fileData.CopyTo(clientData, 4 + fileNameByte.Length);


           

            // Отправляем данные через сокет
            int bytesSent = sender.Send(clientData);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);

            MessageBox.Show("\nОтвет от сервера: {0}\n\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));

                 
            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }
}

