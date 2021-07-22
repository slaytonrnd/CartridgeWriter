# Cartridge Writer for uPrint

Cartridge Writer for uPrint is a Windows program for reading and writing the
DS2433 chip from the printer cartridges of a Stratasys uPrint, uPrint plus, uPrint SE and uPrint SE plus, by just using a serial cable (or a USB to Serial Adapter).

The program is written in C# and consists mostly of the Cartridge Writer program from David Slyton:
https://github.com/slaytonrnd/CartridgeWriter. <br>
Which is based on the tool from Benjamin Vanheuverzwin: https://github.com/bvanheu/stratasys <br>

Requirement is .NET Framework 4.7.2. <br>
So it can run under Windows 7 and upwards.

## Running CartridgeWriter uPrint (SE).exe

CartridgeWriter uPrint (SE).exe can be downloaded as a part of the CartridgeWriter for uPrint repository
and is located in the Release directory of the repository.  

#### Set up the following before running:

- 32 or 64 bit PC with Windows (Only tested on 7)
- At least .Net Framework 4.0
- a null modem serial cable
- for more modern PCs and laptops, which don't have a COM Port, a USB to RS232 adapter
	
#### How to run the Software:
	
1. Connect the DIAG Port on the back of the printer with a null modem cable to your PC or to the serial adapter
2. Run "CartridgeWriter uPrint (SE).exe"
3. Select the serial port of the printer in the Device dropdown.
4. Select the cartridge you want to reset.
5. Press "Read Serial"
6. Check if in the Textbox "received Hex-Code" is some hex code from the Chip of the uPrint.
7. Select a printer type in the "Printer Type" dropdown.
8. Press the "Decrypt" button and remaining fields are populated by reading DS2433 chip.
9. Make changes in "Change Cartridge Values To" column.
10. Press the "generate" Button to generate a new Serial Number, because the printer stores all the serial Numbers, of the already used eeproms.
11. Press the "Write" button to write the changes to the DS2433 chip.

When pressing "Restart" all entries are cleared and the process will start from step 1.

### Settings for Backup:

The program can store backups of the read eeproms as .txt files in a folder called "EEPROMFiles" in the same directory as the program.

To enable it do this step:

1. Check the "save content of EEPROM" checkbox.
2. Every time a cartridge is decrypted while the checkbox is checked, the received content is stored as .txt.

To restore a stored eeprom content do these steps:

1. find the folder of the corresponding UID and copy the content of the txt file with the right date.
2. paste it into the received Hex-Code Box.
3. Continue with step 7 of How to run the software

### Note: 
	
In the standard configuration, the program automatically sets the current quantity to the initial quantity,
so the eeprom is automatically filled up to 100%.

**You can only reset the Eeproms, if the quantity isn't complete down, otherwise the chip is set in the readonly mode and can't be rewritten anymore.**

To avoid this you can increase the "initial quantity" (the "current quantity" should be set automatically to the same value),
so the printer thinks  that there is more filament on the spool, then there actually is. If it now runs out of filament,
it detects this with the sensor in the chip and thinks the filament is broken.
Then you can unload it and load new in. The eeprom is still resetable.

## Compiling Cartridge Writer

The new Version of Cartridge Writer was edited and compiled in Microsoft Visual Studio 2015 with .NET Framework 4.7.2. .


## Updates on 22.07.2020:

- Added uPrint SE compatibility. (Not tested by myself yet.)
- Fixed a bug with the decimal separator of the serial number.
- Added some Error handling when the wrong settings are chosen.
- Added an About and a Help Window.
- Improved the Workflow and cleaned up the code.

# Important Note:

### Some Guys on Ebay try to sell this Reset Solution for a Stratasys uPrint for 100$ which is just this program w/ a slightly different GUI + cable.<br>Such a cable can be bought at an electronic store for around 10$.