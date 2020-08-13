using System;
using System.Linq;
using System.Threading.Tasks;
using Device.Net;
using Device.Net.Windows;
using Usb.Net;
using Usb.Net.Windows;

namespace GarminUsbMassStorageMode
{
    class Program
    {
        static async Task Main()
        {
            try
            {
                await Run();
            }
            catch (Exception e)
            {
                Console.WriteLine("***** Unexpected exception *****");
                Console.WriteLine(e);
            }
        }

        private static async Task Run()
        {
            var logger = new DebugLogger();
            var tracer = new DebugTracer();

            DeviceManager.Current.DeviceFactories.Add(new CustomWindowsUsbDeviceFactory(WindowsDeviceConstants.GUID_DEVINTERFACE_USB_DEVICE, logger, tracer));

            var devices = await DeviceManager.Current.GetConnectedDeviceDefinitionsAsync(null);
            ConnectedDeviceDefinition garminDefinition = devices.SingleOrDefault(device => device.VendorId == 0x091E && device.ProductId == 0x0003);
            if (garminDefinition == null)
            {
                Console.WriteLine("No connected Garmin device found");
                return;
            }
            var usbInterfaceManager = new WindowsUsbInterfaceManager
            (
                garminDefinition.DeviceId,
                logger,
                tracer,
                null,
                null
            );

            var usbDevice = new UsbDevice(garminDefinition.DeviceId, usbInterfaceManager, logger, tracer);

            await usbDevice.InitializeAsync();

            var firstUsbInterface = usbDevice.UsbInterfaceManager.UsbInterfaces.First();
            usbInterfaceManager.WriteUsbInterface = firstUsbInterface;
            firstUsbInterface.WriteEndpoint = firstUsbInterface.UsbInterfaceEndpoints.First(e => e.IsWrite);

            var writeBuffer = Unhexify("140000002f0400000100000000");
            await usbDevice.WriteAsync(writeBuffer);

            Console.WriteLine("Command sent");
        }

        private static byte[] Unhexify(string hex)
        {
            return Enumerable
                .Range(0, hex.Length / 2)
                .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16))
                .ToArray();
        }
    }

    public class CustomWindowsUsbDeviceFactory : WindowsUsbDeviceFactory
    {
        private readonly Guid classGuid;

        public CustomWindowsUsbDeviceFactory(Guid classGuid, ILogger logger, ITracer tracer) : base(logger, tracer)
        {
            this.classGuid = classGuid;
        }

        protected override Guid GetClassGuid()
        {
            return classGuid;
        }
    }
}