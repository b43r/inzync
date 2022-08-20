# InZync
Sophisticated Windows application for synchronizing files and folders. Requires .NET Framework 4.8.

InZync let's you compare and synchronize the files in multiple directories:
![main window](https://github.com/b43r/inzync/blob/master/img/main.png "main window")

Different actions can be configured for every situation. InZync can be used to backup your files to an external harddrive, or synchronize files by keeping only the newer ones.
![settings window](https://github.com/b43r/inzync/blob/master/img/settings.png "settings window")

Files can be filtered by extension or file attributes. Synchronization jobs can be saved to a file that can be run by double-clicking.

## Command line switches

```
/help            Display all possible command line commands
/job "filename"  Load the job from the given file
/run             Immediately execute the job loaded with the /job parameter
/silent          Do not display any UI while running a job (use together with /job and /run)
```
