CXX = $(shell wx-config --cxx)
WX_LIBS = $(shell wx-config --libs)
WX_CXXFLAGS = $(shell wx-config --cxxflags)
PREFIX = /usr

capitalizer: Capitalizer.o MainFrame.o
	g++ Capitalizer.o MainFrame.o $(WX_LIBS) -o capitalizer

Capitalizer.o:
	g++ -c Capitalizer.cpp $(WX_CXXFLAGS)
MainFrame.o:
	g++ -c MainFrame.cpp $(WX_CXXFLAGS)

install: capitalizer
	@cp $< $(DESTDIR)$(PREFIX)/bin

uninstall:
	@rm -f $(DESTDIR)$(PREFIX)/bin/capitalizer

clean:
	@rm -f *.o capitalizer
