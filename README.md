# ManiacClipboardManager
A class library on .NET Standard written in C# programming language that provides types and functions to work with the operating system's clipboard.

## Contents
1. [Description of the library basics](#description-of-the-library-basics)
   - [IClipboardManager interface](#iclipboardmanager-interface)
     - [IsMonitoring](#ismonitoring)
     - [ClipboardChanged](#clipboardchanged)
     - [StartMonitoring](#startmonitoring)
     - [StopMonitoring](#stopmonitoring)
     - [GetClipboardData](#getclipboarddata)
     - [SetClipboardData](#setclipboarddata)
     - [GetClipboardDataType](#getclipboarddatatype)
     - [ClearClipboard](#clearclipboard)
     - [IsClipboardEmpty](#isclipboardempty)
     - [GetClipboardDataAsync](#getclipboarddataasync)
     - [SetClipboardDataAsync](#setclipboarddataasync)
     - [GetClipboardDataTypeAsync](#getclipboarddatatypeasync)
     - [ClearClipboardAsync](#clearclipboardasync)
     - [IsClipboardEmptyAsync](#isclipboardemptyasync)
   - [ClipboardDataType enum](#clipboarddatatype-enum)
   - [ClipboardData class](#clipboarddata-class)
   - [ClipboardChangedEventArgs class](#clipboardchangedeventargs-class)
   - [ClipboardSource class](#clipboardsource-class)
2. Implementations of the IClipboardManager interface
   - Windows
     - WindowsClipboardManager class
     - Shell32 class
     - WindowsImageHelper class

## Description of the library basics
### IClipboardManager interface
The IClipboardManager interface was made for all implementations of clipboard managers for given operating system. The interface inherits from the IDisposable interface.

##### IsMonitoring
Read only property that gets true when the manager observes the operating system's clipboard, otherwise false

##### ClipboardChanged 
Rises when the operating system's clipboard has changed. As the event parameters it creates an instance of the ClipboardChangedEventArgs class

##### StartMonitoring
Starts to observe the operating system's clipboard (should change the value of the IsMonitoring property to true)

##### StopMonitoring
Stops observing the operating system's clipboard (should change the value of the IsMonitoring property to false)

##### GetClipboardData
Gets current stored data from the clipboard, returns null if the clipboard is empty

##### SetClipboardData
Sets given data on the clipboard. The data can be null so the clipboard will be cleared

##### GetClipboardDataType
Gets ClipboardDataType enum value that indicates the type of the data that the clipboard stores currently. The method returns Unknown type when the clipboard is empty

##### ClearClipboard
Clears the clipboard from any data

##### IsClipboardEmpty
Gets true when the clipboard is empty, otherwise false.

##### GetClipboardDataAsync
Returns a task to invoke the GetClipboardData method asynchronously

##### SetClipboardDataAsync
Returns a task to invoke the SetClipboardData method asynchronously

##### GetClipboardDataTypeAsync
Returns a task to invoke the GetClipboardDataType method asynchronously

##### ClearClipboardAsync
Returns a task to invoke the ClearClipboard method asynchronously

##### IsClipboardEmptyAsync
Returns a task to invoke the IsClipboardEmpty method asynchronously

### ClipboardDataType enum
Enum type to indicate the type of data that comes from the operating system's clipboard

##### Unknown
Unknown/Not supported type of data (object type)
##### Text
Text type of data (string type)
##### PathList
Type of data that contains paths to files and folders (string[] array)
##### Image
Image type of data (System.Drawing.Bitmap)

### ClipboardData class
Represents data that were stored on the operating system's clipboard

##### ClipboardData(data, dataType)
Constructor that takes data and type of the data as the parameters, data parameter cannot be null and value of the dataType parameter must be defined
There are additional constructors with extra parameters:
formats - array of strings that contains all of the data formats, can be null
source - source of the data, can be null

##### Data
This read only property contains the data

##### DataType
This read only property gets the type of the data

##### Source
This read only property gets the source of the data. Can be null

##### GetFormats
Copies the array of data and returns it

##### ToString
Returns string from the Data.ToString()

The class also has static methods that help with creating instances of the ClipboardData class and retrieving data from the class as proper types, they are all well described in the source code

### ClipboardChangedEventArgs class
Contains the data that were provided by the IClipboardManager.ClipboardChanged event

##### ClipboardChangedEventArgs(clipboardData)
Constructor that takes one parameter as the ClipboardData class. The parameter cannot be null

##### Data
Gets the data that were provided by the IClipboardManager.ClipboardChanged event

### ClipboardSource class
Contains information about the source of stored data from the clipboard. The class implements the IEquatable<ClipboardSource> interface

#### ClipboardSource(appName)
Constructor that takes a string parameter which is the name of the app where the data come from. The parameter cannot be null or empty

There is one additional constructor that takes another string parameter which is the path of the icon of the source application

##### AppName
Gets the name of the source application

##### IconPath
Gets the path of the icon of the source application

##### ToString
Gets the AppName property value
