using MagicEightBallServiceLib;
using System;
using System.ServiceModel;
using System.Threading;

namespace Legacy.Runner
{
    class Program
    {
        static void Main()
        {
            //Console.WriteLine("Start legacy application");

            //IStockCheck stockCheck = new StockCheck();
            //stockCheck.Monitor(100);

            //Console.WriteLine("End legacy application");

            Console.WriteLine("Starting Service Host . . . ");
            ServiceHost serviceHost = new ServiceHost(typeof(MagicEightBallService));

            try
            {
                // Open the host and start listening for incoming messages.
                serviceHost.Open();
                DisplayHostInfo(serviceHost);
                // Keep the service running 
                Console.WriteLine("The service is ready.");
                Thread.Sleep(Timeout.Infinite);
                // Console.WriteLine("Press the Enter key to terminate service.");
                // Console.ReadLine();
            }
            catch
            {
                serviceHost?.Close();
                Console.WriteLine("The service is closed");
            }


        }


        #region Show all the ABCs exposed from the host.
        static void DisplayHostInfo(ServiceHost host)
        {
            Console.WriteLine();
            Console.WriteLine("***** Host Info *****");

            foreach (System.ServiceModel.Description.ServiceEndpoint se in host.Description.Endpoints)
            {
                Console.WriteLine("Address: {0}", se.Address);
                Console.WriteLine("Binding: {0}", se.Binding.Name);
                Console.WriteLine("Contract: {0}", se.Contract.Name);
            }
            Console.WriteLine("**********************");
        }
        #endregion
    }

}

