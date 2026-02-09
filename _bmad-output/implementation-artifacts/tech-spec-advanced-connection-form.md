---
title: 'Advanced Connection Form'
slug: 'advanced-connection-form'
created: '2026-02-09'
status: 'completed'
stepsCompleted: [1, 2, 3, 4]
tech_stack: ['.NET 10 MAUI', 'CommunityToolkit.Mvvm', 'MongoDB.Driver', 'Shell Navigation']
files_to_modify: ['Pages/ConnectionPage.xaml', 'ViewModels/ConnectionViewModel.cs', 'Pages/AdvancedConnectionPage.xaml', 'Pages/AdvancedConnectionPage.xaml.cs', 'ViewModels/AdvancedConnectionViewModel.cs', 'MauiProgram.cs']
code_patterns: ['MVVM (ObservableObject, RelayCommand)', 'Dependency Injection (Singleton Services, Transient VMs/Pages)', 'Shell Routing', 'SetProperty pattern', 'Source Generators (CommunityToolkit.Mvvm)']
test_patterns: ['No tests for this story']
---

# Tech-Spec: Advanced Connection Form

**Created:** 2026-02-09

## Overview

### Problem Statement

Currently, users must manually type a complete MongoDB connection string (`mongodb://user:pass@host:port/db`) in a single text field. This is error-prone and unfriendly, especially for users who don't know the exact connection string format.

### Solution

Add a "..." button next to the existing connection string field on the ConnectionPage. This button opens an advanced form (popup/modal) with separate fields: server host, port, username, password, database name, and optional auth source. The form builds the connection string and connects directly.

### Scope

**In Scope:**
- "..." button on the existing ConnectionPage next to the connection string Entry
- Advanced connection form (modal popup via `Navigation.PushModalAsync`) with fields: Host, Port, Username, Password, Database, AuthSource (optional)
- Connect button in the advanced form that builds the connection string and connects
- Navigation to Collections on success (same flow as existing)

**Out of Scope:**
- SSL/TLS configuration
- Saved connections / connection history
- Replica set configuration
- Import/export of connection strings

## Context for Development

### Codebase Patterns

**Architecture:**
- **MVVM Strict**: Pages ↔ ViewModels ↔ Services (clean separation)
- **Dependency Injection**: `MauiProgram.cs` - Services = Singleton, ViewModels/Pages = Transient
- **Navigation**: Shell-based routing (`AppShell.xaml.cs` - `Routing.RegisterRoute()`)
- **Properties**: `SetProperty(ref _field, value)` pattern via `ObservableObject` base class
- **Commands**: `IAsyncRelayCommand` from CommunityToolkit.Mvvm
- **Source Generators**: `[ObservableProperty]` and `[RelayCommand]` attributes for boilerplate reduction

**Existing Connection Flow:**
1. User enters connection string in `ConnectionPage.xaml` Entry
2. `ConnectionViewModel.ConnectAsync()` calls `MongoDbService.ConnectAsync(connectionString)`
3. On success → `Shell.Current.GoToAsync("collections")`

### Files to Reference

| File | Purpose | Key Elements |
| ---- | ------- | ------------ |
| `Pages/ConnectionPage.xaml` | Connection UI | Entry for connection string, Connect/Test buttons |
| `Pages/ConnectionPage.xaml.cs` | Code-behind | Constructor with DI for ViewModel |
| `ViewModels/ConnectionViewModel.cs` | Connection logic | `ConnectAsync()`, `TestConnectionAsync()`, properties with `SetProperty()` |
| `Services/MongoDbService.cs` | MongoDB client | `ConnectAsync(string connectionString)` - validates, pings DB |
| `MauiProgram.cs` | Service registration | DI setup for Services, VMs, Pages |
| `AppShell.xaml.cs` | Navigation routing | `Routing.RegisterRoute("collections", typeof(CollectionsPage))` |
| `Models/MongoConnection.cs` | Connection model | `DefaultDockerConnection` constant |

### Technical Decisions

**Party Mode Insights:**

1. **Validation Strategy (Amelia/Winston):**
   - Host: Required, non-empty, no spaces
   - Port: Required, numeric, range 1-65535
   - Database: Required, non-empty
   - Username/Password/AuthSource: Optional
   - Use `CanExecute` pattern for RelayCommand validation
   - Display `ErrorMessage` for validation failures

2. **Connection String Builder (Winston/Amelia):**
   - Simple string interpolation in ViewModel (no separate builder class)
   - Pattern: `mongodb://{user}:{pass}@{host}:{port}/{db}?authSource={authSource}`
   - Handle optional fields gracefully (empty auth, optional authSource param)

3. **UX Design (Sally):**
   - 6 fields: Host, Port, Username, Password, Database, AuthSource
   - Default values: Host=`localhost`, Port=`27017`
   - Password field: `IsPassword="True"`
   - AuthSource: Label with `(Optional)`
   - Progressive disclosure: Optional fields clearly marked
   - ErrorMessage feedback for validation errors

4. **Architecture (Winston):**
   - `AdvancedConnectionViewModel` injects `MongoDbService` (reuse existing service)
   - Modal navigation: `Navigation.PushModalAsync(AdvancedConnectionPage)`
   - On success: `await Navigation.PopModalAsync()` then update parent `ConnectionString` for state coherence
   - Parent ViewModel refresh: Pass callback or use MessagingCenter for state sync

5. **Testing (Amelia):**
   - No unit tests for this story (no test infrastructure exists in project)
   - Manual testing sufficient for initial release

### Dependencies

**NuGet Packages (Already Installed):**
- `CommunityToolkit.Mvvm` - MVVM helpers
- `MongoDB.Driver` - MongoDB client

**Services:**
- `MongoDbService` (Singleton) - Connection management, already exists

**Navigation:**
- Shell routing - `collections` route already registered

## Implementation Plan

### Tasks

Implementation order follows dependency chain (lowest level → highest level):

- [x] **Task 1: Create AdvancedConnectionViewModel**
  - File: `Monbsoft.MongoLite.MApp/ViewModels/AdvancedConnectionViewModel.cs`
  - Action: Create new ViewModel inheriting `ObservableObject`
  - Properties: `Host` (default "localhost"), `Port` (default "27017"), `Username`, `Password`, `Database`, `AuthSource`, `ErrorMessage`, `IsConnecting`
  - Use `[ObservableProperty]` attribute for source generation
  - Inject `MongoDbService` via constructor
  - Command: `ConnectCommand` with `[RelayCommand]` - builds connection string, calls `MongoDbService.ConnectAsync()`, navigates on success
  - Validation: `CanConnect()` method validates Host (non-empty), Port (1-65535 numeric), Database (non-empty)
  - Connection string builder: `BuildConnectionString()` - handles optional auth fields gracefully

- [x] **Task 2: Create AdvancedConnectionPage XAML**
  - File: `Monbsoft.MongoLite.MApp/Pages/AdvancedConnectionPage.xaml`
  - Action: Create new ContentPage with BindingContext `AdvancedConnectionViewModel`
  - Layout: VerticalStackLayout with 6 Entry fields (Host, Port, Username, Password, Database, AuthSource)
  - Defaults: Host placeholder="localhost", Port placeholder="27017"
  - Password field: `IsPassword="True"`
  - AuthSource label: Include "(Optional)" text
  - Connect button: Bound to `ConnectCommand`
  - ActivityIndicator: Bound to `IsConnecting`
  - ErrorMessage Label: Bound to `ErrorMessage`, Red text

- [x] **Task 3: Create AdvancedConnectionPage Code-Behind**
  - File: `Monbsoft.MongoLite.MApp/Pages/AdvancedConnectionPage.xaml.cs`
  - Action: Create partial class with DI constructor
  - Constructor: Inject `AdvancedConnectionViewModel`, set `BindingContext`
  - Pattern: Follow existing `ConnectionPage.xaml.cs` structure

- [x] **Task 4: Modify ConnectionViewModel - Add OpenAdvancedFormCommand**
  - File: `Monbsoft.MongoLite.MApp/ViewModels/ConnectionViewModel.cs`
  - Action: Add new command `OpenAdvancedFormCommand` with `[RelayCommand]` attribute
  - Implementation: `await Navigation.PushModalAsync(new AdvancedConnectionPage(...))`
  - Note: Requires creating `AdvancedConnectionPage` instance with DI-resolved ViewModel

- [x] **Task 5: Modify ConnectionPage XAML - Add Advanced Button**
  - File: `Monbsoft.MongoLite.MApp/Pages/ConnectionPage.xaml`
  - Action: Add Button with text "⚙️ Advanced" next to existing Entry
  - Use HorizontalStackLayout to position Entry + Button side-by-side
  - Button Command: Bound to `OpenAdvancedFormCommand`

- [x] **Task 6: Register New Components in DI**
  - File: `Monbsoft.MongoLite.MApp/MauiProgram.cs`
  - Action: Add service registrations in `CreateMauiApp()` method
  - Add: `builder.Services.AddTransient<AdvancedConnectionViewModel>();`
  - Add: `builder.Services.AddTransient<AdvancedConnectionPage>();`
  - Place after existing ViewModel/Page registrations

### Acceptance Criteria

**Happy Path:**

- [ ] **AC1:** Given I am on ConnectionPage, when I click "⚙️ Advanced" button, then AdvancedConnectionPage modal appears with 6 fields (Host, Port, Username, Password, Database, AuthSource)

- [ ] **AC2:** Given AdvancedConnectionPage is open, when form loads, then Host field shows "localhost" and Port shows "27017" as defaults

- [ ] **AC3:** Given I fill Host="localhost", Port="27017", Database="testdb" (no auth), when I click Connect, then connection string "mongodb://localhost:27017/testdb" is built and connection succeeds, navigating to Collections page

- [ ] **AC4:** Given I fill Host="localhost", Port="27017", Username="admin", Password="password", Database="testdb", when I click Connect, then connection string "mongodb://admin:password@localhost:27017/testdb" is built and connection succeeds

- [ ] **AC5:** Given I fill Host="localhost", Port="27017", Username="admin", Password="password", Database="testdb", AuthSource="admin", when I click Connect, then connection string "mongodb://admin:password@localhost:27017/testdb?authSource=admin" is built and connection succeeds

- [ ] **AC6:** Given successful connection from AdvancedConnectionPage, when navigation to Collections occurs, then modal is dismissed (PopModalAsync) and parent ConnectionViewModel.ConnectionString is updated with the built connection string

**Validation & Error Handling:**

- [ ] **AC7:** Given AdvancedConnectionPage is open with empty Host field, when I attempt to click Connect, then button is disabled (CanConnect returns false)

- [ ] **AC8:** Given AdvancedConnectionPage is open with empty Database field, when I attempt to click Connect, then button is disabled (CanConnect returns false)

- [ ] **AC9:** Given AdvancedConnectionPage is open with Port="abc" (non-numeric), when I attempt to click Connect, then button is disabled (CanConnect returns false)

- [ ] **AC10:** Given AdvancedConnectionPage is open with Port="99999" (out of range), when I attempt to click Connect, then button is disabled (CanConnect returns false)

- [ ] **AC11:** Given I fill valid fields but MongoDB server is unreachable, when I click Connect, then ErrorMessage displays "Failed to connect to MongoDB. Please check your connection string." in red text

- [ ] **AC12:** Given connection attempt is in progress, when waiting for response, then ActivityIndicator is visible and spinning

**Edge Cases:**

- [ ] **AC13:** Given I fill only required fields (Host, Port, Database) with no auth, when I click Connect, then connection string is built without username/password segment ("mongodb://host:port/db")

- [ ] **AC14:** Given Password field is focused, when I type characters, then text is masked (IsPassword="True" works correctly)

## Additional Context

### Dependencies

**Services Required:**
- `MongoDbService` - Already exists as Singleton, will be injected into `AdvancedConnectionViewModel`

**Navigation:**
- Modal navigation via `Navigation.PushModalAsync()` / `PopModalAsync()`
- Shell navigation via `Shell.Current.GoToAsync("collections")` (already configured)

**No new NuGet packages required.**

### Testing Strategy

**Manual Testing:**

1. **Basic Modal Flow:**
   - Open ConnectionPage → Click "⚙️ Advanced" → Verify modal opens
   - Fill required fields → Click Connect → Verify navigation to Collections
   - Verify modal dismisses after successful connection

2. **Validation Testing:**
   - Test empty Host → Connect button disabled
   - Test empty Database → Connect button disabled
   - Test non-numeric Port → Connect button disabled
   - Test Port out of range (0, 65536) → Connect button disabled

3. **Connection String Building:**
   - Test no auth: `mongodb://localhost:27017/testdb`
   - Test with auth: `mongodb://admin:password@localhost:27017/testdb`
   - Test with authSource: `mongodb://admin:password@localhost:27017/testdb?authSource=admin`

4. **Error Handling:**
   - Test unreachable server → ErrorMessage displays
   - Test invalid connection string → ErrorMessage displays

5. **State Synchronization:**
   - After successful connection from modal, verify parent `ConnectionString` field is updated

**No Unit Tests:** Project has no existing test infrastructure. Manual testing sufficient for initial release.

### Notes

- The existing simple connection string field remains as-is; the advanced form is an alternative way to connect.
- The advanced form should build a standard MongoDB connection string from the individual fields.
- **Party Mode Decision:** Modal popup via `Navigation.PushModalAsync` (no extra NuGet dependency).
- **Party Mode Decision:** Default values: Host=`localhost`, Port=`27017`. Smart UX with progressive disclosure.
- **Party Mode Decision:** On success from modal → `PopModalAsync()` then `GoToAsync("collections")`. Update parent ConnectionString for state coherence.
- **Party Mode Decision:** Validation via CanExecute pattern - Host (required), Port (1-65535), Database (required).
- **Party Mode Decision:** String interpolation for connection string builder (no separate class).
- **Party Mode Decision:** No unit tests for this story (no existing test infrastructure).

## Review Notes

- **Adversarial review completed:** 2026-02-09
- **Findings:** 12 total (1 Critical, 2 High, 5 Medium, 3 Low, 2 Minor)
- **Findings addressed:** 3 fixed (F1 Critical, F3 High, AuthSource bonus fix)
- **Findings skipped:** 9 (F2 accepted as limitation, F4-F12 future enhancements)
- **Resolution approach:** Auto-fix
- **Build status:** ✅ Successful
