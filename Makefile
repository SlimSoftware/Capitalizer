CXX = $(shell wx-config --cxx)
WX_LIBS = $(shell wx-config --libs)
WX_CXXFLAGS = $(shell wx-config --cxxflags)

capitalizer: Capitalizer.o MainFrame.o
	g++ Capitalizer.o MainFrame.o $(WX_LIBS) -o capitalizer

Capitalizer.o:
	g++ -c Capitalizer.cpp $(WX_CXXFLAGS)
MainFrame.o:
	g++ -c MainFrame.cpp $(WX_CXXFLAGS)

clean:
	rm *.o capitalizer