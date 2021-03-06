DesktopSwitcher (for windows)
By: David Schraitle
	email: 	rubikscubist@gmail.com
	aim: 	DvdShraytel
	
a desktop switcher for random backgrounds and multiple monitors

Instructions:
Choose a directory with pictures in it and press change background.

Timer:
Set the interval, and click on the Denomination to change to hours,minutes,seconds,days
hit the go button

Ratio tolerance:
if a (picture's aspect ratio) / (aspect ratio of screen) is less than set percent, the image will be stretched, otherwise it will be scaled until one side of it is the same size as the screen. this works for multiple monitors as well

Use tolerance:
if a (picture's aspect ratio) / (aspect ratio of screen) is less than set percent, it will be ignored for consideration for that screen

Parsing:
click parse directory to use parsing until you close the program
click always use parse to parse directory and then always use the saved parse text file to find pictures to use in the future when using this program. to turn off parsing, uncheck always use parse.
parsing goes through the directory specified and records the filename and height and width of all pictures so that it is quickly accessible in the future. It takes a long time to parse, but afterwards, it takes less time to choose pictures. Once parsing is enabled, you can update the library when pictures are added or deleted, this will take less time than reparsing the entire directory.
check update at start to have the program update the library every time it starts up.

Program Filter:
put the name of the process in the text box, separated by spaces, and the timer won't change wallpapers if any of those programs are running.
ex. "hl2 GOM"

Other things:
to start the timer when the program starts, check start automatically
balloon tips will display the current picture(s) used for the background
if you only have one monitor, leave dual monitor unchecked
will automatically account for dual monitor wallpapers
if your desktop size changes or relative positions of monitors is changed, run diagnostic.
won't work correctly if there are two rows of monitors
if you delete a picture and are using parsing, make sure to update the library, otherwise there could be an error later
for the windows 7 fade effect, check the windows 7 fade option, though you have to manually change your wallpaper in windows once per computer boot for the fade effect to start working(for now)
if check for updates is checked, program will check for updates every time it starts


changelog:
09/14/11	---1.1.1---
			fixed issue with window staying disabled when wallpaper change function exited early
			bring wait label to front every time now so it doesn't appear behind other controls
09/13/11:	---1.1.0---
			removed registry interaction, if for some reason you were using this before and missed the latest version, get version 1.0.10 and load your registry settings with the menu item
			disabled main window while wallpaper change is in progress
			changed show log button to properly resize window
			fixed bug in which screens were not being detected properly
			created installation file to ease the updating process
			created an update check to see if there is a new version available
03/06/10:	---1.0.10---
			fixed scrolling in log
			fixed random number generator so there are less tries before finding a suitable picture
			switched to using appdata instead of registry, a button in the options to load settings from registry has been added, this will load settings and when program is exited, they will be saved to appdata. this button will be removed in a future release
01/30/10:	---1.0.9---
			added about page to show current version number
			fixed issue where picture selection would be wrong when using multiple monitors
			wallpaper now position themselves correctly when monitors are offset from each other vertically(dual wallpapers may have issues still with lining up)
			changed screen detection to show the correct screens
			added new way of seeing which screens are which on the system
01/22/10:	---1.0.8.2---
			fixed bug which made desktopswitcher stop windows from shutting down
01/12/10:	---1.0.8.1---
			fixed not making wallpaper tile when picture style was manually changed in system properties
11/14/09:	---1.0.8---
			fixed memory leak in autoparse that prevented computer from shutting down
11/07/09:	added program filter
10/27/09:	changed registry location so can use with UAC (fading doesn't work with UAC for some reason)
			corrected unchecked exceptions so there are no significant errors when starting for the first time
10/25/09:	put in option to use windows 7 fading when changing wallpaper
07/28/09:	fixed picture ratio detection
			added log
			added optional parsing system that saves data to a text file
07/20/09:	fixed start automatically so that it works now
06/28/09:	added start with windows option(don't change file location after checking or you'll have to do it again)
			added error message for registry failure
01/14/09:	streamlined diagnostic
			temporarily fixed bug that would mess up when one monitor was lower than others
			added display change detection (only really useful for tablet pc's)
			added choice of custom wallpapers for each screen
01/12/09:	added drag and drop			
01/11/09: 	added ability to detect physical monitor setup and have desktops being displayed across the monitors when primary monitor isn't the farthest left









disclaimer: an early build of this caused me to lose the ability to double-click for some reason, but that hasn't happened for a while and i'm pretty sure it's fixed
also, if something goes wrong, contact me and i'll say i'm sorry