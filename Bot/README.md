### Welcome to the Best Discord Game Ever Made
# [Name of Game Pending]

This README is geared towards installing the project to ready for development. Welcome valued team member.

Our expectation is you are already familiar with git. As such, go ahead and clone this project into your desired 
directory and follow further installation steps.

## [0] Project and API Details
* <ins>**Target Framework**</ins>: .NET 7
* <ins>**Project IDE**</ins>: Visual Studios 2019+ [recommended to use 2022]
* Discord Application Development Portal
* Latest version of Discord

Ensure these requirements are installed correctly and operational.

## [1] Discord.NET Installation
As noted above, we will be using Visual Studios 2019+. This is required so you can properly
install [Discord.NET](https://discordnet.dev) by managing NuGet packages. 

Discord.NET has a very easy [Getting Started](https://discordnet.dev/guides/getting_started/installing.html?tabs=vs-install%2Ccore2-1) page.
Do the installation portion, then return here.


## [2] Environment setup
If you look at the root directory, you will notice an absent `.env`. This file is used to store credentials for the bot and
database access, so it's pretty important. However, you can probably guess that the contents of this file are **TOP SECRET**.

Go ahead and create an empty text file named exactly `.env` and paste in the following information:

    token=[TOKEN]

    # Center Guild specific data and references
    center_guild_id=[ID]

    #API
    api_url=[ROUTE]

Obviously, we aren't going to post the credentials right here. That would defeat the purpose. You'll need to request them
from other members. Replace each placeholder (notated by [] symbols) with the correct values. Save the file and you should
be good to go.


## [3] Get Familiar with the Project Structure
While this isn't a requirement, it's recommended. Further, ***DOCUMENT EVERYTHING***.
You will be documenting a lot at the [doc](./doc) folder frequently. At least, you should. Here, you will see more 
markdown documents that explain in depth the project and its structure. 
as well as [UML](https://www.google.com/search?client=firefox-b-1-d&q=what+is+UML) documents. 

Be as verbose as possible. It helps everyone, including you. 

If you aren't familiar with UML, that's okay. It's not a difficult design style to understand. 
A simple [article](https://www.geeksforgeeks.org/unified-modeling-language-uml-introduction/) 
and a quick [YouTube video](https://www.youtube.com/watch?v=b8VMFa3Cdbo) should suffice. 
To generate our UML graphs, we will be using Visual Studio's class diagrams. 

Finally, although this isn't part of the project's physical structure, use the [Project](https://github.com/Official-Goat-Studios/DiscordSpaceGame/projects) 
tab frequently. This is where
we all can see who is working on what, what needs to be done, what has been completed, and what needs to be fixed.
Not everyone is familiar or used to working with this kind of workflow, but it's very helpful. 

Also, you will see localization. Use them! Better to be building the bot around localization now rather than later.

## [4] Final Links
A couple of documents have been created for development and documentation, all stored in a Google Drive folder. 
For the time being, I imagine you'll only need two of them:  
* [Designer's Board](https://docs.google.com/document/d/1MoGaKJ6IhF7BYauHbj_gM9ck9hYuIrSHq6V5uJBAYwM/edit?usp=sharing) 
* [Wormhole Center](https://docs.google.com/document/d/1NC0DRjoWszKxxr7eIKWcSvdwAp_reIKeqHtT9Qie6ps/edit?usp=sharing)

For both of these documents, you should have at least viewing permissions, but you can request editing permissions.


**Some important documentation:**
* [Discord.NET](https://discordnet.dev/api/index.html): Discord API service for C#
* [MongoDB](https://www.mongodb.com/docs/drivers/csharp/current/): MongoDB C# Driver information and references.

