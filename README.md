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
- (USP) also to have switching to higher payload or higher baudrate (from CAN to CANFD and vice-versa) dynamically which is unique in this tool. 
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
- Vector Driver (version is to be indicated, link to be included)

**Overall Project structure:**
- the main GUI host application is a Winform application which supports addition of WPF projects (notice "<UseWPF>True</UseWPF>" in ECUSim.csproj file);
	- winforms: used for fast development
	- wpf: is used for rich GUI (MVVM pattern is followed)
- todo: other projects and libs

**Functailities:**
 - communication setup: hardware setup
 - messages (request-response) setup: CAN messages
 - trace window: displays CAN messages

**Testing:**
Steps to be shown using Vector Virtual CAN Driver

**Limitation:** (Temporary)
 - it utilizes Vector Virtual Interface at the moment for enabling the developers perform testing, hence can't be used with any other hardware interfaces (e.g., PEAK, DCI, ETAS etc.) at the moment. But have plan to support in case the planned primary purpose of this tool is observed.
 
**Logging:**
Log4net (to be included)

**CMD-line execution:**
- todo: for the purpose of automation
