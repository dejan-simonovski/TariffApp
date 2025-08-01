# TariffApp Summary
This app is created using .Net 8 Core, it utilizes the MVC design pattern.

During the development of this app, the models were firstly mapped out and created. Every controller has its adequate views that were scaffolded for time efficiency.
Every controller uses services and repositories as standard. The database that is used for this small simple presentation is a local database which can be upgraded to an actual connection if needed.

A compromise that was made during this development was the importing of JSON, in this implementation, additionally you can also add clients and transactions manually, not just by JSON to make things easier during development, and recycling already made functions.
Additional fee charges were added as well as CRUD operations. Additionally, because we are using a local database due to the projects small size, in order to dockerize the project into a container, it was necessary to move from SQL to SQLite because of 'localdb' not being supported within a linux docker container.

If there were no time limitations, I would most likely add multiple select for delete, login, deploying an actual database, more tests (more detailed ones) and etc.

Note: This is a small-scale, time-boxed project built for a 48-hour coding challenge. Due to the short development cycle and limited scope, all work was committed directly to the main branch for simplicity and speed. In a production setting or for larger projects, feature branching and pull requests would be used to ensure code quality and collaboration. **Never** commit directly to main. Additionally, the database has no credentials or such and is pushed alongside the project. Examples of JSOn are within the repository and inside the app itself as a 'guide' on how to insert data.

# Docker Deployment Guide

This guide explains how to deploy and run the TariffApp (.NET MVC) using Docker. You can either pull a pre-built image from Docker Hub (**Approach 1**) or build the image yourself locally (**Approach 2**).

---

## Prerequisites

Make sure the following is installed on your system:

### Docker Desktop
- Download:  
  [https://www.docker.com/products/docker-desktop](https://www.docker.com/products/docker-desktop)
- Install and start Docker Desktop.
- Verify installation:
  ```bash
  docker --version

## Approach 1: Pull from Docker Hub (Recommended)

Skip building the image locally, just pull and run.

### 1. Pull the image
```
docker pull dejans12/tariffapp:latest
```

### 2. Run the container
```
docker run -d -p 5000:8080 --name tariff-container dejans12/tariffapp:latest
```

### 3. Access the application
Open your browser and go to:  
[http://localhost:5000](http://localhost:5000)

---

##  Approach 2: Build Locally from Source

Building the image yourself.

### 1. Clone or download the repository
Place the project in your desired location.

### 2. Navigate to the project directory
Open your terminal or command prompt and go to the folder containing the Dockerfile.

### 3. Build the Docker image
```
docker build -t tariffapp .
```

### 4. Run the container
```
docker run -d -p 5000:8080 --name tariff-container tariffapp
```

### 5. Access the application
Go to:  
[http://localhost:5000](http://localhost:5000)

  
