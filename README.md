**ECU Simulator:**

**Scope:**
- Automotive domain
- Basic simulation of CAN request/response
- CAN/CANFD tracing

**Description:**
- A simple GUI application to configure request/response (CAN), where:
    - request: a CAN message to be received from tester application
    - response: a CAN message typically an ECU would send on the CAN Bus corresponding to a the request message
- Utilizes Vector Virtual Interface, thereby removing the need of any hardwares.
- Testable for CANFD communications too, (i.e., with payload higher than 8 bytes)

**Motivation:**
- the main idea is to enable automotive tool developers perform testing without the need of physical hardwares (e.g., powerpack/ECU and the ECU interface hardware).
- to have a simple UI unlike already available softwares in market (e.g., CANalyzer (link) or BUSMASTER (link)).
- Unlike CANalyzer:
    - no cost for the license.
    - no new programming language (CAPL (Communication Access Programming Language) is used by CANalyzer) is required to be learnt or used.
    - not similar to any other CAPL solutions, as for these kind of solutions would need physical hardware from Vector, thereby requiring to buy the license.
- Unlike BUSMASTER:
    - no cost for the license (though BUSMASTER is free for testing CAN communication, CANFD test feature is licensed)  


**Target Platform:**

Supported OS Version: Windows 10 Version 20H1 (Build 10.0.19041.0) or above.

Tested in: Windows 11 Version 24H2 (OS Build 26100.4061)

**Prerequisites:**
- .NET 8.0
- Vector Driver: Go to Vector Download Center:
  
  https://www.vector.com/int/en/support-downloads/download-center
  
and download the latest version of the followings:

(1). Vector Driver Setup for Windows 10 and 11:

Install only the "Mandatory Components" (note: install the specific driver corresponding to an already available hardware interface from Vector) which include the followings and are enough for performing the tests with this application:
	a) Virtual CAN Bus
	b) Vector Hardware Config (legacy)
	c) Vector Hardware Manager

Note: steps are shown in the "Test Setup" section.

Used version for testing: Vector Driver Setup 25.20.0 for Windows 10 and 11

https://www.vector.com/int/en/download/vector-driver-setup-25-20-0-for-windows-10-and-11/

(2). xl-driver-library:

Install and (optionally) copy the below dlls from "C:\Users\Public\Documents\Vector\XL Driver Library 25.20.14.0\bin" to solution's 'Ref' folder for example:
a) vxlapi.dll [XL Driver Library DLL. This file should be present in the folder with the application.]
b) vxlapi64.dll [64 Bit version of XL Driver Library DLL. This file should be distributed with the application.]
c) vxlapi_NET.dll [.NET Wrapper for .NET applications. This file should be distributed with the application.]

from which "vxlapi_NET.dll" is referenced from the project "VectorXLWrapper" and manually copied the other 2 dlls "vxlapi64.dll" and "vxlapi.dll" to the build "Output" folder.

Note: There are no official C# code samples from Vector specifically targeting .NET 8 or higher for the XL-Driver-Library available on the public Vector website or in their documentation as of now. The official Vector XL-Driver-Library package includes a .NET wrapper (vxlapi_NET) and C# samples (local paths are given below), but these are generally based on older .NET versions (such as .NET Framework or .NET Core 3.x). However, the .NET wrapper is compatible with .NET 3.5 and above, therefore I have used it in this .NET 8 project application.

Referred version: XL Driver Library 25.20.14

https://www.vector.com/se/en/download/xl-driver-library-25-20-14/

API documentations: 
online: 

https://www.vector.com/se/en/download/manual-xl-driver-library/

local path once installed: 

C:\Users\Public\Documents\Vector\XL Driver Library 25.20.14.0\doc\XL Driver Library - Description.pdf

Factsheet: https://cdn.vector.com/cms/content/products/XL_Driver_Library/Docs/XL_Driver_Library_Factsheet_EN_01.pdf

.NET samples can be referred here:

CAN: C:\Users\Public\Documents\Vector\XL Driver Library 25.20.14.0\samples\NET\xlCANdemo_Csharp

CANFD: C:\Users\Public\Documents\Vector\XL Driver Library 25.20.14.0\samples\NET\xlCANFDdemo_Csharp


**Overall Projects Description:**
- ECUSim: The is the main GUI host application. It is a Winform application which also supports addition of WPF projects (notice "<UseWPF>True</UseWPF>" in ECUSim.csproj file).
	- winforms: used for faster development.
	- wpf: is used for rich GUI (MVVM pattern is followed)

- UtilityLib: includes utility methods, e.g., json serialization/deserialization methods.
  
- CommonHwLib: This is the main layer for hardware communication. It includes the wrapper classes for hardware drivers. handles the driver initialization and communication (sending/receiving of CAN messages)
  
- HwSettingsLib: includes definitions of types used for hardware communication.
  
- HwWrapperInterface: interface for Hardware wrapper library.
  
- VectorXLWrapper: vector hardware wrapper/driver library which implements the methods for vector driver initialization, transmitting/receiving messages etc.

- HwWrapperFactory: includes the factory classes for returning the concrete object of the hardware wrapper/driver library.

- MessageDesignerLib: includes the types used for defining a message to be transmitted.

- WPFHostLib (Windows Forms Class Library): contains an ElementHost control to load a WPF view in winform application. This one library is used for loading all the WPF views created in this whole application. With this approach including multiple ElementHost controls is avoided in the application.

To include and use ElementHost in this .NET 8 WinForms class library project:
1) Added the following properties to .csproj file to enable WPF interop:

<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
	  
    <UseWindowsForms>true</UseWindowsForms>
    <UseWPF>true</UseWPF>
    
  </PropertyGroup>

</Project>

This is required for the project to reference both WinForms and WPF assemblies, including WindowsFormsIntegration.dll that contains ElementHost.

Below references are supposed to be automatically included when "<UseWPF>true</UseWPF>" was included in the project file:

    WindowsFormsIntegration.dll

    PresentationCore.dll

    PresentationFramework.dll

    WindowsBase.dll

But, these didn't get added automatically.

Therefore, below additional properties are added to .csproj file, to explicitly add the framework references:

<ItemGroup>
  <FrameworkReference Include="Microsoft.WindowsDesktop.App.WindowsForms" />
  <FrameworkReference Include="Microsoft.WindowsDesktop.App.WPF" />
</ItemGroup>



2) And then add the below using:

using System.Windows.Forms.Integration; // For ElementHost


- WPFLibBase (WPF Class Library): Library containing the interface usercontrol for all the wpf view to implement, which will be used by WPFHostLib to initialize and load the view.

- WPFTraceViewLib (WPF Class Library): it includes the view for showing the CAN trace of the communication.

- WPFComSetupViewLib (WPF Class Library): it includes the view for modifying the communication settings and initialization of the CAN communication.


**Overall Project structure:**
- todo: class relations to be included
- todo: relations between the assemblies also are to be shown

**Functailities:**
 - communication setup: hardware setup
 - messages (request-response) setup: CAN messages
 - trace window: displays CAN messages

**Test Setup:**

**Testing:**
Steps to be shown using Vector Virtual CAN Driver

**Limitation:** (Temporary)
 - the hardware communication code supports only Vector interfaces at the moment, hence can't be used with any other hardware interfaces (e.g., PEAK, DCI, ETAS etc.). But have plan to support in case the planned primary purpose of this tool is observed. Design is done considering this point.
 
**Logging:**
Log4net (to be included)

**CMD-line execution:**
- todo: for the purpose of automation

**Installer:**
