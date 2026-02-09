---
title: 'MongoDB Desktop Viewer'
slug: 'mongodb-desktop-viewer'
created: '2026-02-09T13:37:07.059Z'
status: 'ready-for-dev'
stepsCompleted: [1, 2, 3, 4]
tech_stack: ['.NET 10.0 MAUI', 'Windows + MacCatalyst', 'CommunityToolkit.Mvvm', 'MongoDB.Driver']
files_to_modify: ['Monbsoft.MongoLite.MApp.csproj', 'MauiProgram.cs', 'AppShell.xaml', 'AppShell.xaml.cs', 'MainPage.xaml', 'MainPage.xaml.cs']
code_patterns: ['MVVM with CommunityToolkit.Mvvm', 'Shell Navigation', 'Dependency Injection', 'Generic JSON handling']
test_patterns: ['No existing test framework - to be determined']
---

# MongoDB Desktop Viewer - Technical Specification

## Overview

### Problem Statement
Users need a desktop application to explore, view, edit, and delete MongoDB documents and collections directly from their Windows or macOS computers.

### Solution
A .NET MAUI desktop application using MVVM architecture with CommunityToolkit.Mvvm, connected directly to MongoDB using the official MongoDB driver. The app provides simple, intuitive navigation through MongoDB collections and documents with full CRUD operations on documents for Windows and macOS platforms.

### Scope
**In Scope:**
- Connection to existing MongoDB databases
- Navigation between collections in the database
- List view of available collections
- List view of documents within a selected collection
- Document detail view
- Document editing and saving
- Document deletion
- Shell navigation between screens
- Support for generic JSON documents
- Simple, clean UI focused on functionality
- Docker development environment with sample data

**Out of Scope:**
- Database creation/deletion
- Collection creation/deletion (read-only for collections)
- Advanced MongoDB authentication
- Offline synchronization
- Complex UI/UX features
- Aggregation queries or advanced filtering

## Context for Development

### Technical Requirements
- Use CommunityToolkit.Mvvm for MVVM implementation
- Use Shell navigation for screen transitions
- Handle MongoDB documents as generic JSON objects
- Support .NET MAUI desktop (Windows + MacCatalyst only)
- Include MongoDB.Driver NuGet package
- Keep UI simple and functional

### Project Structure
- Existing .NET MAUI project at `Monbsoft.MongoLite.MApp/`
- Current template uses code-behind, will refactor to MVVM
- Add Services/, Models/, and ViewModels/ folders
- Create pages for: Connection, Collections List, Documents List, Document Detail/Edit
- Add Docker/ folder for development environment setup

### Codebase Patterns (Confirmed Clean Slate)
- **Architecture**: Standard .NET MAUI template with code-behind
- **Navigation**: Basic Shell navigation in AppShell.xaml
- **Dependency Injection**: None currently - will add in MauiProgram.cs
- **File Organization**: Flat structure in root - will organize into folders
- **No existing constraints**: Clean slate for MVVM implementation

### Files to Reference
| File | Purpose | Current State |
|------|---------|---------------|
| `Monbsoft.MongoLite.MApp.csproj` | Project configuration | Basic MAUI template |
| `MauiProgram.cs` | App startup/DI | Standard template |
| `AppShell.xaml/.cs` | Navigation routing | Single route to MainPage |
| `MainPage.xaml/.cs` | Main UI | Template counter example |

### Technical Decisions
- Target Windows + MacCatalyst only (remove mobile targets from csproj)
- Use CommunityToolkit.Mvvm for MVVM infrastructure
- Implement generic document handling with System.Text.Json
- Add MongoDB.Driver NuGet for database connectivity
- Configure DI in MauiProgram for services and ViewModels
- Use Docker Compose for local development database with sample data

## Implementation Plan

### User Stories by Sprint (Value-Driven Delivery)

#### Sprint 1 : MVP Connectivity
**Story 1 : First Connection MVP**
- En tant qu'utilisateur, je veux me connecter à une base de données MongoDB
- **Valeur** : Validation complète de l'architecture technique
- **Acceptance** : Given l'application démarrée, when utilisateur entre connexion valide, then message de succès et structure de base prête

**Story 2 : Docker Quick Start**
- En tant que développeur, je veux démarrer un environnement de test instantané
- **Valeur** : Environnement de développement prêt en 1 commande
- **Acceptance** : Given Docker installé, when `docker-compose up`, then base accessible avec données de test

#### Sprint 2 : Functional Exploration
**Story 3 : Collection Browser**
- En tant qu'utilisateur, je veux naviguer entre les collections de ma base
- **Valeur** : Exploration complète des données disponibles
- **Acceptance** : Given connexion établie, when utilisateur arrive sur collections, then liste complète avec métadonnées

**Story 4 : Document Discovery**
- En tant qu'utilisateur, je veux consulter les documents d'une collection
- **Valeur** : Accès au contenu des données
- **Acceptance** : Given collection sélectionnée, when utilisateur ouvre collection, then liste des documents avec preview JSON

#### Sprint 3 : Complete Manipulation
**Story 5 : Document Editor**
- En tant qu'utilisateur, je pouvoir éditer un document JSON
- **Valeur** : Modification des données en temps réel
- **Acceptance** : Given document sélectionné, when utilisateur modifie JSON, then interface d'édition fonctionnelle

**Story 6 : Data Persistence**
- En tant qu'utilisateur, je pouvoir sauvegarder et supprimer des documents
- **Valeur** : Opérations CRUD complètes
- **Acceptance** : Given document modifié/sélectionné, when utilisateur sauvegarde/supprime, then modifications persistées dans MongoDB

#### Sprint 4 : Production Ready
**Story 7 : Navigation Polished**
- En tant qu'utilisateur, je veux une navigation fluide entre tous les écrans
- **Valeur** : UX professionnelle
- **Acceptance** : Given navigation complète, when utilisateur navigue, then transitions fluides sans erreurs

**Story 8 : Error Handling Complete**
- En tant qu'utilisateur, je veux des messages d'erreur clairs et une gestion robuste
- **Valeur** : Fiabilité en production
- **Acceptance** : Given erreur réseau/connexion, when problème survient, then messages informatifs et application stable

### Task Breakdown by Sprint

#### Sprint 1 : MVP Connectivity (Tasks 1-7)
- [ ] Task 1: Update project configuration for desktop targets only
  - File: `Monbsoft.MongoLite.MApp.csproj`
  - Action: Remove Android/iOS target frameworks, add MongoDB.Driver and CommunityToolkit.Mvvm packages
  - Notes: Keep Windows and MacCatalyst targets only

- [ ] Task 2: Configure dependency injection and services
  - File: `MauiProgram.cs`
  - Action: Add services registration for MongoDB service, ViewModels, and navigation
  - Notes: Set up CommunityToolkit.Mvvm and configure DI container

- [ ] Task 3: Create project structure folders
  - Files: New folders `Services/`, `Models/`, `ViewModels/`, `Pages/`
  - Action: Create folder structure for MVVM architecture
  - Notes: Organize code following standard .NET MAUI MVVM patterns

- [ ] Task 4: Create Docker development environment
  - Files: `Docker/docker-compose.yml`, `Docker/init-mongo.js`
  - Action: Create Docker Compose configuration with MongoDB and initialization script for sample data
  - Notes: Includes test collections (users, products) with sample documents for development testing

- [ ] Task 5: Implement MongoDB connection service
  - File: `Services/MongoDbService.cs`
  - Action: Create service for connecting to MongoDB, listing collections and documents
  - Notes: Handle connection strings, async operations, and error handling. Default connection string for Docker: mongodb://admin:password@localhost:27017/testdb

- [ ] Task 6: Create data models
  - Files: `Models/MongoConnection.cs`, `Models/MongoCollection.cs`, `Models/MongoDocument.cs`
  - Action: Define models for connection info, collection metadata, and generic document handling
  - Notes: Use System.Text.Json for generic document serialization

- [ ] Task 7: Create connection ViewModel
  - File: `ViewModels/ConnectionViewModel.cs`
  - Action: Implement ViewModel for MongoDB connection screen with validation
  - Notes: Handle connection string validation and async connection logic. Pre-fill Docker connection string for quick testing

#### Sprint 2 : Functional Exploration (Tasks 8-11)
- [ ] Task 8: Create collections ViewModel
  - File: `ViewModels/CollectionsViewModel.cs`
  - Action: Implement ViewModel for listing collections with observable collection
  - Notes: Async collection loading and error handling

- [ ] Task 9: Create documents ViewModel
  - File: `ViewModels/DocumentsViewModel.cs`
  - Action: Implement ViewModel for listing documents in a selected collection
  - Notes: Pagination support, search/filter capabilities

- [ ] Task 10: Update AppShell for navigation (minimal)
  - Files: `AppShell.xaml`, `AppShell.xaml.cs`
  - Action: Define basic routes for Connection and Collections pages
  - Notes: Configure minimal Shell navigation for first two screens

- [ ] Task 11: Create collections page
  - Files: `Pages/CollectionsPage.xaml`, `Pages/CollectionsPage.xaml.cs`
  - Action: Build list view of collections with selection navigation
  - Notes: Collection name, document count display

- [ ] Task 12: Create connection page
  - Files: `Pages/ConnectionPage.xaml`, `Pages/ConnectionPage.xaml.cs`
  - Action: Build UI for MongoDB connection with input validation
  - Notes: Connection string entry, test connection button, Docker quick-connect option

#### Sprint 3 : Complete Manipulation (Tasks 13-15)
- [ ] Task 13: Create documents page
  - Files: `Pages/DocumentsPage.xaml`, `Pages/DocumentsPage.xaml.cs`
  - Action: Build list view of documents with JSON preview
  - Notes: Document list with basic preview, navigation to detail

- [ ] Task 14: Create document detail page
  - Files: `Pages/DocumentDetailPage.xaml`, `Pages/DocumentDetailPage.xaml.cs`
  - Action: Build editor for JSON documents with save/delete functionality
  - Notes: JSON editor view, edit/save/delete buttons

- [ ] Task 15: Create document detail ViewModel
  - File: `ViewModels/DocumentDetailViewModel.cs`
  - Action: Implement ViewModel for viewing/editing individual documents
  - Notes: JSON editing, save/delete operations, change tracking

#### Sprint 4 : Production Ready (Tasks 16)
- [ ] Task 16: Complete AppShell navigation and finalize
  - Files: `AppShell.xaml`, `AppShell.xaml.cs`, `MainPage.xaml`, `MainPage.xaml.cs`
  - Action: Complete navigation structure, error handling throughout, clean up MainPage startup
  - Notes: Configure full Shell navigation, add error handling, finalize application flow

### Acceptance Criteria

#### Sprint 1 : MVP Connectivity
- [ ] AC 1: Given the app is launched, when the user enters valid MongoDB connection details, then the app successfully connects and displays available collections
- [ ] AC 2: Given Docker environment is running, when `docker-compose up`, then MongoDB accessible at mongodb://admin:password@localhost:27017/testdb

#### Sprint 2 : Functional Exploration
- [ ] AC 3: Given a connection is established, when the user selects a collection, then the app displays the list of collections with document counts
- [ ] AC 4: Given a collection is selected, when the user opens it, then the app displays the list of documents with basic preview information

#### Sprint 3 : Complete Manipulation
- [ ] AC 5: Given a document is selected, when the user clicks on it, then the app displays the full document content in an editable JSON format
- [ ] AC 6: Given a document is being viewed, when the user modifies JSON content and saves, then the changes are persisted to MongoDB and reflected in the document list
- [ ] AC 7: Given a document is being viewed, when the user deletes the document, then the document is removed from MongoDB and no longer appears in the list

#### Sprint 4 : Production Ready
- [ ] AC 8: Given invalid MongoDB connection details, when the user attempts to connect, then the app displays appropriate error messages
- [ ] AC 9: Given network connectivity issues, when MongoDB operations fail, then the app handles errors gracefully without crashing
- [ ] AC 10: Given the app is running on Windows or MacCatalyst, when the user navigates between screens, then all navigation works correctly using Shell navigation

## Dependencies

### External Dependencies
- MongoDB.Driver NuGet package for MongoDB connectivity
- CommunityToolkit.Mvvm NuGet package for MVVM infrastructure
- System.Text.Json for JSON document handling
- Docker Desktop for local development environment

### Runtime Dependencies
- MongoDB server instance accessible via network connection (or Docker container)
- .NET 10.0 runtime on target platform (Windows/MacCatalyst)
- Docker Desktop for local development environment (optional but recommended)

## Testing Strategy

### Unit Testing
- Test MongoDB service connectivity and collection listing
- Test ViewModels command execution and property updates
- Test JSON document serialization/deserialization
- Test navigation flow between ViewModels

### Integration Testing
- Test end-to-end connection flow from UI to MongoDB
- Test document CRUD operations through the full stack
- Test navigation between pages with proper data passing
- Test error handling throughout the application

### Manual Testing
- Verify connection functionality with different MongoDB configurations
- Test document editing with various JSON structures
- Verify app behavior with network connectivity issues
- Test UI responsiveness on both Windows and MacCatalyst platforms

## Notes

### High-Risk Items
- MongoDB driver compatibility with .NET MAUI desktop platforms
- JSON document size limitations for large documents in UI
- Connection string security handling in desktop application
- Performance when loading collections with many documents

### Known Limitations
- No offline mode - requires active network connection
- Limited to basic document editing (no complex query builder)
- Memory usage may be high with large document sets
- No user authentication beyond MongoDB connection string

### Future Considerations
- Implement user authentication system
- Add support for aggregation queries
- Implement document search and filtering
- Add export/import functionality for documents
- Support for MongoDB Atlas cloud services