CXX = $(shell wx-config --cxx)
WX_LIBS = $(shell wx-config --libs)
WX_CXXFLAGS = $(shell wx-config --cxxflags)
src = $(wildcard *.cpp)
obj = $(src:.cpp=.o)
PREFIX = /usr

capitalizer: $(obj)
	g++ -o $@ $+ $(WX_LIBS)

%.o: %.cpp
	g++ $(WX_CXXFLAGS) -c $< -o $@

PHONY: install
install: capitalizer
	@mkdir -p $(DESTDIR)$(PREFIX)/bin
	@cp -r $< $(DESTDIR)$(PREFIX)/bin

PHONY: uninstall
uninstall:
	@rm -f $(DESTDIR)$(PREFIX)/bin/capitalizer

.PHONY: clean
clean:
	@rm -f *.o capitalizer
