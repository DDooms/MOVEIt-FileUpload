#MOVEIt File Uploader - Setup & Usage Guide

#Overview

This is a console application that automatically uploads files to MOVEit Cloud when new files are added to a specified local folder. 
It monitors the folder and uploads new files to the user's home directory on MOVEit Cloud.

Cloning the Repository
First, download or clone the repository from GitHub

Setting Up Configuration
Before running the application, you need to configure the Config.json file. The application will not work unless this file is properly set up.

Description of Config Fields:
MoveItBaseUrl: The base URL for MOVEit Cloud API. (it should be like this)
Username: Your MOVEit Cloud username.
Password: Your MOVEit Cloud password.
LocalFolderPath: The local folder to monitor for new files.

You have to add username and password credentials, so you can authenticate yourself. Also, a local path should be specified from where files would be uploaded
e.g D\\MOVEIt

Running the Application
Once the configuration is set up, you can run the application using:

 `dotnet run`

This will authenticate with MOVEit Cloud and start monitoring the specified local folder. The console will display:

`Authentication successful.
Monitoring folder. Press Enter to exit.`

Any new file added to the folder will be automatically uploaded to MOVEit Cloud.

How It Works

Authentication: The application authenticates using the username and password provided in Config.json. It retrieves an access token and stores it for making API requests.
Retrieving Home Folder ID: After authentication, the application fetches the user’s home folder ID from MOVEit Cloud.
Monitoring the Folder: The application watches the specified local folder for new files.
Uploading Files: When a new file is detected, it is uploaded to the user’s home folder on MOVEit Cloud.

Checking Uploaded Files in MOVEit Cloud

After files are uploaded, you can verify them in your MOVEit Cloud account:
Log in to your MOVEit Cloud account.
Navigate to Files & Folders.
Open the Home Folder (or the folder ID retrieved by the application).
You should see the uploaded files there.
