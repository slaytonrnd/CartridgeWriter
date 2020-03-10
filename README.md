# Cartridge Writer for uPrint

Cartridge Writer for uPrint is a Windows program for reading and writing the
DS2433 chip from the printer cartridges of a Stratasys uPrint, uPrint plus and uPrint SE, by just using a serial cable.

The program is written in C# and consits mostly of the Cartridge Writer program from David Slyton:
https://github.com/slaytonrnd/CartridgeWriter
Which is based on the tool from Benjamin Vanheuverzwin:
	https://github.com/bvanheu/stratasys

## Running CartridgeWriter.exe
CartridgeWriter.exe can be downloaded as a part of the CartridgeWriter for uPrint repository
and is located in the Release directory of the repository.  

#### Set up the following before running:

	- 32 or 64 bit PC with Windows (Only tested on 7)
	- At least .Net Framework 4.0
	- a null modem serial cable
	- for more modern PCs and laptops, which don't have a COM Port, a USB to Serial Adapter
	
#### How to run the Software:
	
	- Connect the DIAG Port on the back of the Printer with a Null Modem Cable to your PC or to the Serial Adapter
	- Run CartridgeWriter.exe
	- Select the Printer in the Device dropdown.
	- Select the Cartride you want to reset.
	- Press "Read Serial"
	- Check if in the Textbox "received Hex-Code" is some receives Hex code from the Chip of the uPrint.
	- Select a Printer Type in the "Printer Type" dropdown.
	- Press the "Decrypt" button and remaining fields are populated by reading DS2433 chip.
	- Make changes in "Change Cartridge Values To" column.
	- Press the "generate" Button to generate a new Serial Number, because the printer stores all the serial Numbers,
	  of the already used eeproms.
	- Press the Create button to write the changes to the DS2433 chip.

### Note: 
	
In the standart configuration, the programm automatically sets the current qunatity to the inital quantity,
so the eeprom is automatically filled up to 100%.

**You can only reset the Eeproms, if the quantity isn't complete down, otherwise the chip is set in the readonly mode and can't be rewritten anymore.**

To avoid this you can increase the "initial quantity" (the "current quantity" should be set automatically to the same value),
so the printer thinks  that there is more filament on the spool, then there actually is. If it now runs out of filament,
it detects this with the sensor in the chip and thinks the filament is broken.
Then you can unload it and load new in. The eeprom is still resetable.

## Compiling Cartridge Writer
The new Version of Cartridge Writer was edited and compiled in Microsoft Visual Studio 2015.	
