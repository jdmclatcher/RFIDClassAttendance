/*
 * Jonathan McLatcher, Harrison Boyd
 * RFID Class Attendance
 * 2020
 */
using System;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;

namespace RFIDScanner
{
    class Program
	{

        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly RFIDController rfidControl = new RFIDController();
        static void Main(string[] args)
        {
            Pi.Init<BootstrapWiringPi>();
            rfidControl.CardDetected += OnCardDetected;
        }

        static async void OnCardDetected(object sender, EventArgs e)
        {
            var UID = rfidControl.ReadCardUniqueId().Data;
            rfidControl.SelectCardUniqueId(UID);

            Console.WriteLine(rfidControl.AuthenticateCard1A(UID, 28));
            String StudentID = Encoding.ASCII.GetString(rfidControl.CardReadData(28).Data);
            Console.WriteLine(StudentID);
            Dictionary<string, string> JSON = new Dictionary<string, string>
            {
                { "studentID", StudentID }
            };
            HttpResponseMessage response = await httpClient.PostAsync("localhost:44316/API/api", new FormUrlEncodedContent(JSON));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("NO");
            }
        }
    }
}
