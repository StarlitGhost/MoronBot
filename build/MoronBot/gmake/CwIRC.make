# GNU Make project makefile autogenerated by Premake

ifndef config
  config=debug
endif

ifndef verbose
  SILENT = @
endif

ifndef CSC
  CSC=gmcs
endif

ifndef RESGEN
  RESGEN=resgen
endif

ifneq (,$(findstring debug,$(config)))
  TARGETDIR  := ../../../MoronBot/debug
  OBJDIR     := obj/Debug/CwIRC
  DEPENDS    := 
  REFERENCES := 
  FLAGS      += /d:TRACE /d:DEBUG /debug -debug
  define PREBUILDCMDS
  endef
  define PRELINKCMDS
  endef
  define POSTBUILDCMDS
  endef
endif

ifneq (,$(findstring release,$(config)))
  TARGETDIR  := ../../../MoronBot/bin
  OBJDIR     := obj/Release/CwIRC
  DEPENDS    := 
  REFERENCES := 
  FLAGS      += /d:TRACE /optimize
  define PREBUILDCMDS
  endef
  define PRELINKCMDS
  endef
  define POSTBUILDCMDS
  endef
endif

# To maintain compatibility with VS.NET, these values must be set at the project level
TARGET     := $(TARGETDIR)/CwIRC.dll
FLAGS      += /t:library 
REFERENCES += /r:System.dll /r:System.Core.dll /r:System.Xml.Linq.dll /r:System.Data.DataSetExtensions.dll /r:System.Data.dll /r:System.Xml.dll

SOURCES := \
	..\\..\\..\\MoronBot\\CwIRC\\Interface.cs \
	..\\..\\..\\MoronBot\\CwIRC\\IRCMessage.cs \
	..\\..\\..\\MoronBot\\CwIRC\\IRCResponse.cs \
	..\\..\\..\\MoronBot\\CwIRC\\Properties\\AssemblyInfo.cs \

EMBEDFILES := \

COPYFILES += \

SHELLTYPE := msdos
ifeq (,$(ComSpec)$(COMSPEC))
  SHELLTYPE := posix
endif
ifeq (/bin,$(findstring /bin,$(SHELL)))
  SHELLTYPE := posix
endif

.PHONY: clean prebuild prelink

all: $(TARGETDIR) $(OBJDIR) prebuild $(EMBEDFILES) $(COPYFILES) prelink $(TARGET)

$(TARGET): $(SOURCES) $(EMBEDFILES) $(DEPENDS)
	$(SILENT) $(CSC) /nologo /out:$@ $(FLAGS) $(REFERENCES) $(SOURCES) $(patsubst %,/resource:%,$(EMBEDFILES))
	$(POSTBUILDCMDS)

$(TARGETDIR):
	@echo Creating $(TARGETDIR)
ifeq (posix,$(SHELLTYPE))
	$(SILENT) mkdir -p $(TARGETDIR)
else
	$(SILENT) mkdir $(subst /,\\,$(TARGETDIR))
endif

$(OBJDIR):
	@echo Creating $(OBJDIR)
ifeq (posix,$(SHELLTYPE))
	$(SILENT) mkdir -p $(OBJDIR)
else
	$(SILENT) mkdir $(subst /,\\,$(OBJDIR))
endif

clean:
	@echo Cleaning CwIRC
ifeq (posix,$(SHELLTYPE))
	$(SILENT) rm -f $(TARGETDIR)/CwIRC.* $(COPYFILES)
	$(SILENT) rm -rf $(OBJDIR)
else
	$(SILENT) if exist $(subst /,\\,$(TARGETDIR)/CwIRC.*) del $(subst /,\\,$(TARGETDIR)/CwIRC.*)
	$(SILENT) if exist $(subst /,\\,$(OBJDIR)) rmdir /s /q $(subst /,\\,$(OBJDIR))
endif

prebuild:
	$(PREBUILDCMDS)

prelink:
	$(PRELINKCMDS)

# Per-configuration copied file rules
ifneq (,$(findstring debug,$(config)))
endif

ifneq (,$(findstring release,$(config)))
endif

# Copied file rules
# Embedded file rules
