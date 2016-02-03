Cartridge Writer
================

Note:	I have not had time to work on this repository in over a year, it has not
	been updated with the latest code from https://github.com/bvanheu/stratasys
	and does not appear to work with uprint cartridges.  I am hesitant to give
	a timeline on when I will update the repository, since I am not sure when I
	will get a chance to work on it.  For now if you have a uprint please see
	Vanheuverzwijn's stratasys code located at:
		https://github.com/bvanheu/stratasys
	And Matthew Goodman's eepromTool-ds2433 code from:
		https://github.com/meawoppl/eepromTool-ds2433

Cartridge Writer is a Windows program that can use an Arduino to read / write a
DS2433 chip from Stratasys printer cartridges.

The code base is written in c# and consists of ports of Benjamin
Vanheuverzwijn's stratasys code located at:
	https://github.com/bvanheu/stratasys
some of Matthew Goodman's eepromTool-ds2433 code from:
	https://github.com/meawoppl/eepromTool-ds2433
and the addition of a GUI to make reading and writing a DS2433 chip a point and
click operation.

Running CartridgeWriter.exe
----
CartridgeWriter.exe can be downloaded as a part of the CartridgeWriter repository
and is located in the Release directory of the repository.  

Set up the following before running:

	-32 or 64 bit PC with Windows XP or later (Only tested on XP and 7 so far)
	-At least .Net Framework 4.0
	-Arduino configured as shown at https://github.com/meawoppl/eepromTool-ds2433
		* OneWire library from http://www.pjrc.com/teensy/td_libs_OneWire.html
		* onewireProxy from https://github.com/meawoppl/eepromTool-ds2433
		* Wire Arduino to DS2433 chip as shown:
		
			Arduino:	Data Pin		5V			Ground
						  ---			---			 ---
						   |			 |			  |
						   |   2.2 kOhm	 |			  |
						   |----/\/\/----|			  |
						   |						  |
						   |						  |
						  ---						 ---
			DS2433:		Data Pin					Ground

Plug in the Arduino and wait for it to be recognized by Windows before running
CartridgeWriter.exe.

	-Modify values in CartridgeWriter.exe.config if necessary (see note below).
		* BaudRate: Default value is 9600 
		* SerialInitializationWait: Default value is 2000 (Wait for Serial Port to Open) 
	-Run CartridgeWriter.exe
	-Select an Arduino device in the Device dropdown.
	-Select a Printer Type in the Printer Type dropdown.
	-Press the Read button and remaining fields are populated by reading DS2433 chip.
		* Read makes a copy of the DS2433 chip contents to a file.
		* Content file is placed in EEPROMFiles/<chip_id> directory.
		* Content file's name is YYYYMMdd.HHmmss.bin.
	-Make changes in "Change Cartridge Values To" column.
	-Press the Write button to write changes to DS2433 chip.

Note on CartridgeWriter.exe.config values:
The default values selected are very conservative, but may not work in all cases.
Kulitorum reported being able to push the BaudRate to 115200 on an Arduino Mega
1280 (See Issues #3 & #4), but I haven't tried anything but 9600 on my Arduino Micro.
Kulitorum also reported needing to set the SerialInitializationWait very high (3000)
for the Arduino Mega 1280, but with the Arduino Micro no wait is needed at all.
If CartridgeWriter appears to hang when trying to read try setting the
SerialInitializationWait value higher.

Compiling Cartridge Writer
----
Cartridge Writer was written and compiled using Microsoft Visual Studio Express 2013
for Windows Desktop Update 4.  Given that that Express is the base flavor of Visual
Studio this code should compile with any other flavor of Visual Studio 2013 that can
compile c# desktop applications.  This has not been tested with any other versions
of Visual Studio, but if anyone tries compiling with a different version let me know
what happens and I can post the results here.

	-Visual Studio 2013
	
