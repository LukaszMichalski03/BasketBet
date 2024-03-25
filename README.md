# **BasketBet**
A simulation of a sports betting application written in Asp.Net MVC along with a WebAPI that updates real-time NBA League statistics. Includes:

* Creating and logging into accounts stored in a database.
* Updating real-time statistics in a database using a WebAPI that utilizes HTMLAgilityPack.
* Betting on real matches.
* User rankings within the application.
* Real NBA statistics.

### Project Setup
* To run this project you need to set your own **connection string** for the database, which can be found in [databasesettings.json](BasketBet.EntityFramework/databasesettings.json) file
* To build the database, you need to set the startup project to BasketBet.MVC. In the NuGet Package Manager Console, set BasketBet.EntityFramework as the default project. Then, you can build the database using **'update-database'**. Once done, you should revert the startup projects to BasketBetWebAPI and BasketBet.MVC.
### Project Structure
#### BasketBetWebApi
* A WebAPI that utilizes HTMLAgilityPack to update data in the database.
#### BasketBet.Web
* A sports betting application with a system for receiving free points, the ability to place bets with them, featuring NBA statistics, and user rankings.
#### BasketBet.EntityFramework
* A library of classes containing the structure of a shared database.
