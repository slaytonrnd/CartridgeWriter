Cartridge Writer
================

Cartridge Writer is a Windows program that can use an Arduino to read / write a
DS2433 chip from StrataSys printer cartridges.

The code base is written in c# and consists of a ports of Benjamin Vanheuverzwijn's
stratasys code located at https://github.com/bvanheu/stratasys some of
Matthew Goodman's eepromTool-ds2433 code from https://github.com/meawoppl/eepromTool-ds2433
and the addition of a GUI to make reading and writing a DS2433 chip a point and click
operation.

Dependencies
============
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
	