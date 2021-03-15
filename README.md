#Csv Serialiser

This is the backend that converts FHIR resources to csv.

## Installation

Build project and copy `CsvSerialiser.dll` to a separate folder **inside** the plugins folder of Vonk.

See [this for reference](resources/appsettings.instance.json#L6-8). `$csv` needs to be added to the supported interactions. Additionally the plugin needs to be added for Vonk to pick it up. For this to work, the following lines need to be added to the Include-Property of Plugin Branches, like [so](resources/appsettings.instance.json#L33-34). The `appsettings.instance.json` can also be copied to the Vonk root folder.

```
"Vonk.Plugin.CsvSerializer.CsvSerializerConfiguration",
"Vonk.Plugin.CsvSerializer.CsvSerializerMiddlewareConfiguration"
```

## Usage

If post-processing is not needed, the plugin can be used by adding `/$csv` to the Search Parameter. Otherwise refer to [this README](https://git.bihealth.org/cei/csvserialiserfrontend/-/blob/master/README.md) for furhter instructions on how to use this plugin.