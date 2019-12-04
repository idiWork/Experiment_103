using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.System.Threading;

namespace FacialRecognitionBox.Helpers
{
    /// <summary>
    /// Interacts with device GPIO controller in order to control door lock and monitor doorbell
    /// </summary>
    public class GpioHelper
    {
        private static IAsyncAction workItemThread;
        private GpioController gpioController;

        private GpioPin doorBellPin;
        private GpioPin doorLockPin;
        private static GpioPin servoMotorPin;

        /// <summary>
        /// Attempts to initialize Gpio for application. This includes doorbell interaction and locking/unlccking of door.
        /// Returns true if initialization is successful and Gpio can be utilized. Returns false otherwise.
        /// </summary>
        public bool Initialize()
        {
            // Gets the GpioController
            gpioController = GpioController.GetDefault();
            if(gpioController == null)
            {
                // There is no Gpio Controller on this device, return false.
                return false;
            }

            #region BUTTON
            // Opens the GPIO pin that interacts with the doorbel button
            doorBellPin = gpioController.OpenPin(GpioConstants.ButtonPinID);

            if (doorBellPin == null)
            {
                // Pin wasn't opened properly, return false
                return false;
            }

            // Set a debounce timeout to filter out switch bounce noise from a button press
            doorBellPin.DebounceTimeout = TimeSpan.FromMilliseconds(25);

            if (doorBellPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
            {
                // Take advantage of built in pull-up resistors of Raspberry Pi 2 and DragonBoard 410c
                doorBellPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            }
            else
            {
                // MBM does not support PullUp as it does not have built in pull-up resistors 
                doorBellPin.SetDriveMode(GpioPinDriveMode.Input);
            }
            #endregion

            #region LED
            // Opens the GPIO pin that interacts with the door lock system
            doorLockPin = gpioController.OpenPin(GpioConstants.DoorLockPinID);
            if(doorLockPin == null)
            {
                // Pin wasn't opened properly, return false
                return false;
            }
            // Sets doorBell pin drive mode to output as pin will be used to output information to lock
            doorLockPin.SetDriveMode(GpioPinDriveMode.Output);
            // Initializes pin to low voltage. This locks the door. 
            doorLockPin.Write(GpioPinValue.Low);
            #endregion

            #region SERVO
            // Opens the GPIO pin that interacts with the door lock system
            servoMotorPin = gpioController.OpenPin(GpioConstants.ServoMotorPinID);
            if (servoMotorPin == null)
            {
                // Pin wasn't opened properly, return false
                return false;
            }
            // Sets the sesrvo motor pin drive mode to output as pin will be used to output information to lock
            servoMotorPin.SetDriveMode(GpioPinDriveMode.Output);
            // Initializes servo motor pin to low voltage. 
            servoMotorPin.Write(GpioPinValue.Low);

            PWM_L(servoMotorPin.PinNumber);
            #endregion

            //Initialization was successfull, return true
            return true;
        }

        /// <summary>
        /// Returns the GpioPin that handles the doorbell button. Intended to be used in order to setup event handler when user pressed Doorbell.
        /// </summary>
        public GpioPin GetDoorBellPin()
        {
            return doorBellPin;
        }

        /// <summary>
        /// Unlocks door for time specified in GpioConstants class
        /// </summary>
        public async void UnlockDoor()
        {
            // Turn the LED off
            doorLockPin.Write(GpioPinValue.High);
            // Open the door
            PWM_R(servoMotorPin.PinNumber);

            // Wait for specified length
            await Task.Delay(TimeSpan.FromSeconds(GpioConstants.DoorLockOpenDurationSeconds));

            // Close the door
            PWM_L(servoMotorPin.PinNumber);
            // Turn the LED on
            doorLockPin.Write(GpioPinValue.Low);
        }

        public static void PWM_R(int pinNumber)
        {
            var stopwatch = Stopwatch.StartNew();

            workItemThread = Windows.System.Threading.ThreadPool.RunAsync(
                 (source) =>
                 {
                     // setup, ensure pins initialized
                     ManualResetEvent mre = new ManualResetEvent(false);
                     mre.WaitOne(1500);

                     ulong pulseTicks = ((ulong)(Stopwatch.Frequency) / 1000) * 2;
                     ulong delta;
                     var startTime = stopwatch.ElapsedMilliseconds;
                     while (stopwatch.ElapsedMilliseconds - startTime <= 300)
                     {
                         servoMotorPin.Write(GpioPinValue.High);
                         ulong starttick = (ulong)(stopwatch.ElapsedTicks);
                         while (true)
                         {
                             delta = (ulong)(stopwatch.ElapsedTicks) - starttick;
                             if (delta > pulseTicks) break;
                         }
                         servoMotorPin.Write(GpioPinValue.Low);
                         starttick = (ulong)(stopwatch.ElapsedTicks);
                         while (true)
                         {
                             delta = (ulong)(stopwatch.ElapsedTicks) - starttick;
                             if (delta > pulseTicks * 10) break;
                         }
                     }
                 }, WorkItemPriority.High);
        }

        public static void PWM_L(int pinNumber)
        {
            var stopwatch = Stopwatch.StartNew();

            workItemThread = Windows.System.Threading.ThreadPool.RunAsync(
                 (source) =>
                 {
                     // setup, ensure pins initialized
                     ManualResetEvent mre = new ManualResetEvent(false);
                     mre.WaitOne(1500);

                     ulong pulseTicks = ((ulong)(Stopwatch.Frequency) / 1000) * 2;
                     ulong delta;
                     var startTime = stopwatch.ElapsedMilliseconds;
                     while (stopwatch.ElapsedMilliseconds - startTime <= 300)
                     {
                         servoMotorPin.Write(GpioPinValue.High);
                         ulong starttick = (ulong)(stopwatch.ElapsedTicks);
                         while (true)
                         {
                             delta = starttick - (ulong)(stopwatch.ElapsedTicks);
                             if (delta > pulseTicks) break;
                         }
                         servoMotorPin.Write(GpioPinValue.Low);
                         starttick = (ulong)(stopwatch.ElapsedTicks);
                         while (true)
                         {
                             delta = (ulong)(stopwatch.ElapsedTicks) - starttick;
                             if (delta > pulseTicks * 10) break;
                         }
                     }
                 }, WorkItemPriority.High);
        }

    }
}
