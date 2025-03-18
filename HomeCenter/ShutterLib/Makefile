
all: libshutter.so s.exe

s.exe: s.cs
	mcs s.cs

libshutter.so: shutter.o
	g++ -g -fPIC -rdynamic -shared -o libshutter.so shutter.o -lstdc++

shutter.o: shutter.cpp
	g++ -fPIC -shared -rdynamic -Wall -Wextra -Werror -g -c shutter.cpp -o shutter.o

clean:
	rm shutter.o
	rm libshutter.so
