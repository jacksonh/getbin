

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/getbin.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

GETBIN_EXE_CONFIG_SOURCE=getbin.exe.config

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/getbin.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

GETBIN_EXE_CONFIG_SOURCE=getbin.exe.config

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

PROGRAMFILES = \
	$(GETBIN_EXE_CONFIG)  

BINARIES = \
	$(GETBIN)  


	
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES) 

FILES = \
	getbin.cs \
	Options.cs 

DATA_FILES = \
	getbin.exe.config 

RESOURCES = 

EXTRAS = \
	getbin.in 

REFERENCES =  \
	System.Configuration \
	System

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

include $(top_srcdir)/Makefile.include

GETBIN = $(BUILD_DIR)/getbin
GETBIN_EXE_CONFIG = $(BUILD_DIR)/getbin.exe.config

$(eval $(call emit-deploy-wrapper,GETBIN,getbin,x))
$(eval $(call emit-deploy-target,GETBIN_EXE_CONFIG))


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
