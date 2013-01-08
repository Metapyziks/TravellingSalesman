CSC = /usr/local/bin/mcs

CSVERSION = future

DEF = LINUX

SRCDIR = .

SRC = \
	$(SRCDIR)/*.cs \
	$(SRCDIR)/Properties/*.cs

LIB = ../TravellingSalesman.dll

TARGET = ../Searcher.exe

release:
	rm -f $(TARGET)
	$(CSC) -langversion:$(CSVERSION) $(SRC) -r:$(LIB) -d:$(DEF) \
		-t:exe -out:$(TARGET)
