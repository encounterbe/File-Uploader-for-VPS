# File-Uploader-for-VPS

# Instructions:
1. Navigate to line 39, which contains the following line: using (TcpClient client = new TcpClient("SERVERIPADRESS", 20089)).
2. Remove "SERVERIPADRESS".
3. Insert the IP address of your server in place of "SERVERIPADRESS".

## Contents
- Client: The client application allows users to select files from their local PC and upload them directly to the VPS. It displays the upload progress, speed, and estimated time remaining.
- Server: The server application runs on the VPS, receiving uploaded files from the client application and storing them in a specified directory.

## Description
File Uploader for VPS is a lightweight client-server solution designed to enable seamless file transfers between a local PC and a virtual private server (VPS). This project provides an efficient method for uploading files directly from a user's local machine to a designated directory on a VPS without relying on third-party services.

The solution includes a client application that allows users to select files from their local PC and upload them directly to the VPS. The client displays upload progress, speed, and estimated time remaining. The server application runs on the VPS and listens for incoming file uploads. It receives the uploaded files from the client and stores them in a specified directory on the server.

## Key features of the project include:
- Fast and direct file uploads from a local PC to a VPS.
- Displaying upload progress and speed in real time on the client side.
- Estimating the time remaining for the upload to complete.
- Simple and straightforward setup and configuration for both client and server applications.
- The project is ideal for users who host their servers on a VPS and need a quick and reliable method to transfer files directly to the server. 
