# Command Glossory
Record all of the slash command information in this file.

## Central Server Only
Commands here are only available at the central server level.

## Company Only
Commands here are only available at company levels.

### Company Administration
These commands control the administrative process of the company.

#### *Pre-Established Commands*
These commands can only be used prior to a company being established. Once a company is established, thse commands are
removed from the guild and are returned when the company gets disbanded.
* `begin-establishment`: Begins the establishing process within a company. 
    * Can only be used by the server owner.
* `establish-company`: Attempts to complete the establishing process of a company. Must meet
certain conditions. 
    * Owner & *admin console* usage only

#### *Post-Established Commands*
These commands can only be used after the guild is a verified and established company. The majority of the administrative 
commands live in this category.
* `disband-company`: Disbands a company and undeploys all players within.
    * Owner & *admin console* usage only


### Default Company Commands
The commands are available regardless of the company's establishment status.
* `checklist`: A simple command that returns the necessary requirements for a company to remain operational
* `create-role`: A command that creates and assign roles and their ids for the company. 
    * Has the ability to create all roles
    * Owner & *admin console* usage only
* `set-role`: 

### Player Commands
These commands can only be used after the guild is a verified and established company.
For reference, these commands are technically *Post-Established* commands. 

* `deploy`: If allowed, this command deploys a player to their selected company
* `undeploy`: If used in the correct guild, a player removes their game from the company 
* `save`: A manual save option to save the player's progress


## Pseudo Global
Commands are available in both the central server and company levels, but not at DM channels.

* `ping`: Returns the latency of the bot. 
* `doc`: Currently displays game resource data but 
    eventually will server as a comprehensive documentation command for the player

## Global
Commands here are truly globally available.
