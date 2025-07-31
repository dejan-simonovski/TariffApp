# TariffApp Summary
This app is created using .Net 8 Core, it utilizes the MVC design pattern.

During the development of this app, the models were firstly mapped out and created. Every controller has its adequate views that were scaffolded for time efficiency.
Every controller uses services and repositories as standard. The database that is used for this small simple presentation is a local database which can be upgraded to an actual connection if needed.

A compromise that was made during this development was the importing of JSON, in this implementation, additionally you can also add clients and transactions manually, not just by JSON to make things easier during development, and recycling already made functions.
Additional fee charges were added as well as CRUD operations.

If there were no time limitations, I would most likely add multiple select for delete, login, deploying an actual database, more tests and etc.

# Docker Deployment Guide

This guide explains how to build and run the TariffApp (.NET MVC) using Docker.

## Prerequisites

Before you start, make sure the following is installed on your system:

### Docker Desktop
- Download:  
  [https://www.docker.com/products/docker-desktop](https://www.docker.com/products/docker-desktop)

- Install and start Docker Desktop.
- Verify installation:
  ```bash
  docker --version
  ```
### Commands
- Open docker and navigate to the directory of the Dockerfile
- Run the following in command prompt to build the docker image
  ```bash
  docker build -t tariffapp .
  ```
- Run the docker image with
  ```bash
  docker run -d -p 5000:8080 --name tariff-container tariffapp
  ```
- Access the application via localhost:5000
- To stop the container presss STOP on the container via docker desktop. (blue square icon) or type the following in the command prompt 
  ```bash
  docker stop tariff-container
  ```
- To remove the container you can either press the red trash can icon on docker desktop or use the following in the command prompt
  ```bash
  docker rm tariff-container
  ```

  
