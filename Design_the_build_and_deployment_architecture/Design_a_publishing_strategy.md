 > This chapter should cover:
> - [Implement application publishing using dotnet.exe](#implement-application-publishing-using-dotnetexe)
> - [Manage publishing options in csproj](#manage-publishing-options-in-csproj)
> - [Implement additional tooling](#implement-additional-tooling)
> - [Implement pre-publish and post-publish scripts](#implement-pre-publish-and-post-publish-scripts)
> - [Implement native compilation](#implement-native-compilation)
> - [Publish to Docker container image](#publish-to-docker-container-image)

## Implement application publishing using dotnet.exe

### Framework-dependent deployments (FDD)

In a framework-dependent deployments, your app will use the version of .NET Core that's present on the target system.

Advantages:
 * .NET Core uses a common PE file format for executables and libraries regardless of operating system, .NET Core can execute your app regardless of the underlying operating system
 * The size of your deployment package is small
 * It use the latest serviced runtime installed on the target system
 * If multiple apps use the same .NET Core installation, it reduces both disk space and memory usage on host systems 

Disadvantages:
 * Your app can run only if the version of .NET Core your app targets.
 * The .NET Core runtime and libraries to change without your knowledge in future releases. In rare cases, this may change the behavior of your app.

#### How to use dotnet.exe in a framework-dependent deployments strategy
Open a CMD inside the same folder of [project].csproj file, then call  
`dotnet publish -c Release -o c:\temp\destination_folder [--self-contained false]`

### Self-contained deployments (SCD)

In a self-contained deployments, you deploy your app and any required third-party dependencies along with the version of .NET Core that you used to build the app.

Advantages:
 * You have sole control of the version of .NET Core that is deployed with your app.
 * You can be assured that the target system can run your .NET Core app

Disadvantages:
 * You must select the target platforms for which you build deployment packages in advance.
 * The size of your deployment package is relatively large
 * Deploying numerous self-contained .NET Core apps to a system can consume significant amounts of disk space

#### How to use dotnet.exe in a framework-dependent deployments strategy

Open a CMD inside the same folder of [project].csproj file, then call  
`dotnet publish -c Release -o c:\temp\destination_folder --self-contained true -r <RID>`

#### Runtime IDentifier (RID) 
RID is short for Runtime IDentifier. RID values are used to identify target platforms where the application runs. They're used by .NET packages to represent platform-specific assets in NuGet packages.  

The following list shows a small subset of the most common RIDs used for each OS

##### Windows
 Portable (.NET Core 2.0 or later versions)
  * win-x64
  * win-x86
  * win-arm
  * win-arm64
* Windows 7 / Windows Server 2008 R2
  * win7-x64
  * win7-x86
* Windows 8.1 / Windows Server 2012 R2
  * win81-x64
  * win81-x86
  * win81-arm
* Windows 10 / Windows Server 2016
  * win10-x64
  * win10-x86
  * win10-arm
  * win10-arm64

##### Linux

* Portable (.NET Core 2.0 or later versions)
  * linux-x64 (Most desktop distributions like CentOS, Debian, Fedora, Ubuntu and derivatives)
  * linux-musl-x64 (Lightweight distributions using musl like Alpine Linux)
  * linux-arm (Linux distributions running on ARM like Raspberry Pi)
* Red Hat Enterprise Linux
  * rhel-x64 (Superseded by linux-x64 for RHEL above version 6)
  * rhel.6-x64 (.NET Core 2.0 or later versions)
* Tizen (.NET Core 2.0 or later versions)
  * tizen
  * tizen.4.0.0
  * tizen.5.0.0

##### macOS RIDs

* Portable (.NET Core 2.0 or later versions)
  * osx-x64 (Minimum OS version is macOS 10.12 Sierra)
* macOS 10.10 Yosemite
  * osx.10.10-x64
* macOS 10.11 El Capitan
  * osx.10.11-x64
* macOS 10.12 Sierra (.NET Core 1.1 or later versions)
  * osx.10.12-x64
* macOS 10.13 High Sierra (.NET Core 1.1 or later versions)
  * osx.10.13-x64
* macOS 10.14 Mojave (.NET Core 1.1 or later versions)
  * osx.10.14-x64

## Manage publishing options in csproj

### Exclude files

Example, exclude all .txt files in wwwroot/:
```xml
<ItemGroup>
  <Content Update="wwwroot/**/*.txt" CopyToPublishDirectory="Never" />
</ItemGroup>
```

### Include files

Include node_modules folder in the publish directory:
```xml
<ItemGroup>
    <Content Include="node_modules/**" CopyToPublishDirectory="PreserveNewest" />
</ItemGroup>
```

Or another example, includes an images folder outside the project directory to the wwwroot/images folder:
```xml
<ItemGroup>
  <_CustomFiles Include="$(MSBuildProjectDirectory)/../images/**/*" />
  <DotnetPublishFiles Include="@(_CustomFiles)">
    <DestinationRelativePath>wwwroot/images/%(RecursiveDir)%(Filename)%(Extension)</DestinationRelativePath>
  </DotnetPublishFiles>
</ItemGroup>
```

## Implement additional tooling
## Implement pre-publish and post-publish scripts

Pre-Publish scripts:
```xml
<Target Name="CustomActionsBeforePublish" BeforeTargets="BeforePublish">
    <Message Text="Inside BeforePublish" Importance="high" />
    <Exec Command="npm install">
</Target>
```

Post-Publish scripts:
```xml
<Target Name="CustomActionsAfterPublish" AfterTargets="AfterPublish">
    <Message Text="Inside AfterPublish" Importance="high" />
</Target>
```

## Implement native compilation
## Publish to Docker container image
