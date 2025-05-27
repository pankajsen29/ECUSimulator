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
Windows 10 or above.

**Prerequisites:**
- .NET 8.0
- Vector Driver: Go to Vector Download Center: https://www.vector.com/int/en/support-downloads/download-center
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

**Overall Project structure:**
- the main GUI host application is a Winform application which supports addition of WPF projects (notice "<UseWPF>True</UseWPF>" in ECUSim.csproj file);
	- winforms: used for fast development
	- wpf: is used for rich GUI (MVVM pattern is followed)
- todo: other projects and libs

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
