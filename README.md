# Introduction 
KnowledgeSpace is a open source project for everyone. Every member can create new knowledge base record (KB) and share to community.
For each KB, user can vote it and comment to below KB.

# Migration
- Add-Migration Initial -OutputDir Data/Migrations

#Technology Stack
1. ASP.NET Core 3.1
2. Angular 8
3. Identity Server 4
5. SQL Server 2019

# How to run this Project
1. Clone this source code from Repository
2. Build solution to restore all Nuget Packages
2. Set startup project is KnowledgeSpace.BackendServer
3. Run Update-Database to generate database
4. Set startup project to multiple projects include: KnowledgeSpace.BackendServer and KnowledgeSpace.WebPortal

# References
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-3.1)
- [Visual Studio](https://visualstudio.microsoft.com/)
- [IdentityServer4](https://identityserver.io/)
