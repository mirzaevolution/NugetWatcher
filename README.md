# NugetWatcher
A simple tool for daily task to handle local generation of nuget by monitoring changes inside bin/debug folder(s) of the monitored project(s)


![NugetWatcher](https://raw.githubusercontent.com/mirzaevolution/NugetWatcher/master/Assets/2021-12-27_02h14_54.png)

**Nuget Watcher** is a really basic tool to monitor directories of .NET Library Projects in which it will generate local nuget packages in C:\Users\<User_Profile>\NugetWatcherRepo by reading the local.nuspec profile within those monitored directory of the projects.

![NugetWatcher](https://raw.githubusercontent.com/mirzaevolution/NugetWatcher/master/Assets/2021-12-27_02h25_18.png)

Basically, this solution contains one project with two supporting files to make this tool work as expected. 

- **dirs.txt**: A text file which contains raw list of monitored directories. The directory itself pointed where the *.csproj file is located. Under this directory itself, a nuspec file called **local.nuspec** must also exist
![NugetWatcher](https://raw.githubusercontent.com/mirzaevolution/NugetWatcher/master/Assets/2021-12-27_02h15_45.png)

- **nuget.exe**: An executable file used to pack the nuget file based on nuspec file defined in the monitored directories




In the monitored project directory, you should create a local.nuspec file where the structure of the nuspec can be found in the [Nuget Official Documentation](https://docs.microsoft.com/en-us/nuget/). 

Here's the sample of screenshot where the target directory/project was called WeatherForecast.Library.
![NugetWatcher](https://raw.githubusercontent.com/mirzaevolution/NugetWatcher/master/Assets/2021-12-27_02h16_39.png)

After you have created the **local.nuspec** file, and the directory location been added in the **dirs.txt** (in **NugetWatcher** folder), then you just need to run the app, and make changes to the projects you monitor and nuget package(s) will be automatically generated in C:\Users\<User_Profile>\NugetWatcherRepo folder.

![NugetWatcher](https://raw.githubusercontent.com/mirzaevolution/NugetWatcher/master/Assets/2021-12-27_02h18_23.png)

After that, you can integrate that nuget package(s) to your project(s) that need them.

![NugetWatcher](https://raw.githubusercontent.com/mirzaevolution/NugetWatcher/master/Assets/2021-12-27_02h19_16.png)

![NugetWatcher](https://raw.githubusercontent.com/mirzaevolution/NugetWatcher/master/Assets/2021-12-27_02h20_13.png)


© 2021 - **Mirza Ghulam Rasyid**
