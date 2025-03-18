# How to build?

apt update
apt full-upgrade
apt install cmake
cmake --version
mkdir build
cd build
cmake ..
make

Client:

https://www.mqttstudio.com/en/index.html, see Microsoft Store
https://github.com/thomasnordquist/MQTT-Explorer

./MQTTShutterService.App -s 192.168.1.38 -u sidlvet -pw KrommeBeet55 -sub Shutters -c 555545

# Manual

https://blog.cyril.by/en/documentation/emqtt5-doc/emqtt5-tools

eMQTT5 is a MQTT v5.0 client that's targetting low resource usage for embedded system.

It comes with some useful example tools that'll allow you to connect to a MQTT v5.0 broker, subscribe and publish messages, and analyze the network's packet to figure out their meaning.

MQTTc is an example client using the provided library. It's build automatically when you build the software. You'll call it this way:

$ ./tests/MQTTc
Usage is: ./MQTTc [options]
Options:
    --help or -h            Get this help message
    --server or -s arg          The server URL (for example 'mqtt.mine.com:1883')
    --username or -u arg            The username to use
    --password or -pw arg           The password to use
    --clientid or -c arg            The client identifier to use
    --keepalive or -k arg           The client keep alive time
    --publish or -pub arg           Publish on the topic the given message
    --retain or -r          Retain published message
    --qos or -q arg         Quality of service for publishing or subscribing
    --subscribe or -sub arg         The subscription topic
    --der or -d arg         Expected broker certificate in DER format
    --verbose or -v         Dump communication

Connecting to a broker for subscribing to whatever topic, is done this way:

$ ./tests/MQTTc -s demo.mqtt.com -u me -pw secret -sub whatever
Connected to mqtt://demo.mqtt.com
Subscribed to whatever
Waiting for messages...

Connecting to a broker to publish a message to whatever is done this way (try in a new terminal):

$ ./tests/MQTTc -s demo.mqtt.com -u me -pw secret -pub whatever "My message"
Connected to mqtt://demo.mqtt.com
Published My message to whatever
The first terminal will show:

Msg received: (8F4C)
  Topic: whatever
  Payload: My message

MQTTParsePacket is a tool used to parse the network packet received from/sent to the network. It can be useful to debug communication issue with a broker or to ensure the validity of the broker's answer. You can use it this way:

$ # Give it the raw bytes from network communication and it'll dump what it means
$ ./MQTTParsePacket 30 1E 00 18 73 74 61 74 75 73 2F 59 4F 4C 54 79 79 76 75 57 58 50 5A 2F 6C 6F 67 73 00 5B 31 5D
Detected PUBLISH packet
with size: 32
PUBLISH control packet (rlength: 30)
  Header: (type PUBLISH, retain 0, QoS 0, dup 0)
  PUBLISH packet (id 0x0000): Str (24 bytes): status/YOLTyyvuWXPZ/logs
  Properties with length VBInt: 0
  Payload (length: 3)


./tests/MQTTc -s 192.168.1.14 -u me -pw secret -sub whatever
... use MQTT explorer to send on e.g. Windows to topic "whatever"
./tests/MQTTc -s raspberrypi.local -u u23893 -pw KrommeMozart55 -sub shutter/studygarden/command
