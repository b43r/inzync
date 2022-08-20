Version history:
----------------

20.08.2022: Version 2.1.0

	- Updated to VS 2022 and .NET Framework 4.8


23.02.2020: Version 2.0.0

	- Renamed from eSync.NET to InZync.

	- Various small bugs fixed.

	- eSync logfile is ignored if saved in the source or destination folder.

	- Sourcecode ported to VS 2019 and .NET Framework 4.6.1.

	- Support for files >2GB.

	- Optimized display of file size, show size of different files in status bar.


12.03.2006: Version 1.0.8

	- User defined context menu entries are only shown if both files are existing.
	
	- In the "Paths to synchronize" dialgo an "update" button has been added.
	
	- One user defined context menu entry can be configured to be executed
	  automatically with a double-click on the different files.
	

23.02.2006: Version 1.0.7

	- Files with certain extensions can be excluded from synchronization.
	
	- Context menu can start external application (e.g. compare tool, WinMerge
	  and ExamDiff are automaticaly recognised).
	
	- Bug in checking file extensions fixed.
	
	- Icons in menus.
	

16.12.2005:	Version 1.0.6

	- Ported to Microsoft .NET Framework 2.0, Windows XP like GUI.
	
	- Sort column and order is saved in the registry. Bugfix regarding
	  column orders.
	  
	- Wildcards for file extensions are now fully supported.
	  
	- Various code optimisations and refactoring,
	  use of generic collections, a lot of comment added.
	  
	- Online help added, try F1.


28.08.2005:	Version 1.0.5

	- New column "Destination path".
	
	- Some columns can be hidden.
	
	- Column widhts are saved to the registry.

	- List can be sorted by clicking on a column header.


22.06.2005:	Version 1.0.4

	- Filelist now shows source path instead of relative path.

	- Progress display much more significant when synchronizing
	  multiple directories.
	  
	- New menu command "Options", "Clear list".

	- Bugfix: All GUI elements are disabled during comparing/synchronizing.
	
	- Bugfix: A directory containing only an empty subdirectory was not
	  recognized as empty. Deletion caused an exception.
	
	- Option to choose if double clicking a .syncjob file in explorer will
	  run the job or open it in eSync.NET.


26.05.2005:	Version 1.0.3

	- Multiple source/destination paths.

	- ".syncjob" files are now associated with eSync.NET
	  so you can doubleclick them in explorer.

	- Filtering for file extensions more userfriendly by
	  using templates. (See "FileExtensions.xml")

	- New menu command "Save job as..."

	- Progress window is no longer topmost window and now
	  has a "Cancel" button.

	- Additional filter for "archive" file attribute
	  so eSync.NET can be used to do simple backups.

	- Bug in handling directories fixed.

	- Multiple selection possible in file list.

	- Program icon ;-)


10.02.2005:	Version 1.0.2

	- Command line support. (try /help)

	- Filter to show only different files.

	- Link to project webpage in help menu.

	- Logfile can be saved, either automatically or manually.


02.12.2004:	Version 1.0.1

	- After a compare the proposed action can be changed
	  by right clicking on a row in the grid.

	- Application does not crash anymore if it encounters a
	  file without any extension.

	- Files with "system" or "hidden" attributes set can
	  be excluded from comparing.

	- Progress bar window does not stay open anymore under
	  some circumstances.

	- Read-only files can also be overwritten.


17.11.2004:	Version 1.0.0

	- First release.



Known bugs / things to do:
--------------------------

	- Known bug: It's now possible to manually change the proposed actions so
	  that a file in a subdirectory should be created but subdirectory itself should
	  be deleted. Of course this will not work... ;-)
