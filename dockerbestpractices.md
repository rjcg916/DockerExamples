# Overview

# Tools and Technologies
   
To support creation and publishing of Docker Images, the following tools and technologies are used:

- VSTS
    - Source Code Control (GIT)
    - Build Pipeline

- Docker
    - Container Definition
    - Container Testing/Integration (via Docker Componse)

- Node/NPM
    - JS Package Management
    - Install/Build Scripting
  
- Nuget
    - .NET Package Management 
  
- Visual Studio / SDK
    - MS Build Services
    - dotnet build Services

# Building and Publishing Docker Images
Docker Images are created from Source Code following the steps described below:

1. Source Code Acquisition
2. Docker Image Build
3. Docker Image Publish
    

## 1. Source Code Acquisition


The build pipeline accesses source code via a preconfigured reference to a VSTS/GIT repository and branch.  The build pipeline is configured for "Continuous Integration, meaning that any commits to the source branch will trigger the execution of the build pipeline.

## 2. Image Build


The build pipeline uses "docker build" to create the images.  Docker images are created using the steps contained in a Dockerfile (which is obtained from the build pipeline Repo). 

To minimize the size of containers, a two-stage build process is specified within the Dockerfile. During the first phase, a "builder" container is used to generate run-time components (e.g. restore and compile).  In the second phase, run-time components are copied into the final container. Since the Docker image is only based upon the contents of the final container, build-only files are discarded. 

Conceptually, a two-stage image build works as follows:

1. Specify builder base image as current layer. Then, using the tools within the now running builder layer . . . 
    - restore files required for package restore
    - restore packages
    - copy source code files
    - compile and publish needed components
  

2. Specify run-time base image as current layer.  Then, configure run-time container as foundation for final, run-time image . . .
    - copy run-time components from builder layer
    - customize run-time environment
        - e.g. create web-site
    - expose ports using EXPOSE
    - define monitor coded using HEALTHCHECK
 
See example at the end of this page.

### Image Build Best Practices


| Practice           | Rationale  |
|:-------------------:| :-----------:|
|  Use appropriate builder and runtime Base Images |  Builders contain specific applications and support particular commands |
|  Specify specific versions of Base Images| Avoid breaking your Dockerfile as a result of untested base Image changes
| Define specific COPY and RUN steps for package restoration / installation | This will increase layer re-use and make subsequent re-builds faster
| Define specific COPY and RUN steps for program build/compilation | This will limit layer re-build to only cases where new files have been introduced
| Minimize number of commands e.g. use ';' for PowerShell commands | This will limit number of layers that must be created
| Use fully qualify names for external resources |Otherwise, IP addressed would need to be used
| Use "COPY --from" to obtain files from Build phase into Runtime layer | Allows selective use of files from build container
| Include EXPOSE as appropriate | Allows use of -P Docker Run option
| Include appropriate HEALTHCHECK | Supports uptime/recovery monitoring

In addition, the Docker site contains the following best practices: 
- [Leveraging Build Cache](https://docs.docker.com/develop/develop-images/dockerfile_best-practices/#leverage-build-cache)
- [Copy/Add and Build Cache](
https://docs.docker.com/develop/develop-images/dockerfile_best-practices/#add-or-copy)



### Builder Containers and Base Images

Constructed from the appropriate platform base image, the builder images contain tools required to generate run-time components.  These tools include the following: 

- visual studio sdks: compiler
- nuget: package management (.net)
- node/npm: package management ( web/javacript)
    
Different builder images exists targeted to distinct platforms including the following:

- .net core
- asp .net core
- .net framework
- asp .net framework


### Runtime Containers and Base Images

Constructed from the appropriate platform base image, the runtime images are configured to support specific runtime requirements such as:

- IIS Hosting
- Windows Authentication
- Certificate Management

Different run-time images exists for the following application types:

- .net core
- asp .net core
- .net framework
- asp .net framework


## 3. Docker Image Publish

Docker images are published to the private Docker registry via the last step of the build pipeline.

# Development Environment and Workflow

## Setup
- Windows Server
    - if Windows 10/VDI
- Docker for Windows
- Git for Windows
- Visual Studio
- Visual Studio Code w/Extensions for Docker
- Local IIS
- Rabbit MQ development instance-  using full qualified address
- SQL server development instance - using SQL Server User
 - Authentication instance
  

          
## Workflow

Below is a workflow for already existing solutions (This workflow is assuming Windows 10 where docker build must be run remotedly)

1.  Connect with the VSTS Repo; pull down to local machine; work to get a clean release build
    - Create Branch -> magenic/docker
    - Connect to a Project
    - Review Solution
    - Update .Gitignore
    - Determine Containerization approach - how many, what names
2. Get solution running locally using test cases and dependencies including IIS solutions, Rabbit, etc.
    - Use Publish to smoke out issues
    - Look for npm install, grunt, etc., in addition to Visual Studio build
        - SPA code may or may not be integrated into Visual Studio build definition
3.  Copy previous Dockerfile that best match current solution 
    - Setup: create a new pipeline for solution + enable continuous integration
    - Edit Dockerfile using VS Code
        - Build Layer using Base Build Image
        - Runtime Layer using Base Runtime Image
            - (if necessary) Edit Solution using Visual Studio
        - Update VSTS repo with code
                - Commit will trigger pipeline
        -  Review pipeline build output in VSTS console
        -  Repeat these steps until get a clean container build
		NOTE: update base build and/or run-time image if necessary
4. Perform “Docker Run” on newly created image – reference Jabil Local Repo -> image pulled down to local machine
- Use VS Code Docker Extension to manage images, containers, etc.
- Use docker inspect to get IP, etc.
    - Test container
        - Docker Compose
        - Service Fabric
  
## Application Configuration

Files
- Appsettings.json
- web.config
- .npmrc
- nuget.config
- SPA settings
  
Settings
- SQL
- Rabbit
- Auth
- Other Service Endpoints

## Container Testing
- Docker
- Docker Compose
- Connect with Authentication
  
## Authentication
- gMSA doesn't work on Windows 10
    - solution: use Windows Server with GMSA (service fabric machine)


# Appendix


## Dockerfile Example


 A simplified example is as follows:

```
# escape=
 
#create builder container from published image
FROM azuse2dtc01.corp.jabil.org:5000/it-stp-df_devops/dotnet-builder-mvc as AppBuild

# Restore the default Windows shell for correct batch processing below.
SHELL ["cmd", "/S", "/C"]

# Selectively fetch only files required for restore/install`
COPY package.json      /Source/package.json`
COPY .npmrc            /Source/.npmrc`
COPY gulpfile.js       /Source/gulpfile.js`
COPY ./src/PhoenixApi/package.json      /Source/src/PhoenixApi/package.json
COPY ./src/PhoenixApi/package-lock.json /Source/src/PhoenixApi/package-lock.json
COPY ./src/PhoenixApi/gulpfile.js       /Source/src/PhoenixApi/gulpfile.js
COPY ./src/PhoenixApi/.config/          /Source/src/PhoenixApi/.config/ 

WORKDIR /Source

# Restore packages (NOTE: npm is in builder container)
RUN npm install

WORKDIR /Source

# Restore any/all packages

# Copy only those files necessary for restore process
COPY nuget.config /Source/src/nuget.config 
COPY ./src/ngMES.Assemble/packages.config  c:/source/src/ngMES.Assemble/packages.config 
RUN Nuget.exe restore -PackagesDirectory c:/source/src/packages `-ConfigFile c:/source/src/nuget.config c:/source/src/ngMES.Assemble/packages.config 
COPY ./src/PhoenixApi/packages.config c:/source/src/PhoenixApi/packages.config
RUN Nuget.exe restore -PackagesDirectory c:/source/src/packages -ConfigFile c:/source/src/nuget.config  c:/source/src/PhoenixApi/packages.config 

# At this point, copy all code for build

COPY . .
RUN c:/buildtools/msbuild/15.0/bin/msbuild.exe c:/source/src/PhoenixApi/PhoenixApi.csproj /maxcpucount /p:DebugSymbols=false /p:Configuration=Release /p:PublishProfile=FolderProfile /p:DeployOnBuild=true

#create final container starting with base, run-time image
FROM azuse2dtc01.corp.jabil.org:5000/it-stp-df_devops/iis-aspnet-fullframework

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'Continue'; $verbosePreference='Continue';"]

#copy component from builder container to runtime image

COPY --from=AppBuild /Source/src/PhoenixApi/bin/Release/publish /app

## create IIS Site with Authorization
## NOTE: builder image hosts IIS

RUN New-WebAppPool APP;`
    Set-ItemProperty IIS:\AppPools\APP managedRuntimeVersion '';
    New-Website -Name 'APP' -PhysicalPath 'c:\app' -ApplicationPool 'APP' -Port 80 | Start-Website ; 
    Set-WebConfigurationProperty -filter /system.webServer/security/authentication/anonymousAuthentication -name enabled -value false -PSPath IIS:\Sites -location APP; 
    Set-WebConfigurationProperty -filter /system.webServer/security/authentication/windowsAuthentication -name enabled -value true -PSPath IIS:\Sites -location APP;  

##  Provide monitoring for service based upon representative/test endpoint

HEALTHCHECK CMD powershell -command   
    try \{ 
     \$response = Invoke-WebRequest -Uri http://localhost/endpoint -UseBasicParsing; `
     if (\$response.StatusCode -eq 200) { return 0} 
     else {return 1}; 
  \} catch { return 1 }

# Port for external access
EXPOSE 80

```




