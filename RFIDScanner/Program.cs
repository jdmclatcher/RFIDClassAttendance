/*
 * Jonathan McLatcher, Harrison Boyd
 * RFID Class Attendance
 * 2020
 */
using System;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;
using System.Text;
using Unosquare.RaspberryIO.Peripherals;

namespace RFIDScanner
{
	class Program
	{
        static RFIDController RFIDControl;
        static void Main(string[] args)
        {
            Pi.Init<BootstrapWiringPi>();

            RFIDControl = new RFIDController();
            RFIDControl.CardDetected += OnCardDetected;
            /*while (true) {
                byte[] data = Encoding.ASCII.GetBytes("Test");
                byte[] key = {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
                 if (RFIDControl.DetectCard().ToString() == "AllOk") {
                    //RFIDControl.SelectCardUniqueId(RFIDControl.ReadCardUniqueId().Data);
                    var UID = RFIDControl.ReadCardUniqueId().Data;
                    Console.WriteLine($"UID: {UID[0]},{UID[1]},{UID[2]},{UID[3]}");
                    RFIDControl.SelectCardUniqueId(UID);

                    Console.WriteLine(RFIDControl.AuthenticateCard1A(UID, 28));
                    Console.WriteLine(AESEncryption.Text_Encryption("IDK", "LOL", 64));
                    Console.WriteLine(AESEncryption.Text_Encryption("IDK", "LOL", 64));
                    //Console.WriteLine(RFIDControl.CardWriteData(28, Encoding.ASCII.GetBytes("1100301089000000")));
                    Console.WriteLine(Encoding.ASCII.GetString(RFIDControl.CardReadData(28).Data));
                 }
                Thread.Sleep(500);
            }*/
        }

        static void OnCardDetected(object sender, EventArgs e)
        {
            //byte[] key = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            var UID = RFIDControl.ReadCardUniqueId().Data;
            Console.WriteLine($"UID: {UID[0]},{UID[1]},{UID[2]},{UID[3]}");
            RFIDControl.SelectCardUniqueId(UID);

            Console.WriteLine(RFIDControl.AuthenticateCard1A(UID, 28));
            Console.WriteLine(Encoding.ASCII.GetString(RFIDControl.CardReadData(28).Data));
        }
    }
}
