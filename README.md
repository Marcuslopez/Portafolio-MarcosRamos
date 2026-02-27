# ERP Lite -- Enterprise Full Stack Architecture Portfolio

## Overview

ERP Lite is a modular enterprise-style system developed as a
professional portfolio project to demonstrate advanced backend
architecture, security, transactional integrity, and multi-technology
integration.

This project showcases a clean layered architecture using .NET 8, SQL
Server with Stored Procedures, Node.js (Realtime), Python (Computation
Engine), and modern frontend integrations.

------------------------------------------------------------------------

# Architecture Overview

Frontend (MVC / Angular) │ ▼ ASP.NET Core Web API (.NET 8) │ ├── SQL
Server (Stored Procedures + Transactions) ├── Node.js Realtime Service
(Socket.IO) └── Python Quote Engine (FastAPI)

------------------------------------------------------------------------

# Technology Stack

## Backend

-   ASP.NET Core 8
-   ADO.NET
-   SQL Server
-   Stored Procedures
-   JWT Authentication
-   Role-Based Authorization
-   Policy-Based Authorization
-   Swagger

## Realtime

-   Node.js
-   Express
-   Socket.IO

## Computation Engine

-   Python
-   FastAPI

## Frontend

-   ASP.NET MVC
-   Angular 21
-   Bootstrap
-   JavaScript

## Cloud & DevOps

-   Azure App Service (Linux)
-   Azure SQL Database
-   Azure Container Apps
-   GitHub

------------------------------------------------------------------------

# Project Structure

MasterPortafolio ├── WebAPIMRL → REST API (.NET 8) ├──
ClassApplicationMRL → Business Logic Layer ├── ClassDataMRL → Data
Access Layer ├── ERPLite.Domain → Entities & DTOs ├──
RealtimeServiceNode → WebSocket Service ├── QuoteServiceNode → Quote
Orchestrator (Node) ├── QuoteEnginePython → Calculation Engine (Python)
├── PortalWebMRL → MVC Web Portal └── PortalAngularMRL → SPA Angular 21

------------------------------------------------------------------------

# Security Architecture

-   JWT Authentication
-   Role-based authorization
-   Policy-based authorization (AdminByIdRol)
-   Token validation with issuer and audience
-   Secure CORS configuration
-   Separation of DTOs and Entities

Roles Implemented: - Administrator - User

------------------------------------------------------------------------

# Database Design

-   SQL Server
-   Fully stored procedure-driven CRUD
-   Operations controlled by flags:
    -   OpAdd
    -   OpMod
    -   OpDel
    -   OpGet
    -   OpList
-   Transactional order creation
-   Stock validation at database level
-   Logical delete via Activo flag

------------------------------------------------------------------------

# Core Functionalities

## Products

-   Create / Update / List
-   Stock control
-   Category association
-   Logical activation/deactivation

## Clients

-   Full CRUD
-   Linked to Orders

## Orders

-   Transactional creation with detail
-   Stock validation
-   Status transitions:
    -   Created
    -   Paid
    -   Cancelled
-   Real-time notifications

## Quotation Engine

-   Multi-item calculation
-   Discount percentage
-   Tax percentage
-   Externalized calculation via Python engine

## Realtime Notifications

-   Socket.IO implementation
-   Severity levels (success, warning, error, info)
-   LocalStorage persistence
-   Event broadcasting after order changes

------------------------------------------------------------------------

# Deployment Strategy (Azure)

Recommended Production Architecture:

-   API → Azure App Service (Linux B1)
-   Database → Azure SQL Database
-   Node Realtime → Azure Container Apps
-   Python Engine → Azure Container Apps
-   Angular → Azure Static Web Apps

------------------------------------------------------------------------

# Running Locally

## Database

-   Execute SQL scripts
-   Configure connection string in appsettings.json

## API

dotnet run

## Realtime Node

cd RealtimeServiceNode npm install node server.js

## Quote Engine

cd QuoteEnginePython uvicorn main:app --port 8001 --reload

## Angular

cd PortalAngularMRL ng serve

------------------------------------------------------------------------

# Architectural Principles Demonstrated

-   Clean layered architecture
-   Separation of concerns
-   Transaction management
-   Multi-language microservice integration
-   Secure authentication design
-   Enterprise-ready REST design
-   Realtime event-driven integration
-   Cloud deployment readiness

------------------------------------------------------------------------

# Professional Objective

This project demonstrates enterprise backend engineering capabilities,
security best practices, distributed system integration, and full-stack
architectural understanding suitable for mid-to-senior backend roles.

------------------------------------------------------------------------

Author: Professional Full Stack Developer Portfolio Project
