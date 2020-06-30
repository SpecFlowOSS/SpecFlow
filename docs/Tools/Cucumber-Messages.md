# Cucumber Messages

Cucumber messages provide a set of standardised messages across all Cucumber implementations. These messages are emitted when running your scenarios.

A standardised set of messages make it possible to write tools (e.g. report generators) that work for all Cucumber implementations, such as SpecFlow, Cucumber JVM, Cucumber Ruby, Cucumber.js etc.

Cucumber messages are sent to sinks, which you can configure. If Cucumber messages are enabled, but no sinks are configured, a file sink with the default path `cucumbermessages\messages` is used.

## Configuration

You can configure Cucumber messages in your specflow.json configuration file, or in the the SpecFlow section of your App.config (.NET Framework only).

The App.config specifications can be found [here](https://github.com/techtalk/SpecFlow/blob/master/Tests/TechTalk.SpecFlow.Specs/Features/Configuration/Cucumber-Messages%20App.Config%20Configuration.feature).

The specflow.json specifications can be found [here](https://github.com/techtalk/SpecFlow/blob/master/Tests/TechTalk.SpecFlow.Specs/Features/Configuration/Cucumber-Messages%20Json%20Configuration.feature)     .

### Enabled

You can enable or disable the generation of Cucumber Messages by setting `enabled` to "true" or "false" in your configuration. The following defaults apply depending on the version of SpecFlow you are using:

- SpecFlow 3.1: Cucumber messages are disabled by default.  
- SpecFlow 3.2 and later: Cucumber messages are enabled by default.  

#### Example: Enabling Cucumber Messages

**specflow.json**

```
{
    "cucumber-messages": {
        "enabled": true
    }
}
```

**App.config**

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <configSections>
        <section name="specFlow" type="TechTalk.SpecFlow.Configuration.ConfigurationSectionHandler, TechTalk.SpecFlow" />
    </configSections>

    <specFlow>
        <cucumber-messages enabled="true" />
    </specFlow>
</configuration>
```

Set `enabled` to "false" to disable the Cucumber messages.

### Sinks

Sinks determine where Cucumber messages are sent. If Cucumber messages are enabled, but no sinks are configured, a file sink with the default path `cucumbermessages\messages` is used.

Use the `type` property to determine the type of sink.  

#### File Sinks

When using file sinks (`type="file"`), Cucumber messages are written to the file specified using the `path` property.

#### Example: File Sinks

**specflow.json**

```
{
    "cucumber-messages": {
        "enabled": true,
        "sinks": [
            {
                "type": "file",
                "path": "custom_cucumber_messages_file.cm"
            }
        ]
    }
}
```

**App.config**

``` 
    <specFlow>
        <cucumber-messages enabled="true">
            <sinks>
                <sink type="file" path="custom_cucumber_messages_file.cm" />
            </sinks>
        </cucumber-messages>
    </specFlow>
```