Prism 4.0
Welcome to the 4.0 release of Prism (formally known as the Composite Application Guidance for WPF and Silverlight). This file contains late-breaking information that can be useful in using the guidance.

============
Known Issues
============
To see the Prism Library known issues, see: http://www.codeplex.com/CompositeWPF/Wiki/View.aspx?title=Known%20Issues%20%2f%20Fixes.

============
System Requirements
============
This guidance was designed to run on the Microsoft Windows 7, Windows Vista, Windows XP Professional, Windows Server 2003, or Windows Server 2008 operating system. 
WPF applications built using this guidance will require the .NET Framework 4.0 to run and Silverlight applications will require the .NET Framework for Silverlight 4.

If you are using a previous version of Silverlight, please see "New in This Release" on MSDN:
http://msdn.microsoft.com/en-us/library/ff649547.aspx

Before you can use the Composite Application Library, the following must be installed:
• Microsoft Visual Studio 2010
• Microsoft .NET Framework 4.0 (the .NET Framework 4.0 includes WPF): http://www.microsoft.com/downloads/details.aspx?familyid=9CFB2D51-5FF4-4491-B0E5-B386F32C0992&displaylang=en
• Microsoft Silverlight 4 (this is required only if you are creating Silverlight applications): http://www.microsoft.com/silverlight/
• Microsoft Silverlight 4 Tools for Visual Studio 2010 (this is required only if you are creating Silverlight applications): 
http://www.microsoft.com/downloads/details.aspx?FamilyID=eff8a0da-0a4d-48e8-8366-6ddf2ecad801&displaylang=en
• MOQ (Moq.4.0.10531.7-bin.zip - includes Silverlight): http://code.google.com/p/moq/ 
	
You may also want to install the following:
• Microsoft Expression Blend 4: http://www.microsoft.com/expression/products/Blend_Overview.aspx
• Microsoft Visual Studio 2010 SDK to compile Project Linker: http://www.microsoft.com/downloads/details.aspx?FamilyID=47305cf4-2bea-43c0-91cd-1b853602dcc5&displaylang=en
• Microsoft Silverlight Unit Test Framework to run the unit tests in Silverlight: http://silverlight.codeplex.com/releases/view/43528

==================================================     
Compiling the Prism Library Source
==================================================
To compile the Prism Library source code, perform these steps:

	1. Double-click the "Desktop & Silverlight - Open Composite Application Library" batch file to open the solution in Visual Studio.
	2. Build the solution. 
		- The Prism Library assemblies for a Desktop application will be placed in the CAL\Desktop\Composite.UnityExtensions\bin\Debug folder.
		- The Prism Library assemblies for a Silverlight application will be placed in the CAL\Silverlight\Composite.UnityExtensions\bin\Debug folder.


========================================================
Solutions Included with Prism
========================================================
Prism includes different sample solutions. To open them in Visual Studio, you can run the following batch files or Solutions

 * "Desktop & Silverlight - Open Composite Application Library.bat"	
 * "Desktop & Silverlight - Open RI - StockTrader Reference Implementation.bat"
 * "Desktop only - Open Composite Application Library.bat"                                                                                                                  
 * "Desktop only - Open RI - StockTrader Reference Implementation.bat"                                                                                                      


Prism includes the following QuickStarts:
 * BasicMVVM
 * Desktop\ModularityWithMef
 
====================================
Documentation
====================================
Prism includes the following documentation:

	* Prism 4. This is the guidance documentation.

======================================================
Prism Library and Code Access Security
======================================================
The Prism Library uses all the default .NET settings with regards to signing assemblies and code access security. It is a recommended practice to strong name all your assemblies, including the Composite Application Library assemblies, shell assembly, and any modules you might want to create. This is not a requirement. It is possible to load assemblies that have not been signed into a (signed or unsigned) Composite Application Library application. You can change this behavior by applying a .NET security policy that disallows the use of unsigned assemblies or one that changes the trust level of an assembly. Please note that the .NET Framework does not allow you to load partially trusted assemblies, unless you add the AllowPartiallyTrustedCallers attribute to the Prism Library assemblies. 

For more information, check the Code Access Security section on MSDN (http://msdn.microsoft.com/en-us/library/930b76w0.aspx).