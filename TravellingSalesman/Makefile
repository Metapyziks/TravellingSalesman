CSC = /usr/local/bin/mcs

CSVERSION = future

DEF = LINUX

SRCDIR = .

SRC = \
	$(SRCDIR)/*.cs \
	$(SRCDIR)/Properties/*.cs

TARGET = ../TravellingSalesman.dll

release:
	rm -f $(TARGET)
	$(CSC) -langversion:$(CSVERSION) $(SRC) -d:$(DEF) \
		-t:library -out:$(TARGET)
