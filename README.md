# Introduction
This project is an in progress .NET implementation of the [OGC SensorThings 1.0 spec](http://docs.opengeospatial.org/is/15-078r6/15-078r6.html) server. 

One of the main goals of this implementation has been to enable the embedding of a SensorThings server into a Xamarin.Android application.

# Benefits
The primary benefits of this implementation is that it allows a SensorThings server to be embedded where ever .NET can run. For example, the server may be embedded into an Android service, or it may be run on Windows, Linux, or Mac!

# Getting Started
To get started running the server, take a look at the [TestDriver.cs](https://github.com/PAR-Government/sensor-things-server/blob/master/TestDriver/Driver.cs) file.

# Known Deficiencies
The following capabilities defined in the spec are not yet implemented at this time:
* Deep tree creation of entities
* Enforcement of associations between entities
* Query/filter capabilities
