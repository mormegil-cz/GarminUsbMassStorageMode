Enabling USB Mass Storage Mode on Garmin Vívomove HR
======

When a Garmin Vívomove HR smartwatch is connected to a Windows computer via USB, it is installed as a “Garmin device” (“Garmin USB GPS”).

If you want to access data on the watch, you need to switch it to a USB mass storage mode. This is a simple tool to do exactly that. It works by sending a specific command on the USB bulk out interface of the watch.

For the program to work, you need to replace the default Garmin driver with a generic WinUSB driver. For that, you can use e.g. [Zadig](https://zadig.akeo.ie/). After that, just run the program. The USB device will reconnect in USB mass storage mode.

The implementation is copied from [Python implementation by Leberwurscht](https://github.com/Leberwurscht/vivosmart). Thanks a lot!!
