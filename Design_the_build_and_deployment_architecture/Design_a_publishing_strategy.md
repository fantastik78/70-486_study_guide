 > This chapter should cover:
> - [Implement application publishing using dotnet.exe]()
> - [Manage publishing options in csproj]()
> - [Implement additional tooling]()
> - [Implement pre-publish and post-publish scripts]()
> - [Implement native compilation]()
> - [Publish to Docker container image]()

## Implement application publishing using dotnet.exe

### Framework-dependent deployments

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
`dotnet publish -c Release -o c:\temp\destination_folder`


### Self-contained deployments

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
`dotnet publish -c Release -o c:\temp\destination_folder --self-contained -r win-x64`

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
