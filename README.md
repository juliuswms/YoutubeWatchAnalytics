# YoutubeWatchAnalytics
Ever wondered how much time you spend watching Youtube Videos? Find it out now!

This project is a tool for analyzing YouTube Watch History data, including video titles, lengths, creators, and more. The project uses the YouTube Data API v3 to gather information about each video, and then performs various analyses on the data to help users better understand their video watching habits.

## Getting Started
To use this tool, you'll need to obtain a Google API key for the YouTube Data API v3. You can get a key by following these steps:

1. Go to the Google [Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select an existing one
3. Navigate to the "APIs & Services" section and select "Dashboard"
4. Click the "+ ENABLE APIS AND SERVICES" button and search for "YouTube Data API v3"
4. Click the "ENABLE" button to enable the API for your project
5. Navigate to the "Credentials" section and click the "+ CREATE CREDENTIALS" button
6. Select "API key" and copy the key that is generated

## Usage
To use the tool, simply run the YouTubeAnalyzer project. The program will prompt you to enter the path to a file containing a list of video IDs (one per line). You can obtain your own watch history data by following the steps outlined in the "Obtaining Watch History Data" section below.

Once you've provided the program with the watch history data, it will query the YouTube Data API for information about each video and perform several analyses on the data, including:

* Total count of all videos
* Total length of all videos
* Total count of creators
* Top 3 creators by video count

The results of the analyses will be displayed in the console.

## Obtaining Watch History Data
To obtain your own watch history data, follow these steps:

1. Go to Google and sign in to your account
2. Click on your Profil
3. Select "Manage your Google Account"
4. Select "Data and privacy"
5. Click on "[Download your Data](https://takeout.google.com/)" at the bottom of the page
6. Now deselect everything except for "Youtube and Youtube Music"
7. Under "All Youtube data formats" select only "history"
8. Under "Multiple formats" select JSON
9. Now click on "Next step" and than "Create export"
Once the export is complete, you'll receive an email with a link to download the data. Download the file and extract the contents to a directory on your computer.

### Dependencies
This project uses the following dependencies:

Newtonsoft.Json (version 13.0.1)
Google.Apis.YouTube.v3 (version 1.51.0.460)

These dependencies are managed using NuGet.
