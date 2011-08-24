solution "MoronBot"

	language "C#"
	framework "3.5"
	location ( "MoronBot/" .. _ACTION )
	flags { "ExtraWarnings" }
	defines { "TRACE" }
		
	links { "System", "System.Core", "System.Xml.Linq", "System.Data.DataSetExtensions", "System.Data", "System.Xml" }
	
	configurations { "Debug", "Release" }
	
	configuration "Debug"
		defines { "DEBUG" }
		flags { "Symbols" }
		buildoptions { "-debug" }
		targetdir "../MoronBot/debug/"
		
	configuration "Release"
		flags { "Optimize" }
		targetdir "../MoronBot/bin/"
		
	project "CwIRC"
		uuid "E4D2DC70-87EA-405A-ACDC-8E0AD3015607"
		files { "../MoronBot/CwIRC/**.cs" }
		kind "SharedLib"
		
	project "MBFunctionInterface"
		uuid "CC33E7C5-53BF-4A3F-9B0F-F11939F61F84"
		files { "../MoronBot/MBFunctionInterface/**.cs" }
		kind "SharedLib"
		
		links { "CwIRC" }
		
	project "MBUtilities"
		uuid "05E8C267-4CF0-4A9C-B8F5-F5A9762D9982"
		files { "../MoronBot/MBUtilities/**.cs" }
		kind "SharedLib"
		
		links { "CwIRC", "MBFunctionInterface" }
		links { "System.Windows.Forms", "System.Web" }
		links { "..\\MoronBot\\MoronBot\\Bitly.dll", "..\\MoronBot\\MoronBot\\GAPI.dll", "..\\MoronBot\\MoronBot\\System.Data.SQLite.dll" }
	
	project "MoronBot"
		uuid "814409D2-DE90-4E1B-BE9F-3B83D3F81F5D"
		files { "../MoronBot/MoronBot/**.cs" }
		kind "ConsoleApp"
		
		links { "CwIRC", "MBFunctionInterface", "MBUtilities" }
