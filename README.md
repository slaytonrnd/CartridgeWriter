Cartridge Writer
================

Cartridge Writer is a Windows program that can use an Arduino to read / write a
DS2433 chip from StrataSys printer cartridges.

The code base is written in c# and consists of ports of Benjamin
Vanheuverzwijn's stratasys code located at:
	https://github.com/bvanheu/stratasys
some of Matthew Goodman's eepromTool-ds2433 code from:
	https://github.com/meawoppl/eepromTool-ds2433
and the addition of a GUI to make reading and writing a DS2433 chip a point and
click operation.

---------------------------------------
Dependencies to run CartridgeWriter.exe
---------------------------------------
	-32 or 64 bit PC with Windows XP or later (Only tested on XP and 7 so far)
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
			
	-At least .Net Framework 4.0
	
----------------------------------------
Dependencies to compile Cartridge Writer
----------------------------------------
Cartridge Writer was written and compiled using Microsoft Visual Studio Express 2013
for Windows Desktop.  Given that that Express is the base flavor of Visual Studio
this code should compile with any other flavor of Visual Studio 2013 that can compile
c# desktop applications.  This has not been tested with any other versions of Visual
Studio, but if anyone tries compiling with a different version let me know what
happens and I can post the results here.
	-Visual Studio 2013
	